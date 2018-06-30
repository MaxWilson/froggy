module Froggy.Dnd5e.Data

type RollSpec =
  | Sum of n:int * die:int
  | SumBestNofM of n:int * m:int * die:int
  | Compound of rolls: RollSpec list * bonus: int

type RollResolver = RollSpec -> int

let rec resolve (r: int -> int) = function
  | RollSpec.Sum(n, die) ->
    seq { for _ in 1..n -> r die } |> Seq.sum
  | RollSpec.SumBestNofM(n, m, die) ->
    seq { for _ in 1..n -> r die } |> Seq.sortDescending |> Seq.take m |> Seq.sum
  | RollSpec.Compound(rolls,  bonus) ->
    rolls |> Seq.sumBy (resolve r) |> (+) bonus

type StatId = Str | Dex | Con | Int | Wis | Cha
type ClassId = Fighter | Wizard | Thief

let classData = [
  // ClassId, string rep, average HP
  Fighter, "Fighter", 6
  Wizard, "Wizard", 4
  Thief, "Thief", 5
  ]

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

type ClassLevel = { Class: ClassId; Level: int }

type StatBlock = {
    Name : string
    Stats: StatArray
    HP: int
    Race: RaceData option
    XP: int
    Levels: ClassLevel list
    IntendedLevels: ClassLevel list
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
  }

type State = {
    Current: int option
    Party: StatBlock list
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
