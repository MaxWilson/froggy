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

// Here's a simple grammar which demonstrates usage
let (|End|_|) ((ctx, ix): Input) =
  if ix = ctx.input.Length then Some() else None

let (|Str|_|) (str: string) ((ctx, ix): Input) =
  if ix + str.Length <= ctx.input.Length && System.String.Equals(ctx.input.Substring(ix, str.Length), str, System.StringComparison.InvariantCultureIgnoreCase) then Some((ctx, ix+str.Length)) else None

// Optional whitespace
let (|OWS|) ((ctx, ix): Input) =
  let rec seek i =
    if i >= ctx.input.Length || ctx.input.[i] <> ' ' then i
    else seek (i+1)
  ctx, (seek ix)

let (|YesNo|_|) = function
  | Str "No" rest -> Some(0I, rest)
  | Str "Yes" rest -> Some(1I, rest)
  | _ -> None
let rec (|YesNos|_|) = pack(
  function
  | YesNos(v1, OWS(YesNo(v2, rest))) -> Some(v1*10I+v2, rest)
  | YesNo(v, rest) -> Some(v, rest)
  | _ -> None
  )

let q input =
  match ParseContext.Init input with
  |YesNos(v, End) -> v.ToString()
  | _ -> "parse failure"

q "Yes" = "1" // basic query, showing conversion of Yes/No to binary
q "yes" = "1" // shows that it's case-insensitive
q "yesyesnoyesnoyes" = "110101" // show a more complex conversion
q "yesn" = "parse failure" // show what happens with bad input
q "yes no yes yesNonoYES" = "1011001" // show that interior spacing can be ignored
q "yesyesyesyesyesyesyesyesnoyesnoyesyesyesyesyesyesyesno" = "1111111101011111110" // can handle large numbers
