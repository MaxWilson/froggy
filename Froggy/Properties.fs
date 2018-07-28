#if INTERACTIVE
#I __SOURCE_DIRECTORY__
#load "Common.fs"
#else
module Froggy.Properties
#endif
  open Froggy.Common

  type PropertyName = string
  type RoundNumber = int
  type PropertyScope<'store> = Permanent | Lasting | Temporary of ('store -> bool)
  type PropertyValueComponent<'store, 'args, 't> = Value of 't | Transform of ('t -> 't) | Computed of ('args -> 'store -> 't)
  type PropertyValue<'store, 'args, 't> = PropertyValue of (PropertyScope<'store> * PropertyValueComponent<'store, 'args, 't>) list
  type ParentLens<'t> = Lens<'t, 't option, 't option, 't>
  let computeValue (scopeFilter: PropertyScope<_> -> bool) (getParentLens: ParentLens<'store> option) (args: 'args) (store: 'store) (PropertyValue(vs): PropertyValue<'store, 'args, 't>) =
    let rec applyValue v rest =
      match v with
      | Value v -> v
      | Transform t ->
        let v = eval rest
        t v
      | Computed f -> f args store
    and eval =
      function
        | (scope, _)::rest when scopeFilter scope = false ->
          // do NOT apply since it's for a different scope
          eval rest
        | ((Permanent | Lasting), v)::rest ->
          applyValue v rest
        | (Temporary(pred), v)::rest when pred store ->
          applyValue v rest
        | (Temporary(_), _)::rest ->
          // do NOT apply since pred was false
          eval rest
        | v -> matchfail v
    eval vs
  let computePermanentValue getParentLens = computeValue (function Permanent -> true | _ -> false) getParentLens
  let computeCurrentValue getParentLens = computeValue (thunk true) getParentLens

  type MyScope = { parent: MyScope option; data: Map<string, string> }
  let parent = { parent = None; data = ["HP", "330"; "Name", "Remorhaz"] |> Map.ofSeq }
  let data = { parent = Some parent; data = ["X", "32"; "Y", "-27"; "Name", "Reince"] |> Map.ofSeq }
  let getParent = Lens.lens (function { parent = x } -> x | v -> matchfail v) (fun v x -> { x with parent = v })
  let l = getParent
  Lens.view l data
  Lens.over l (Option.map <| fun p -> { p with data = p.data |> Map.add "Class" "Monster"}) data

  module SimpleProperties =
    type PropertyValueUnion = Text of string | Number of int
    let asNumber = function Number n -> n | v -> failwithf "Invalid cast: %A is not a number" v
    let asText = function Text n -> n | v -> failwithf "Invalid cast: %A is not text" v

    [<AbstractClass>]
    type Property(name : PropertyName) =
      member this.Name = name
      abstract member TryParse: string -> PropertyValueUnion option
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
