module Froggy.Adventure
open Froggy.Common
open Froggy.Data
open Froggy.Data.Properties
open Froggy.Data.AdventureData

module AdventureData =
  let load data (statBlock:CharSheet) =
    let name = statBlock.Name
    if data.roster.ContainsKey name then
      failwithf "%s is already loaded. Name must be unique" name
    let id' =
      if data.roster.IsEmpty then 1
      else (data.roster |> Seq.maxBy (fun kv -> kv.Value)).Value |> (+) 1
    { data with
        roster = Map.add name id' data.roster
        reverseRoster = Map.add id' name data.reverseRoster
        mapping =
          let mapProperty mapping _ (prop:Property) =
            match prop.Origin with
            | Some(f) ->
                mapping |> Map.add (id', prop.Name) (f statBlock)
            | None -> mapping
          data.properties |> Map.fold mapProperty data.mapping
      }

  type DeferredInput<'t> =
    | Result of 't
    | Query of continuation:((string -> string) -> Data -> Data * DeferredInput<'t>)
    with
    static member Execute queryIO state = function
      | Result(t) -> t, state
      | Query(continuation) ->
        let state, next = continuation queryIO state
        DeferredInput<_>.Execute queryIO state next

  type DeferredInputBuilder() =
    let resolve (id:Id, propName:PropertyName) continuation queryIO (data: Data) =
      let v, data =
        match Map.tryFind (id, propName) data.mapping with
        | Some(v) -> v, data
        | None ->
          let prop = Properties.[propName]
          let v = (acquireValue queryIO) data.reverseRoster.[id] prop
          v, { data with mapping = data.mapping |> Map.add (id, prop.Name) v }
      data, continuation v
    member this.Bind((id:Id, propName:PropertyName), continuation: PropertyValue -> DeferredInput<_>) =
      Query(resolve (id, propName) continuation)
    member this.Bind((name:string, propName:PropertyName), continuation: PropertyValue -> DeferredInput<_>) =
      let resolve queryIO (data: Data) =
        let id = data.roster.[name]
        resolve (id, propName) continuation queryIO data
      Query(resolve)
    //member this.For(m, continuation: PropertyValue -> Resolve) = Placeholder
    member this.Zero() = Result ()
    member this.Return(value) =
      Result value

  let adventure = DeferredInputBuilder()

open AdventureData

let Init(pcs: CharSheet list) =
  pcs |> List.fold load Data.Empty

module Grammar =
  open Froggy.Packrat
  let (|Data|_|) = ExternalContextOf<Data>
  let (|NumberProperty|_|) = function
    | Word(word, rest) & Data(d) ->
      match d.properties |> Map.tryFind word with
      | Some(:? NumberProperty as prop) -> Some(prop, rest)
      | _ -> None
    | _ -> None
  let (|TextProperty|_|) = function
    | Word(word, rest) & Data(d) ->
      match d.properties |> Map.tryFind word with
      | Some(:? TextProperty as prop) -> Some(prop, rest)
      | _ -> None
    | _ -> None
  let (|Character|_|) = function
    | Word(word, rest) & Data(d) ->
      match d.roster |> Map.tryFind word with
      | Some(id) -> Some(id, rest)
      | _ -> None
    | _ -> None

  let (|Command|_|) =
    let commandSeparator = Microsoft.FSharp.Collections.Set<_>[';']
    function
    | Str "set" (Character(id, NumberProperty(propName, Int(v, rest)))) -> Some(Command.Set(HP, Number v, id), rest)
    | Str "set" (Character(id, TextProperty(propName, CharsExcept commandSeparator (v, rest)))) -> Some(Command.Set(HP, Text v, id), rest)
    | _ -> None

  let rec (|Commands|_|) = pack <| function
    | Commands(cmds, Str ";" (OWS(Command(c, rest)))) -> Some(cmds @ [c], rest)
    | Command(c, rest) -> Some([c], rest)
    | _ -> None

module Execution =
  open AdventureData
  open Froggy.Data
  open Froggy.Data.AdventureData

  let executeOne (io: IO<_>) cmd data =
    match cmd with
    | Set(prop, v, id) ->
      { data with mapping = data.mapping |> Map.add (id, prop.Name) v }
    | Increment(prop, v, id) ->
      let currentValue, data = lookup io.query prop id data // may force a query
      { data with mapping = data.mapping |> Map.add (id, prop.Name) (asNumber currentValue + v |> Number) }
    | Deduct(prop, v, id) ->
      let currentValue, data = lookup io.query prop id data // may force a query
      { data with mapping = data.mapping |> Map.add (id, prop.Name) (asNumber currentValue - v |> Number) }
  let update io roll cmds state =
    cmds |> List.fold (flip (executeOne io)) state
let update = Execution.update

let longRest data =
  let inline fullHeal (mapping: StatBank) id : StatBank =
    mapping |> Map.add (id, HP.Name) (Number <| if (id:Id) = 1 then 33 else 40) // lazy programmer
  { data with mapping = data.reverseRoster |> Map.fold (thunk2 fullHeal) data.mapping }

module Fight =
  open Execution
  open Froggy.Common
  let r = System.Random()
  let ofAdventure (encounter: MonsterTemplate list) adventure =
    let add data (template: MonsterTemplate) =
      let id' =
        if data.roster.IsEmpty then 1
        else (data.roster |> Seq.maxBy (fun kv -> kv.Value)).Value |> (+) 1
      let name =
        let rec choose count =
          let name = sprintf "%s the %s" (randomChoice template.Namelist) template.TypeName
          if not <| data.roster.ContainsKey name then
            name
          elif count < template.Namelist.Length * 2 then // make limited number of attempts to find a valid name
            choose (count + 1)
          else
            sprintf "%s the %s #%d" (randomChoice template.Namelist) template.TypeName id'
        choose 0
      { data with
          roster = Map.add name id' data.roster
          reverseRoster = Map.add id' name data.reverseRoster
          mapping =
            let mapProperty mapping _ (prop:Property) =
              match prop.FromTemplate with
              | Some(f) ->
                  mapping |> Map.add (id', prop.Name) (f template)
              | None -> mapping
            data.properties |> Map.fold mapProperty data.mapping
        }
    encounter |> List.fold add adventure
  let run (io: Froggy.Data.IO<_>) fight =
    let mordred = 1 // lazy programmer
    let sam = 2
    let roll = Roll.eval
    if (lookup io.query XP sam fight |> fst |> asNumber) < 100 then
      fight
        |> update io roll
          [
            Deduct(HP, 3, mordred)
            Deduct(HP, 8, sam)
            Increment(XP, 100, mordred)
            Increment(XP, 100, sam)
          ]
    else
      fight
        |> update io roll
          [
            Deduct(HP, 20, mordred)
            Deduct(HP, 12, sam)
            Increment(XP, 100, mordred)
            Increment(XP, 100, sam)
          ]

  let update fight adventure = fight
