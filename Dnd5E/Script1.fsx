
#if INTERACTIVE
#I __SOURCE_DIRECTORY__
#load @"../Froggy/Common.fs"
open Froggy.Common
#else
module Froggy.Fight
open Froggy.Common
#endif
type Id = int
type PropertyName = string
type PropertyType = Text | Number | DataStructure
type PropertyValue = Text of string | Number of int | DataStructure
  with
  member this.AsNumber =
    match this with Number(n) -> n | _ -> failwithf "%A is not a number" this
  member this.AsText =
    match this with Text(t) -> t | _ -> failwithf "%A is not text" this
type Property = { Name: PropertyName; Type: PropertyType }
let Properties =
  [
  "Name", PropertyType.Text
  "HP", PropertyType.Number
  "Attacks", PropertyType.DataStructure
  ]
  |> List.map (fun (name, type1) -> name, { Name = name; Type = type1 })
  |> Map.ofList
type Data = {
  roster: Map<string, Id>
  reverseRoster: Map<Id, string>
  mapping: Map<Id*PropertyName, PropertyValue>
}
with
  static member Empty = { roster = Map.empty; reverseRoster = Map.empty; mapping = Map.empty }
type StatBank = Map<Id*PropertyName, PropertyValue>

let getValue (query: string -> string) (getName: unit -> string) (prop: Property) =
  let response = query (sprintf "What is %s's %s?" (getName()) prop.Name)
  let rec getPropertyValue (response: string) =
    match prop.Type with
    | PropertyType.Text -> response.Trim() |> Text
    | PropertyType.Number ->
      match System.Int32.TryParse response with
      | true, v -> Number v
      | _ -> requery response
    | PropertyType.DataStructure ->
      failwith "Not implemented; scenario not well-understood yet"
  and requery prevAnswer =
    getPropertyValue (query (sprintf "Sorry, I didn't understand '%s'.\nWhat is %s's %s?" prevAnswer (getName()) prop.Name))
  getPropertyValue response

let queryFromConsole x =
  printfn "%s" x
  System.Console.ReadLine()
//getValue queryFromConsole (thunk "Mort") Properties.["HP"]

let load name data =
  if data.roster.ContainsKey name then
    failwithf "%s is already loaded. Name must be unique" name
  let id' =
    if data.roster.IsEmpty then 1
    else (data.roster |> Seq.maxBy (fun kv -> kv.Value)).Value |> (+) 1
  { data with
      roster = Map.add name id' data.roster
      reverseRoster = Map.add id' name data.reverseRoster
    }

let pcs = ["Eladriel"; "Cranduin"; "Vlad"; "Jack"]
let mutable test =
  pcs
  |> List.fold (flip load) Data.Empty

type Resolve = Resolve with
  static member Execute queryIO state = ()
type ResolveBuilder() =
  member this.Bind(m, continuation: PropertyValue -> Resolve) = Resolve
  member this.For(m, continuation: PropertyValue -> Resolve) = Resolve
  member this.Zero() = Resolve
  member this.Return(m, value: int) = Resolve

let resolve = ResolveBuilder()

let lookup id propName (data : Data) =
  match Map.tryFind (id, propName) data.mapping with
  | Some(v) -> v, data
  | None ->
    let prop = Properties.[propName]
    let v = (getValue queryFromConsole) (fun _ -> data.reverseRoster.[id]) prop
    v, { data with mapping = data.mapping |> Map.add (id, prop.Name) v }

resolve {
  let pc = "Eladriel Shadowdancer"
  let! hp = lookup test.roster.[pc] "HP"
  printfn "%s's HP: %d" pc hp.AsNumber
  let pc = "Vladimir Nightbinder"
  let! hp = lookup test.roster.[pc] "HP"
  printfn "%s's HP: %d" pc hp.AsNumber
  let pc = "Nevermore Jack"
  let! hp = lookup test.roster.[pc] "HP"
  printfn "%s's HP: %d" pc hp.AsNumber
  let pc = "Cranduin the Lesser"
  let! hp = lookup test.roster.[pc] "HP"
  printfn "%s's HP: %d" pc hp.AsNumber
  return "what's this for?", hp.AsNumber
  //for pc in pcs do
  //  let! hp = lookup (test.roster.[pc]) "HP"
  //  printfn "%s's HP: %d" pc hp.AsNumber
} |> Resolve.Execute queryFromConsole (pcs |> List.fold (flip load) Data.Empty)

