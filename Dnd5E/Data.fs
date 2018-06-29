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
type ClassId = Fighter | Wizard

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

type StatBlock = {
    Name : string
    Stats: StatArray
    HP: int
    Race: RaceData option
    XP: int
    Levels: (ClassId * int) list
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
  }

type State = {
    Current: int option
    Party: StatBlock list
  }
  with
  static member Empty = { Current = None; Party = [] }

