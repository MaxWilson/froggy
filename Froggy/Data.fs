﻿module Froggy.Data
open Froggy.Common
open Froggy.Properties

// interface for external interactions with user and/or file system
type IO<'t> =
  {
    save: string -> 't -> unit
    load: string -> 't option
    query: string -> string
    output: string -> unit
  }
  with
  static member Fail =
    let fail _ = failwith "Not implemented"
    { save = thunk fail; load = fail; query = fail; output = fail } : IO<'t>

type Id = int
type PropertyName = string

[<AbstractClass>]
type PropertyMetadata<'union>(name : PropertyName) =
  member this.Name = name
  abstract member TryParse: string -> 'union option
type PropertyMetadata<'union,'store,'t>(name : PropertyName, create: PropertyValue<'store, 't> -> 'union, get: 'union -> PropertyValue<'store, 't> option, tryParse: string -> 't option) =
  inherit PropertyMetadata<'union>(name)
  member this.Create = create
  member this.Get = get
  override this.TryParse v =
    match tryParse v with
    | Some v -> PropertyValue([Permanent, Value v]) |> create |> Some
    | None -> None
  member this.FromUnion v = get v
  member this.ToUnion v = create v

type StatBank<'propertyValueType> = Map<Id*PropertyName, 'propertyValueType>
type Data<'propertyValueUnion> = {
  roster: Map<string, Id>
  reverseRoster: Map<Id, string>
  mapping: StatBank<'propertyValueUnion>
  properties: Map<string, PropertyMetadata<'propertyValueUnion>>
  parentScope: Data<'propertyValueUnion> option
  round: int option
}

module Data =
  let empty = { roster = Map.empty; reverseRoster = Map.empty; mapping = Map.empty; properties = Map.empty; parentScope = None; round = None }
  let addProps (props: PropertyMetadata<_> seq) d = { d with properties = props |> Seq.fold (fun m p -> m |> Map.add p.Name p) d.properties }
  let withProps (props: PropertyMetadata<_> seq) = empty |> addProps props
  let getParent<'mt, 't> : RecursiveOptionLens<Data<'t>> = Lens.lens (fun x -> x.parentScope) (fun v x -> { x with parentScope = v })
  let newRound d = { d with round = Some ((defaultArg d.round 0) + 1) }
  let set (prop: PropertyMetadata<'union, _ ,_>) (id: Id) scope v d =
    let currentValue =
      match d.mapping |> Map.tryFind (id, prop.Name) with
      | Some pv ->
        defaultArg (prop.FromUnion pv) PropertyValue.empty
      | None -> PropertyValue.empty
    let pv': 'union = PropertyValue.add scope v currentValue |> prop.ToUnion
    { d with mapping = d.mapping |> Map.add (id, prop.Name) pv' }

module SimpleProperties =
  type Conditions = {
                      CharmedBy: Id list; Blinded: bool; Deafened: bool; Exhausted: int; FrightenedOf: Id list;
                      Grappled: bool; Incapacitated: bool; Invisible: bool; Paralyzed: bool; Petrified: bool;
                      Poisoned: bool; Prone: bool; Restrained: bool; Stunned: bool; Unconscious: bool; }
  module Conditions =
    let empty = { CharmedBy = []; Blinded = false; Deafened = false; Exhausted = 0; FrightenedOf = [];
                  Grappled = false; Incapacitated = false; Invisible = false; Paralyzed = false; Petrified = false;
                  Poisoned = false; Prone = false; Restrained = false; Stunned = false; Unconscious = false }
    let computeClosure (c:Conditions) =
      { c with Incapacitated = c.Incapacitated || c.Petrified || c.Paralyzed || c.Stunned || c.Unconscious;
                Prone = c.Prone || c.Unconscious } // todo: how to prevent prone from ending just because Unconscious ends
    let isIncapacitated c =
      (computeClosure c).Incapacitated
  type SimpleStore = Data<PropertyValueUnion>
  and PropertyValue<'t> = PropertyValue<SimpleStore, 't>
  and PropertyValueUnion = Condition of PropertyValue<Conditions> | Number of PropertyValue<int> | Text of PropertyValue<string>

  let numberValue n = (PropertyValue [Permanent, Value n])
  let parseNumber input =
    match System.Int32.TryParse input with
    | true, v -> Some v
    | _ -> None
  let parseText input = if String.length input > 0 then Some input else None
  let NumberProperty name = PropertyMetadata<PropertyValueUnion, SimpleStore, int>(name, Number, (function Number n -> Some n | v -> None), parseNumber)
  let TextProperty name = PropertyMetadata<PropertyValueUnion, SimpleStore, string>(name, Text, (function Text t -> Some t | v -> None), parseText)

  let hp = NumberProperty "HP"
  let isRound x s = s.round = Some x
  let addTemporaryCondition duration addCondition (PropertyValue cs) =
    PropertyValue(((Temporary duration), (Transform addCondition))::cs)
  let addProne c = { c with Prone = true }
  let pv : PropertyValue<Data<PropertyValueUnion>,Conditions> = PropertyValue.empty |> addTemporaryCondition (isRound 1) addProne
  let data = Data.withProps [hp] |> Data.set hp 1 Lasting (Value 4)
  let v = (PropertyValue.computeCurrentValue Data.getParent data pv)
  printf "Are we prone? %A" v

  type PropertyValueUnion = Text of string | Number of int | Conditions of Conditions
  let asNumber = function Number n -> n | v -> failwithf "Invalid cast: %A is not a number" v
  let asText = function Text n -> n | v -> failwithf "Invalid cast: %A is not text" v

  let Name = TextProperty("Name")
  let HP = NumberProperty("HP")
  let SP = NumberProperty("SP")
  let XP = NumberProperty("XP")
  let Properties =
    ([ Name; HP; SP; XP ] : Property list)
    |> List.map (fun t -> t.Name, t)
    |> Map.ofList

open SimpleProperties
// gets value from the user
let acquireValue (io:IO<_>) (name: string) (prop: Property) =
  let response = io.query (sprintf "What is %s's %s?" name prop.Name)
  let rec getPropertyValue (response: string) =
    match prop.TryParse response with
    | Some v -> v
    | None ->
      getPropertyValue (io.query (sprintf "Sorry, I didn't understand '%s'.\nWhat is %s's %s?" response name prop.Name))
  getPropertyValue response

let lookup io (prop: Property) id (data : Data<_, _>) =
  match Map.tryFind (id, prop.Name) data.mapping with
  | Some(v) -> v, data
  | None ->
    let v = acquireValue io data.reverseRoster.[id] prop
    v, { data with mapping = data.mapping |> Map.add (id, prop.Name) v }
