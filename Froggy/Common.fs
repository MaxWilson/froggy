module Froggy.Common

let flip f x y = f y x
let thunk x _ = x
let betweenInclusive bound1 bound2 x = (min bound1 bound2) <= x && x <= (max bound1 bound2)
let thunk1 f x _ = f x
let thunk2 f x y _ = f x y
let matchfail v = failwithf "No match found for %A" v

// Lens code based on http://www.fssnip.net/7Pk/title/Polymorphic-lenses by Vesa Karvonen

type Lens<'InState,'ValGet,'ValSet,'OutState> = ('ValGet -> Option<'ValSet>) -> 'InState -> Option<'OutState>
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
let randomChoice (lst: _ array) =
  if lst.Length = 0 then failwith "Cannot choose from an empty list"
  else lst.[random.Next(lst.Length)]