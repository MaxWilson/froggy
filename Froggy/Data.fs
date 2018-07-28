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
let getParent = Lens.lens (fun x -> x.parent) (fun v x -> { x with parent = v })

open Properties.SimpleProperties
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
