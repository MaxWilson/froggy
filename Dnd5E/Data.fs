/// Shared data structures, especially ones that get serialized to JSON
module Froggy.Dnd5e.Data

open Froggy
open Froggy.Common

type RollSpec =
  | Sum of n:int * die:int
  | SumKeepBestNofM of n:int * m:int * die:int
  | Compound of rolls: RollSpec list * bonus: int

type RollResolver = RollSpec -> int

let rec resolve (r: int -> int) = function
  | RollSpec.Sum(n, die) ->
    seq { for _ in 1..n -> r die } |> Seq.sum
  | RollSpec.SumKeepBestNofM(n, m, die) ->
    seq { for _ in 1..(max 0 n) -> r die }
    |> Seq.sortDescending
    |> Seq.take (max 0 (min m n)) // bounds check
    |> Seq.sum
  | RollSpec.Compound(rolls,  bonus) ->
    rolls |> Seq.sumBy (resolve r) |> (+) bonus

type StatId = Str | Dex | Con | Int | Wis | Cha
type ClassId = Bard | Barbarian | Cleric | Druid | Fighter | Monk | Paladin | Ranger | Rogue | Sorcerer | Warlock | Wizard
type ClassData = { Id: ClassId; StringRep: string; AverageHP: int; Subclasses: string list }
  with
  static member Table =
    [
      // ClassId, string rep, average HP
      Bard, "Bard", 5, ["Lore Bard"; "Valor Bard"]
      Barbarian, "Barbarian", 7, ["Barbearian"; "Ancestor Barbarian"; "Zealot"; "Berserker"; "Battlerager"]
      Cleric, "Cleric", 5, ["Knowledge Cleric"; "Life Cleric"; "Light Cleric"; "Nature Cleric"; "Tempest Cleric"; "Trickery Cleric"; "War Cleric"; "Death Cleric"; "Arcana Cleric"; "Forge Cleric"; "Grave Cleric"]
      Druid, "Druid", 5, ["Moon Druid"; "Shepherd Druid"; "Dream Druid"]
      Fighter, "Fighter", 6, ["Eldritch Knight"; "Battlemaster"; "Champion"; "Cavalier"; "Samurai"; "Purple Dragon Knight"; "Banneret"]
      Monk, "Monk", 5, ["Shadow Monk"; "Long Death Monk"; "Open Hand Monk"; "Elemental Monk"]
      Paladin, "Paladin", 6, ["Paladin of Devotion"; "Paladin of Ancients"; "Paladin of Vengeance"; "Paladin of Conquest"]
      Ranger, "Ranger", 6, ["Hunter"; "Beastmaster"; "Gloomstalker"]
      Rogue, "Rogue", 5, ["Thief"; "Assassin"; "Swashbuckler"; "Mastermind"; "Inquisitive"]
      Sorcerer, "Sorcerer", 4, ["Wild Mage"; "Shadow Sorcerer"; "Divine Soul"; "Dragon Sorcerer"]
      Wizard, "Wizard", 4, ["Abjuror"; "Conjuror"; "Diviner"; "Evoker"; "Enchanter"; "Illusionist"; "Necromancer"; "Transmuter"; "Bladesinger"; "War Mage"]
      Warlock, "Warlock", 5, ["Fiendlock"; "Celestialock"; "Cthulock"; "Feylock"; "Deathlock"; "Hexblade"; "Blade Pact"; "Tome Pact"; "Chain Pact"; "Bladelock"; "Tomelock"; "Chainlock"]
    ]
    |> List.map (fun (id, str, hp, subclasses) -> { Id = id; StringRep = str; AverageHP = hp; Subclasses = subclasses })

type StatArray = {
    Str: int
    Dex: int
    Con: int
    Int: int
    Wis: int
    Cha: int
  }

type StatMod = { Stat: StatId; Bonus: int }
type RaceData = { Name: string; Mods: StatMod list; Trait: string option }

type ClassLevel = { Id: ClassId; Level: int }

type CharSheet = {
    Name : string
    Stats: StatArray
    HP: int
    Race: RaceData option
    XP: int
    Levels: ClassLevel list
    IntendedLevels: ClassLevel list
    Subclasses: (ClassId * string) list
    Notes: string list
  }
  with
  static member Empty = {
    Name = "Unnamed"
    Stats =
      {
      Str = 10
      Dex = 10
      Con = 10
      Int = 10
      Wis = 10
      Cha = 10
      }
    Race = None
    HP = 1
    XP = 0
    Levels = []
    IntendedLevels = []
    Notes = []
    Subclasses = []
  }

type Party = {
    Current: int option
    Party: CharSheet list
  }
  with
  static member Empty = { Current = None; Party = [] }

type PCXP = { Level: int; XPRequired: int }
  with
  static member Table =
    [
      // level, XP required, daily XP budget, easy, medium, hard, deadly
      0, 0
      1, 0
      2, 300
      3, 900
      4, 2700
      5, 6500
      6, 14000
      7, 23000
      8, 34000
      9, 48000
      10, 64000
      11, 85000
      12, 100000
      13, 120000
      14, 140000
      15, 165000
      16, 195000
      17, 225000
      18, 265000
      19, 305000
      20, 355000
      ] |> List.map (fun (level, xp) -> { Level = level; XPRequired = xp })

let combatBonus statVal =
  (statVal/2) - 5
let skillBonus statVal =
  ((statVal+1)/2) - 5

module Grammar =
  open Froggy.Packrat
  let rec (|Rolls|_|) = pack <| function
    | SimpleRoll(lhs, OWS(Char('+', OWS(Rolls(rhs, rest))))) -> Some(lhs::rhs, rest)
    | SimpleRoll(roll, rest) -> Some([roll], rest)
    | _ -> None
  and (|SimpleRoll|_|) = pack <| function
    | Int(n, Char ('d', Int(d, Char ('k', Int(m, rest))))) -> Some (RollSpec.SumKeepBestNofM(n, m, d), rest)
    | Int(n, Char ('d', Int(d, rest))) -> Some (RollSpec.Sum(n, d), rest)
    | Int(n, Char ('d', rest)) -> Some (RollSpec.Sum(n, 6), rest)
    | Char ('d', Int(d, rest)) -> Some (RollSpec.Sum(1, d), rest)
    | _ -> None
  let (|Roll|_|) = pack <| function
    | Rolls(lhs, OWS(Char('+', OWS(Int(rhs, rest))))) -> Some (RollSpec.Compound(lhs, rhs), rest)
    | Rolls(lhs, OWS(Char('-', OWS(Int(rhs, rest))))) -> Some (RollSpec.Compound(lhs, -rhs), rest)
    | Rolls([v], rest) -> Some(v, rest)
    | Rolls(rolls, rest) -> Some(RollSpec.Compound(rolls, 0), rest)
    | _ -> None

type MonsterTemplate = { TypeName : string; Namelist: string[]; HP: RollSpec }

module Properties =

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

module AdventureData =
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

  // gets value from the user
  let acquireValue (query: string -> string) (name: string) (prop: Property) =
    let response = query (sprintf "What is %s's %s?" name prop.Name)
    let rec getPropertyValue (response: string) =
      match prop.TryParse response with
      | Some v -> v
      | None ->
        getPropertyValue (query (sprintf "Sorry, I didn't understand '%s'.\nWhat is %s's %s?" response name prop.Name))
    getPropertyValue response

  let lookup query (prop: Property) id (data : Data) =
    match Map.tryFind (id, prop.Name) data.mapping with
    | Some(v) -> v, data
    | None ->
      let v = acquireValue query data.reverseRoster.[id] prop
      v, { data with mapping = data.mapping |> Map.add (id, prop.Name) v }

  type Command =
    | Set of Property * PropertyValue * Id
    | Increment of NumberProperty * int * Id
    | Deduct of NumberProperty * int * Id

type GameState = { party: Party; adventure: AdventureData.Data option }

