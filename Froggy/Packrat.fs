#if INTERACTIVE
#else
module Froggy.Packrat
#endif

(* DEEP MAGIC BEGINS HERE
The following memo function allows construction of left-recursive grammars using a stateful
iterated algorithm. Don't mess with the code in this file unless you've read the computer
science papers involved and thoroughly understand them, and also have unit tests for the change
you want to make.
*)

type Id = int
type Pos = int
type ParseResult = Success of obj * Pos | Fail
type ParseContext =
  {
    input: string
    active: Set<Pos * Id> ref
    settled: Map<(Pos * Id), ParseResult> ref
  }
  with
  static member Init(input) = { input = input; active = ref Set.empty; settled = ref Map.empty }, 0
type Input = ParseContext * Pos
type Rule<'a> = (Input -> ('a * Input) option)

let nextId =
  let mutable i = 0
  fun() ->
    i <- i + 1
    i

let pack (rule: Rule<'t>) : Input -> ('t * Input) option =
  let id: Id = nextId()
  let eval (input: Input) =
    let ctx, (pos: Pos) = input
    let active' = ctx.active.Value
    let key = (pos, id)
    ctx.active := ctx.active.Value.Remove key // mark visited
    match ctx.settled.Value.TryFind key with
    | Some(Success(v, endpos)) ->
      Some(unbox v, ((ctx, endpos) : Input)) // cache says the biggest possible match is v, ending at endpos
    | Some(Fail) ->
      None // cache says it will fail
    | None -> // nothing settled yet--we have to grow a match or failure
      let settled = ctx.settled.Value // in left recursive case, holding on to an old reference lets us "forget" unsettled results
      let active = ctx.active.Value
      ctx.active := active.Add key
      ctx.settled := settled.Add(key, Fail) // initialize intermediate set to failure to prevent infinite left-recursion
      let evalResult = rule (ctx, pos)
      let hadLeftRecursion = not <| ctx.active.Value.Contains(key) // todo: check and see if any heads grew in the same position
      ctx.active := ctx.active.Value.Remove key // Clean up after ourselves, though it shouldn't be necessary
      let grow seed settled =
        let rec grow seed settled =
          // update the intermediate cache before re-evaluating
          ctx.settled := settled
          match seed, (rule (ctx, pos)) with // we just had our first success--try to grow!
          | None, Some(v, (_, endpos)) ->
            grow (Some (box v, endpos)) (settled |> Map.add key (Success (box v, endpos)))
          | Some(_, oldendpos), Some(v, (_, endpos) as rest) when endpos > oldendpos -> // we just grew, let's try growing again!
            grow (Some (box v, endpos)) (settled |> Map.add key (Success (box v, endpos)))
          | Some(v, endpos), _ ->
            Some(v, endpos)
          | None, None ->
            None
        // we want to revert to the original "settled" before memoizing our results
        match grow seed settled with
          | Some(v, endpos) ->
            ctx.settled := (settled |> Map.add key (Success (v, endpos))) // remember the largest success
            Some(unbox v, (ctx, endpos))
          | None ->
            ctx.settled := (settled |> Map.add key Fail) // remember the failure
            None
      match evalResult with
      | None ->
        if hadLeftRecursion then
          // since left recursion happened, we use our original "settled" as a start set, ignoring all the intermediate results already
          // in ctx.settled, because using them could cause false negatives on parse recognition
          grow None (settled |> Map.add key Fail)
        else
          // no left recursion, so we can take all the intermediate results in ctx.settled instead of undoing back to settled
          ctx.settled := ctx.settled.Value |> Map.add key Fail // remember the failure
          None
      | Some(v, ((ctx, outpos) as output)) ->
        if hadLeftRecursion then
          // since left recursion happened, we use our original "settled" as a start set, ignoring all the intermediate results already
          // in ctx.settled, because using them could cause false negatives on parse recognition
          grow (Some(box v, outpos)) (settled |> Map.add key (Success (box v, outpos)))
        else
          ctx.settled := ctx.settled.Value |> Map.add key (Success (box v, outpos)) // remember the success
          Some(v, output)
  eval // return eval function

// Here's some basic parser primitives that might be useful for anything

let (|End|_|) ((ctx, ix): Input) =
  if ix = ctx.input.Length then Some() else None

let (|Str|_|) (str: string) ((ctx, ix): Input) =
  if ix + str.Length <= ctx.input.Length && System.String.Equals(ctx.input.Substring(ix, str.Length), str, System.StringComparison.InvariantCultureIgnoreCase) then Some((ctx, ix+str.Length)) else None

// set up some basic alphabets
let alpha = Set<_>['A'..'Z'] + Set<_>['a'..'z']
let numeric = Set<_>['0'..'9']
let whitespace = Set<_>[' '; '\t'; '\n'; '\r']
let arithmeticOperators = Set<_>['+'; '-']
let alphanumeric = alpha + numeric

let (|Chars|_|) alphabet ((ctx, ix): Input) =
  let rec seek i =
    if i < ctx.input.Length && Set.contains ctx.input.[i] alphabet then seek (i+1)
    else i
  match seek ix with
  | endpos when endpos > ix -> Some(ctx.input.Substring(ix, endpos - ix), (ctx, endpos))
  | _ -> None

let (|Word|_|) = (|Chars|_|) alphanumeric

let (|Words|_|) =
  let set = (alphanumeric |> Set.add ' ')
  function
  | Chars set (words, rest) -> Some(words.Trim(), rest)
  | _ -> None

let (|AnyCase|) (input: string) = input.ToLowerInvariant()

let (|Any|) ((ctx, ix): Input) =
  ctx.input.Substring(ix), (ctx, ctx.input.Length)

// Optional whitespace
let (|OWS|) ((ctx, ix): Input) =
  let rec seek i =
    if i < ctx.input.Length && Set.contains ctx.input.[i] whitespace then seek (i+1)
    else i
  ctx, (seek ix)
// Required whitespace
let (|WS|_|) ((ctx, ix): Input) =
  let rec seek i =
    if i < ctx.input.Length && Set.contains ctx.input.[i] whitespace then seek (i+1)
    else i
  match seek ix with
  | endx when endx > ix -> Some(ctx, endx)
  | _ -> None

let (|Int|_|) = function
  | OWS(Chars numeric (v, OWS(rest))) ->
    match System.Int32.TryParse(v) with
    | true, v -> Some(v, rest)
    | _ -> None
  | _ -> None
