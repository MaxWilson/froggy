module Froggy.Dnd5e.Tests

open Xunit

open Froggy.Packrat
open Froggy.Dnd5e.CharGen

[<Fact(DisplayName="Usage tests: verify that chargen commands can be parsed correctly")>]
let UsageTest() =
  let output = ref ""
  let mutable i = 11
  let ctx = StatBank((fun _ -> i <- i + 1; i), UpdateStatus = fun summary -> output := summary)
  let proc cmd = ParseContext.Init cmd |> Froggy.Dnd5e.CharGen.parse |> List.iter ctx.Execute
  proc "name Mengar the Magnificent"
  Assert.Contains("Name: Mengar the Magnificent", !output)
  proc "roll"
  Assert.Contains ("Str 12", !output)
  Assert.Contains ("Dex 13", !output)
  Assert.Contains ("Con 14", !output)
  Assert.Contains ("Int 15", !output)
  Assert.Contains ("Wis 16", !output)
  Assert.Contains ("Cha 17", !output)
  proc "assign 6 5 2 3 1 4"
  Assert.Contains ("Str 12", !output)
  Assert.Contains ("Dex 13", !output)
  Assert.Contains ("Con 16", !output)
  Assert.Contains ("Int 15", !output)
  Assert.Contains ("Wis 17", !output)
  Assert.Contains ("Cha 14", !output)
  proc "swap dex int; swap con ch; swap w intelligence"
  Assert.Contains ("Str 12", !output)
  Assert.Contains ("Dex 15", !output)
  Assert.Contains ("Con 14", !output)
  Assert.Contains ("Int 17", !output)
  Assert.Contains ("Wis 13", !output)
  Assert.Contains ("Cha 16", !output)
  let mutable jsonFile = StatBlock.Empty
  ctx.IO <- { save = (fun _ data -> jsonFile <- data); load = (fun fileName -> if fileName = "Mary Sue" then Some ({StatBlock.Empty with Name = "Mary Sue"; Stats = { Str = 18; Dex = 18; Con = 18; Int = 18; Wis = 18; Cha = 22 }}) else None) }
  proc "save"
  Assert.Equal("Mengar the Magnificent", jsonFile.Name)
  Assert.Equal(12, jsonFile.Stats.Str)
  proc "load Mary Sue"
  Assert.Contains("Name: Mary Sue", !output)
  Assert.Contains ("Str 18", !output)
  Assert.Contains ("Dex 18", !output)
  Assert.Contains ("Con 18", !output)
  Assert.Contains ("Int 18", !output)
  Assert.Contains ("Wis 18", !output)
  Assert.Contains ("Cha 22", !output)


[<Fact(DisplayName="Usage tests: verify corner cases for parse commands")>]
let CornerCasees() =
  let output = ref ""
  let mutable i = 11
  let ctx = StatBank((fun _ -> i <- i + 1; i), UpdateStatus = fun summary -> output := summary)
  let proc cmd = ParseContext.Init cmd |> Froggy.Dnd5e.CharGen.parse |> List.iter ctx.Execute
  let failproc cmd =
    let cmds = ParseContext.Init cmd |> Froggy.Dnd5e.CharGen.parse
    Assert.Empty(cmds)
  Assert.Equal("", !output)
  proc "new"
  Assert.Contains("Name: Unnamed", !output)
  proc "name     Uncanny John, eater of fish  " // deliberate extraneous whitespace and commas
  Assert.Contains("Name: Uncanny John, eater of fish", !output)
  proc "namebob" // not a valid name, should change nothing
  Assert.Contains("Name: Uncanny John, eater of fish", !output)
  proc "roll"
  failproc "assign 6 4 2 3 1" // invalid assignment (wrong number of stats) should change nothing except potentially outputting a parse error
  Assert.Contains ("Str 12", !output)
  Assert.Contains ("Dex 13", !output)
  Assert.Contains ("Con 14", !output)
  Assert.Contains ("Int 15", !output)
  Assert.Contains ("Wis 16", !output)
  Assert.Contains ("Cha 17", !output)
  proc "assign 1 4 4 2 3 5" // ties should be allowed as meaning "don't care", and broken arbitrarily
  Assert.Contains ("Str 17", !output)
  Assert.True (output.Value.Contains("Dex 13")||output.Value.Contains("Con 13"))
  Assert.True (output.Value.Contains("Dex 14")||output.Value.Contains("Con 14"))
  Assert.Contains ("Int 16", !output)
  Assert.Contains ("Wis 15", !output)
  Assert.Contains ("Cha 12", !output)
  i <- 11
  proc "name Bob; assign 2 2 2 1 2 2"
  Assert.Contains("Name: Bob\n", !output)
  Assert.Contains("Int 17", !output)
  ctx.IO <- { save = (fun _ _ -> ()); load = (fun fileName -> if fileName = "Mary Sue" then Some ({StatBlock.Empty with Name = "Mary Sue"; Stats = { Str = 18; Dex = 18; Con = 18; Int = 18; Wis = 18; Cha = 22 }}) else None) }
  Assert.Contains("Name: Bob", !output)
  proc "load Hedwig"
  Assert.Contains("Name: Bob", !output)
  proc "load Mary Sue"
  Assert.Contains("Name: Mary Sue", !output)
  Assert.Contains ("Str 18", !output)
  Assert.Contains ("Dex 18", !output)
  Assert.Contains ("Con 18", !output)
  Assert.Contains ("Int 18", !output)
  Assert.Contains ("Wis 18", !output)
  Assert.Contains ("Cha 22", !output)
