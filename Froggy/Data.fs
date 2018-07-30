module Froggy.Data
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
[<AbstractClass>]
type PropertyMetadata<'union, 't>(name: PropertyName) =
  inherit PropertyMetadata<'union>(name)
  abstract member FromUnion: 'union -> 't option
  abstract member ToUnion: 't -> 'union

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
  let set (prop: PropertyMetadata<'union, PropertyValue<_, _>>) (id: Id) duration v d =
    let currentValue =
      match d.mapping |> Map.tryFind (id, prop.Name) with
      | Some pv ->
        defaultArg (prop.FromUnion pv) PropertyValue.empty
      | None -> PropertyValue.empty
    let pv': 'union = PropertyValue.add duration v currentValue |> prop.ToUnion
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
  and PropertyValueUnion =
    | Condition of PropertyValue<Conditions>
    | Number of PropertyValue<int>
    | Text of PropertyValue<string>
    | NameList of PropertyValue<string[]>

  type Property<'t>(name : PropertyName,
                    create: PropertyValue<SimpleStore, 't> -> PropertyValueUnion,
                    get: PropertyValueUnion -> PropertyValue<SimpleStore, 't> option,
                    tryParse: string -> 't option,
                    ?fallback: IO<_> -> Id -> SimpleStore -> ('t * SimpleStore) option
                    ) =
    inherit PropertyMetadata<PropertyValueUnion, PropertyValue<SimpleStore,'t>>(name)
    member this.TryParse v = tryParse v
    override this.FromUnion v = get v
    override this.ToUnion v = create v
    member this.Fallback = fallback

  // gets value from the user
  let acquireValue (io:IO<_>) (name: string) (prop: Property<_>) =
    let response = io.query (sprintf "What is %s's %s?" name prop.Name)
    let rec getPropertyValue (response: string) =
      match prop.TryParse response with
      | Some v -> v
      | None ->
        getPropertyValue (io.query (sprintf "Sorry, I didn't understand '%s'.\nWhat is %s's %s?" response name prop.Name))
    getPropertyValue response

  let rec lookup io (prop: Property<'t>) id (data : SimpleStore) =
    let (|PropertyMatch|_|) =
      prop.FromUnion
    match Map.tryFind (id, prop.Name) data.mapping with
    | Some(PropertyMatch(pv)) ->
      let v = PropertyValue.computeCurrentValue data pv
      v, data
    | _ ->
      // get inherited value if possible
      match data.parentScope with
      | Some parent ->
        let v, parent = lookup io prop id parent
        v, { data with parentScope = Some parent }
      | None ->
        // get default or value from user, depending on property metadata
        let defaultVals =
          match prop.Fallback with
          | Some fallback ->
            fallback io id data
          | _ -> None
        // fallback may or may not return values
        match defaultVals with
        | Some(v, data) ->
          v, data
        | _ ->
          // when all else fails, get value from user, at whatever scoping level we currently are at
          let v = acquireValue io data.reverseRoster.[id] prop
          v, Data.set prop id Permanent (Value v) data

  let parseNumber input =
    match System.Int32.TryParse input with
    | true, v -> Some v
    | _ -> None
  let parseText input = if String.length input > 0 then Some input else None
  let NumberProperty name = Property<int>(name, Number, (function Number n -> Some n | v -> None), parseNumber)
  let NumberPropertyFB name fb = Property<int>(name, Number, (function Number n -> Some n | v -> None), parseNumber, fb)
  let TextProperty name = Property<string>(name, Text, (function Text t -> Some t | v -> None), parseText)
  let TextPropertyFB name fb = Property<string>(name, Text, (function Text t -> Some t | v -> None), parseText, fb)

  let hp = NumberProperty "HP"
  let nameList = Property<string []>("Name List", NameList, (function NameList l -> Some l | v -> None), thunk1 failwith "NameList parse should never be called because a default exists.", fun _ _ store -> Some(Array.empty, store))
  let creatureType = TextPropertyFB "CreatureType" (fun _ _ data -> Some("Unknown", data))
  let name = TextPropertyFB "Name" (fun io id data ->
                                      match lookup io nameList id data with
                                      | [||], data -> None
                                      | names, data ->
                                        Some(randomChoice names, data))
  let condition = Property<Conditions>("Condition", Condition, (function Condition t -> Some t | v -> None), thunk1 failwithf "Cannot parse conditions")
  let isRound x s = s.round = Some x
  let addCondition duration value (PropertyValue cs) =
    PropertyValue((duration, value)::cs)
  let addTemporaryCondition duration conditionTransform pv =
    addCondition (Temporary duration) (Transform conditionTransform) pv
  let addProne c = { c with Prone = true }
  let pv =
    PropertyValue.empty
    |> addCondition Permanent (Value Conditions.empty)
    |> addTemporaryCondition (isRound 2) addProne
  Data.withProps [name;nameList] |> Data.set nameList
  let mutable data = Data.withProps [hp] |> Data.set hp 1 Lasting (Value 4)
  for x in [1..5] do
    data <- Data.newRound data
    let v = (PropertyValue.computeCurrentValue data pv)
    printfn "Are we prone? %s" <| v.Prone.ToString()
