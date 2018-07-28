#if INTERACTIVE
#I __SOURCE_DIRECTORY__
#load "Common.fs"
#else
module Froggy.Properties
#endif
  open Froggy.Common

  type PropertyName = string
  type RoundNumber = int
  type PropertyScope<'store> = Permanent | Lasting | Temporary of ('store -> bool) | Inherit
  type PropertyValueComponent<'store, 't> = Value of 't | Transform of ('t -> 't) | Computed of ('store -> 't)
  type PropertyValue<'store, 't> = PropertyValue of (PropertyScope<'store> * PropertyValueComponent<'store, 't>) list
  let computeValue (scopeFilter: PropertyScope<_> -> bool) (store: 'store) (PropertyValue(vs): PropertyValue<'store, 't>) =
    let rec applyValue v rest =
      match v with
      | Value v -> v
      | Transform t ->
        let v = eval rest
        t v
      | Computed f -> f store
    and eval =
      function
        | (scope, _)::rest when scopeFilter scope = false ->
          // do NOT apply since it's for a different scope
          eval rest
        | ((Permanent | Lasting), v)::rest ->
          applyValue v rest
        | (Temporary(pred), v)::rest when pred store ->
          applyValue v rest
        | (Temporary(pred), _)::rest ->
          // do NOT apply since pred was false
          eval rest
        | v -> failwithf "Could not evaluate %A" v
    eval vs
  let computePermanentValue store pvs = computeValue (function Permanent -> true | _ -> false) store pvs
  let computeCurrentValue store pvs = computeValue (thunk true) store pvs

  let isDivisibleBy x arg = arg % x = 0
  let v = (PropertyValue [Temporary(isDivisibleBy 4), Value 3; Temporary (flip (>=) 5), Transform(flip (/) 2); Permanent, Transform((+) 2); Lasting, Computed id])
  for x in [-10..10] do
    printfn "%d %d" x (computeCurrentValue x v)

  type PropertyValue = Text of string | Number of int
  let asNumber = function Number n -> n | v -> failwithf "Invalid cast: %A is not a number" v
  let asText = function Text n -> n | v -> failwithf "Invalid cast: %A is not text" v

  [<AbstractClass>]
  type Property(name : PropertyName) =
    member this.Name = name
    abstract member TryParse: string -> PropertyValue option
  type NumberProperty(name) =
    inherit Property(name)
    override this.TryParse input =
      match System.Int32.TryParse input with
      | true, v -> Some <| Number v
      | _ -> None
  type TextProperty(name) =
    inherit Property(name)
    override this.TryParse input = Some <| Text input

  let Name = TextProperty("Name")
  let HP = NumberProperty("HP")
  let SP = NumberProperty("SP")
  let XP = NumberProperty("XP")
  let Properties =
    ([ Name; HP; SP; XP ] : Property list)
    |> List.map (fun t -> t.Name, t)
    |> Map.ofList
