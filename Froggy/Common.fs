module Froggy.Common

let flip f x y = f y x
let thunk x _ = x
let betweenInclusive bound1 bound2 x = (min bound1 bound2) <= x && x <= (max bound1 bound2)

// Lens code based on http://www.fssnip.net/7Pk/title/Polymorphic-lenses by Vesa Karvonen

type Lens<'s,'t,'a,'b> = ('a -> Option<'b>) -> 's -> Option<'t>
module Lens =
  let view l s =
    let r = ref Unchecked.defaultof<_>
    s |> l (fun a -> r := a; None) |> ignore
    !r

  let over l f =
    l (f >> Some) >> function Some t -> t | _ -> failwith "Impossible"
  let set l b = over l <| fun _ -> b
  let lens get set = fun f s ->
    (get s |> f : Option<_>) |> Option.map (fun f -> set f s)


let emptyString = System.String.Empty
module String =
  let join delimiter strings = System.String.Join((delimiter: string), (strings: string seq))
  let equalsIgnoreCase lhs rhs = System.String.Equals(lhs, rhs, System.StringComparison.InvariantCultureIgnoreCase)