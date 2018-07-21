module Froggy.Dnd5e.FightTests

open Xunit
open Froggy.Packrat
open Froggy.Dnd5e.Data
open Froggy.Common
open Froggy.Dnd5e
open Froggy.Dnd5e.Data.AdventureData
open Froggy.Dnd5e.Data.Properties
open Froggy.Dnd5e.Encounter
open Froggy.Dnd5e.Adventure

let pcs =
  [
  {
    CharSheet.Name = "Mordred"
    Stats = { Str = 12; Dex = 12; Con = 14; Int = 13; Wis = 6; Cha = 18 }
    HP = 33
    Race = None
    XP = 100
    Levels = [{ Id = Fighter; Level = 1 }; { Id = Warlock; Level = 4 }]
    IntendedLevels = [ { Id = Fighter; Level = 1 }; { Id = Warlock; Level = 10 } ]
    Subclasses = []
    Notes = []
  }
  {
    CharSheet.Name = "Samwise"
    Stats = { Str = 17; Dex = 14; Con = 14; Int = 10; Wis = 12; Cha = 9 }
    HP = 40
    Race = None
    XP = 0
    Levels = [{ Id = Fighter; Level = 5 }]
    IntendedLevels = [ { Id = Fighter; Level = 5 }]
    Subclasses = []
    Notes = ["Samwise is very loyal to Mordred"; "Lawful good"]
  }
  ]

let DieInputs =
  [
    "4d6", RollSpec.Sum(4,6)
    "d12-4", RollSpec.Compound([RollSpec.Sum(1,12)], -4)
    "3d+2", RollSpec.Compound([RollSpec.Sum(3,6)], +2)
    "4d6k3+2", RollSpec.Compound([RollSpec.SumKeepBestNofM(4,3,6)], +2)
    "4d+  d12 + d6 +2d4", RollSpec.Compound([RollSpec.Sum(4,6);RollSpec.Sum(1,12);RollSpec.Sum(1,6);RollSpec.Sum(2,4)], 0)
    "4d+d100+ 3d-4", RollSpec.Compound([RollSpec.Sum(4,6);RollSpec.Sum(1,100);RollSpec.Sum(3,6)], -4)
  ]
  |> List.map (fun (x,y) -> [|box x; box y|])
  |> Array.ofList
[<Theory>]
[<MemberData("DieInputs")>]
let DieParsingTest(txt: string, expected: RollSpec) =
  match ParseArgs.Init txt with
  | Data.Grammar.Roll(roll, End) ->
    Assert.Equal(expected, roll)
  | _ -> failwithf "Could not parse '%s'" txt

[<Fact>]
let UsageTest() =
  (*
  1. Set up an adventure instance from stat blocks
  2. Set up a fight from adventure instance + encounter parameters (2 orcs)
  3. Run the fight
  4. Check that the PCs' HP in the new adventure instance have been depleted and XP incremented
  5. Set up another fight from adventure instance + encounter parameters (2 more orcs)
  6. Check that HP are lower than before
  7. Run the fight
  8. Rest
  9. Check that HP have reset to full but XP is still incremented
  *)
  let adv = Adventure.Init pcs
  let mordred, sam = pcs |> List.map (fun pc -> adv.roster.[pc.Name]) |> function [a;b] -> a,b | _ -> failwith "match failure!"
  let fight = adv |> Fight.ofAdventure (encounter ["orc", 2])
  let enemyId fight = fight.roster |> Seq.pick (function KeyValue(name, id) when id <> mordred && id <> sam -> Some id | _ -> None)
  let query = (failwithf "Lookup of %s not implemented")
  let lookup (property: Property) id data =
    lookup query property id data |> fst |> asNumber
  Assert.Equal(15, (lookup HP (enemyId fight) fight))
  let adv =
    fight
    |> Fight.run query
    |> flip Fight.update adv
  Assert.Equal(30, (lookup HP mordred adv))
  Assert.Equal(32, (lookup HP sam adv))
  Assert.Equal(200, (lookup XP mordred adv))
  Assert.Equal(100, (lookup XP sam adv))
  let fight = adv |> Fight.ofAdventure (encounter ["orc", 2])
  let adv = Fight.run query fight |> flip Fight.update adv
  Assert.Equal(10, (lookup HP mordred adv))
  Assert.Equal(20, (lookup HP sam adv))
  Assert.Equal(300, (lookup XP mordred adv))
  Assert.Equal(200, (lookup XP sam adv))
  let adv = adv |> Adventure.longRest
  Assert.Equal(33, (lookup HP mordred adv))
  Assert.Equal(40, (lookup HP sam adv))
  Assert.Equal(300, (lookup XP mordred adv))
  Assert.Equal(200, (lookup XP sam adv))
  ()