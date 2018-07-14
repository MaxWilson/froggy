module Froggy.Dnd5e.Adventure
open Froggy.Common

module Properties =
  open Data

  type PropertyName = string
  type PropertyValue = Text of string | Number of int
  let asNumber = function Number n -> n | v -> failwithf "Invalid cast: %A is not a number" v
  let asText = function Text n -> n | v -> failwithf "Invalid cast: %A is not text" v

  [<AbstractClass>]
  type Property(name : PropertyName, origin : (CharSheet -> PropertyValue) option, output : (PropertyValue -> CharSheet -> CharSheet) option, fromTemplate: (MonsterTemplate -> PropertyValue) option) =
    member this.Name = name
    member this.Origin = origin
    member this.Output = output
    member this.FromTemplate = fromTemplate
    abstract member TryParse: string -> PropertyValue option
  type NumberProperty(name, ?origin : (CharSheet -> PropertyValue), ?output : (PropertyValue -> CharSheet -> CharSheet), ?fromTemplate: (MonsterTemplate -> PropertyValue)) =
    inherit Property(name, origin, output, fromTemplate)
    override this.TryParse input =
      match System.Int32.TryParse input with
      | true, v -> Some <| Number v
      | _ -> None
  type TextProperty(name, ?origin : (CharSheet -> PropertyValue), ?output : (PropertyValue -> CharSheet -> CharSheet), ?fromTemplate: (MonsterTemplate -> PropertyValue)) =
    inherit Property(name, origin, output, fromTemplate)
    override this.TryParse input = Some <| Text input

  type Property<'t> =
    {
      Name: PropertyName;
      Lens: Lens<PropertyValue, PropertyValue, 't, 't>;
      Origin: (CharSheet -> PropertyValue) option
      Output: (PropertyValue -> CharSheet -> CharSheet) option
      }
    with
    static member New(name, lens) = { Name = name; Lens = lens; Origin = None; Output = None }
  let Name = TextProperty("Name", origin = fun sb -> Text sb.Name)
  let HP = NumberProperty("HP", origin = (fun sb -> Number sb.HP), fromTemplate = (fun t -> resolve (random.Next >> (+) 1) >> Number <| t.HP ))
  let SP = NumberProperty("SP", origin = fun sb -> Number sb.HP)
  let XP = NumberProperty("XP", origin = (fun sb -> Number sb.XP), output = fun pv sb -> { sb with XP = asNumber pv })
  let Properties =
    ([ Name; HP; SP; XP ] : Property list)
    |> List.map (fun t -> t.Name, t)
    |> Map.ofList

open Properties

module FightData =
  open Properties
  open Data

  type Id = int
  type StatBank = Map<Id*PropertyName, PropertyValue>
  type Data = {
    roster: Map<string, Id>
    reverseRoster: Map<Id, string>
    mapping: StatBank
    properties: Map<string, Property>
  }
  with
    static member Empty = { roster = Map.empty; reverseRoster = Map.empty; mapping = Map.empty; properties = Properties }

  type Command =
    | Set of Property * PropertyValue * Id
    | Increment of NumberProperty * int * Id
    | Deduct of NumberProperty * int * Id

  // gets value from the user
  let acquireValue (query: string -> string) (name: string) (prop: Property) =
    let response = query (sprintf "What is %s's %s?" name prop.Name)
    let rec getPropertyValue (response: string) =
      match prop.TryParse response with
      | Some v -> v
      | None ->
        getPropertyValue (query (sprintf "Sorry, I didn't understand '%s'.\nWhat is %s's %s?" response name prop.Name))
    getPropertyValue response

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

  let queryFromConsole x =
    printfn "%s" x
    System.Console.ReadLine()

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

  let resolve = DeferredInputBuilder()

  let lookup query (prop: Property) id (data : Data) =
    match Map.tryFind (id, prop.Name) data.mapping with
    | Some(v) -> v, data
    | None ->
      let v = acquireValue query data.reverseRoster.[id] prop
      v, { data with mapping = data.mapping |> Map.add (id, prop.Name) v }

open Data
open FightData

let Init(pcs: CharSheet list) =
  pcs |> List.fold load Data.Empty

let executeOne query cmd data =
  match cmd with
  | Set(prop, v, id) ->
    { data with mapping = data.mapping |> Map.add (id, prop.Name) v }
  | Increment(prop, v, id) ->
    let currentValue, data = lookup query prop id data // may force a query
    { data with mapping = data.mapping |> Map.add (id, prop.Name) (asNumber currentValue + v |> Number) }
  | Deduct(prop, v, id) ->
    let currentValue, data = lookup query prop id data // may force a query
    { data with mapping = data.mapping |> Map.add (id, prop.Name) (asNumber currentValue - v |> Number) }
let execute query cmds data =
  cmds |> List.fold (flip (executeOne query)) data
let longRest data =
  let inline fullHeal (mapping: StatBank) id : StatBank =
    mapping |> Map.add (id, HP.Name) (Number <| if (id:Id) = 1 then 33 else 40) // lazy programmer
  { data with mapping = data.reverseRoster |> Map.fold (thunk2 fullHeal) data.mapping }

module Fight =
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
  let run query fight =
    let mordred = 1 // lazy programmer
    let sam = 2
    if (lookup query XP sam fight |> fst |> asNumber) < 100 then
      fight
        |> execute query
          [
            Deduct(HP, 3, mordred)
            Deduct(HP, 8, sam)
            Increment(XP, 100, mordred)
            Increment(XP, 100, sam)
          ]
    else
      fight
        |> execute query
          [
            Deduct(HP, 20, mordred)
            Deduct(HP, 12, sam)
            Increment(XP, 100, mordred)
            Increment(XP, 100, sam)
          ]

  let update fight adventure = fight