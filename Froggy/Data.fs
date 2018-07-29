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
}

let empty = { roster = Map.empty; reverseRoster = Map.empty; mapping = Map.empty; properties = Map.empty; parentScope = None }
let getParent<'mt, 't> : RecursiveOptionLens<Data<'mt, 't>> = Lens.lens (fun x -> x.parentScope) (fun v x -> { x with parentScope = v })

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
