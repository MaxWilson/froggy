module Froggy.Common

let flip f x y = f y x
let thunk x _ = x
let betweenInclusive bound1 bound2 x = (min bound1 bound2) <= x && x <= (max bound1 bound2)
let thunk1 f x _ = f x
let thunk2 f x y _ = f x y
let fail() = failwith "Not implemented" // occasionally useful while developing, as a placeholder to avoid confusing type inference
type MatchFailException(msg) = inherit System.InvalidOperationException(msg)
let matchfail v = sprintf "No match found for %A" v |> MatchFailException |> raise

// Lens code based on http://www.fssnip.net/7Pk/title/Polymorphic-lenses by Vesa Karvonen

type Lens<'InState,'ValGet,'ValSet,'OutState> = ('ValGet -> Option<'ValSet>) -> 'InState -> Option<'OutState>
type SimpleLens<'Outer, 'Inner> = Lens<'Outer, 'Inner, 'Inner, 'Outer>
type RecursiveOptionLens<'t> = SimpleLens<'t, 't option>
module Lens =
  let view (l: Lens<'InState,'ValGet,'ValSet,'OutState>) s =
    let r = ref Unchecked.defaultof<_>
    s |> l (fun a -> r := a; None) |> ignore
    !r

  let over (l: Lens<'InState,'ValGet,'ValSet,'OutState>) f =
    l (f >> Some) >> function Some t -> t | _ -> failwith "Impossible"
  let set (l: Lens<'InState,'ValGet,'ValSet,'OutState>) b = over l <| fun _ -> b
  let lens get set : Lens<'InState,'ValGet,'ValSet,'OutState> =
    fun f s ->
      ((get s |> f : Option<_>) |> Option.map (fun f -> set f s))


let emptyString = System.String.Empty
module String =
  let join delimiter strings = System.String.Join((delimiter: string), (strings: string seq))
  let equalsIgnoreCase lhs rhs = System.String.Equals(lhs, rhs, System.StringComparison.InvariantCultureIgnoreCase)
  let firstWord input =
    match Option.ofObj input with
    | Some(v:string) -> v.Trim().Split(' ') |> Seq.head
    | None -> input

let random = System.Random()
let rollOneDie d = random.Next(d) + 1
let randomChoice (lst: _ array) =
  if lst.Length = 0 then failwith "Cannot choose from an empty list"
  else lst.[random.Next(lst.Length)]

module Fraction =
  open System.Numerics

  let ratio precision (m:BigInteger) (n:BigInteger) = (m*(10I ** precision)/n |> float)/(float (10I**precision))

  type Fraction = { numerator: BigInteger; denominator: BigInteger }
  let create n m = { numerator = n; denominator = m }
  let toFloat { numerator = n; denominator = m } = ratio 3 n m
  let toPercent { numerator = n; denominator = m } = ratio 1 (n*100I) m