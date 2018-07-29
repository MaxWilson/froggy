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
type StatBank<'propertyValueType> = Map<Id*PropertyName, 'propertyValueType>
type Data<'propertyMetadataType, 'propertyValueType> = {
  roster: Map<string, Id>
  reverseRoster: Map<Id, string>
  mapping: StatBank<'propertyValueType>
  properties: Map<string, 'propertyMetadataType>
  parentScope: Data<'propertyMetadataType, 'propertyValueType> option
  round: int option
}
type PropertyMetadata<'store, 'union, 't> = { Name: PropertyName; Create: 't -> PropertyValueComponent<'store, 'union>; Get: 'union -> 't option }
module PropertyMetadata =
  let create name createFunction getFunction =
    { Name = name; Create = createFunction; Get = getFunction }
module Data =
  let empty = { roster = Map.empty; reverseRoster = Map.empty; mapping = Map.empty; properties = Map.empty; parentScope = None; round = None }
  let getParent<'mt, 't> : RecursiveOptionLens<Data<'mt, 't>> = Lens.lens (fun x -> x.parentScope) (fun v x -> { x with parentScope = v })
  let newRound d = { d with round = Some ((defaultArg d.round 0) + 1) }
  let set prop (id: Id) scope v d =
    let currentValue =
      match d.mapping |> Map.tryFind (id, prop.Name) with
      | Some pv -> pv
      | None -> PropertyValue.empty
    let pv' = PropertyValue.add scope v currentValue
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

  type PropertyValueUnion = Condition of Conditions | Number of int
  let addTemporaryCondition duration addCondition (PropertyValue cs) =
    PropertyValue(((Temporary duration), (Transform addCondition))::cs)
  let addProne c = { c with Prone = true }
  let isRound x s = s.round = Some x
  let pv = PropertyValue.empty |> addTemporaryCondition (isRound 1) addProne
  let hp = PropertyMetadata.create "HP" id (function Number n -> Some n | v -> None)
  let data = Data.empty |> Data.set hp 1 Lasting (Value (Number 4))
  let v = (PropertyValue.computeCurrentValue Data.getParent data pv)
  printf "Are we prone? %A" v

  type PropertyValueUnion = Text of string | Number of int | Conditions of Conditions
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
