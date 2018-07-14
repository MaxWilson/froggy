module Froggy.Dnd5e.ChargenTests

open Xunit

open Froggy.Packrat
open Froggy.Dnd5e.CharGen
open Froggy.Dnd5e.Data
open System

[<Fact>]
let VerifyStatBonus() =
  Assert.Equal(0, combatBonus 10)
  Assert.Equal(0, skillBonus 10)
  Assert.Equal(-1, combatBonus 9)
  Assert.Equal(0, skillBonus 9)
  Assert.Equal(0, combatBonus 11)
  Assert.Equal(+1, skillBonus 11)
  Assert.Equal(+4, combatBonus 19)
  Assert.Equal(+5, skillBonus 19)
  Assert.Equal(-4, combatBonus 2)
  Assert.Equal(-4, skillBonus 2)

[<Fact(DisplayName="Usage tests: verify that chargen commands in core scenario can be parsed correctly")>]
let UsageTest() =
  let output = ref ""
  let mutable i = 11
  let ctx = GameState((fun _ -> i <- i + 1; i), UpdateStatus = fun summary -> output := summary)
  let proc cmd =
    let cmds = ParseArgs.Init cmd |> Froggy.Dnd5e.CharGen.parse
    Assert.NotEmpty cmds
    ctx.Execute cmds

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

  proc "swap dex wis"
  Assert.Contains ("Dex 17", !output)
  Assert.Contains ("Wis 13", !output)

  proc "human int dex Sharpshooter"
  Assert.Contains("VHuman", !output)
  Assert.Contains ("Int 16", !output)
  Assert.Contains ("Dex 18", !output)
  Assert.Contains ("Sharpshooter", !output)

  proc "fighter 5; wizard 5; fighter 16; XP 14024"
  Assert.Contains ("Fighter 5", !output)
  Assert.Contains ("Wizard 1",!output)
  Assert.Contains ("XP 14024", !output)
  Assert.Contains ("HP 56", !output)
  Assert.Contains ("[Fighter 1-5; Wizard 1-5; Fighter 6-15]", !output)

  proc "note Mengar is an orphan. He likes Grommit and fears his older sister."
  proc "note Wally is his best friend"
  Assert.Contains ("Mengar is an orphan. He likes Grommit and fears his older sister.", !output)
  Assert.Contains ("Wally is his best friend", !output)
  proc "amendnote Wally was his best friend"
  Assert.Contains ("Mengar is an orphan. He likes Grommit and fears his older sister.", !output)
  Assert.DoesNotContain ("Wally is his best friend", !output)
  Assert.Contains ("Wally was his best friend", !output)
  proc "clearnotes; note Mengar fears his mother"
  Assert.DoesNotContain ("Mengar is an orphan. He likes Grommit and fears his older sister.", !output)
  Assert.DoesNotContain ("Wally is his best friend", !output)
  Assert.Contains("Mengar fears his mother", !output)

[<Fact>]
let TestSwapAttributes() =
  let output = ref ""
  let mutable i = 11
  let ctx = GameState((fun _ -> i <- i + 1; i), UpdateStatus = fun summary -> output := summary)
  let proc cmd =
    let cmds = ParseArgs.Init cmd |> Froggy.Dnd5e.CharGen.parse
    Assert.NotEmpty(cmds)
    ctx.Execute cmds
  proc "name Mengar the Magnificent"
  Assert.Contains("Name: Mengar the Magnificent", !output)
  proc "roll"
  Assert.Contains ("Str 12", !output)
  Assert.Contains ("Dex 13", !output)
  Assert.Contains ("Con 14", !output)
  Assert.Contains ("Int 15", !output)
  Assert.Contains ("Wis 16", !output)
  Assert.Contains ("Cha 17", !output)
  proc "swap dex int; swap con ch; swap w intelligence"
  Assert.Contains ("Str 12", !output)
  Assert.Contains ("Dex 15", !output)
  Assert.Contains ("Con 17", !output)
  Assert.Contains ("Int 16", !output)
  Assert.Contains ("Wis 13", !output)
  Assert.Contains ("Cha 14", !output)

[<Fact>]
let TestClassLevels() =
  let output = ref ""
  let mutable i = 11
  let ctx = GameState((fun _ -> i <- i + 1; i), UpdateStatus = fun summary -> output := summary)
  let proc cmd =
    let cmds = ParseArgs.Init cmd |> Froggy.Dnd5e.CharGen.parse
    Assert.NotEmpty(cmds)
    ctx.Execute cmds
  proc "fighter 3; wizard 2; xp 6500"
  Assert.Contains ("Fighter 3", !output)
  Assert.Contains ("Wizard 2",!output)
  proc "wizard 0"
  Assert.Contains ("Fighter 5", !output)
  Assert.DoesNotContain ("Wizard",!output, StringComparison.InvariantCultureIgnoreCase)
  proc "thief 3; wizard 2; fighter 0"
  Assert.Contains ("Wizard 2", !output)
  Assert.DoesNotContain ("Fighter",!output, StringComparison.InvariantCultureIgnoreCase)
  proc "thief 4; wizard 3"
  Assert.Contains ("Thief 4;", !output)
  proc "thief 1; wizard 3"
  Assert.Contains ("Thief 1;", !output)
  proc "Blade Pact 3; Hexblade 5"
  Assert.Contains ("Blade Pact Hexblade 1-5", !output)



[<Fact>]
let TestSaveLoad() =
  let output = ref ""
  let mutable i = 11
  let ctx = GameState((fun _ -> i <- i + 1; i), UpdateStatus = fun summary -> output := summary)
  let proc cmd =
    let cmds = ParseArgs.Init cmd |> Froggy.Dnd5e.CharGen.parse
    Assert.NotEmpty(cmds)
    ctx.Execute cmds
  proc "name Mengar the Magnificent"
  Assert.Contains("Name: Mengar the Magnificent", !output)
  proc "roll"
  Assert.Contains ("Str 12", !output)
  Assert.Contains ("Dex 13", !output)
  Assert.Contains ("Con 14", !output)
  Assert.Contains ("Int 15", !output)
  Assert.Contains ("Wis 16", !output)
  Assert.Contains ("Cha 17", !output)
  let mutable jsonFile = CharSheet.Empty
  let mutable fileName = ""
  ctx.IO <- { save = (fun fileName' data -> fileName <- fileName'; jsonFile <- data); load = (fun fileName -> if fileName = "Mary Sue" then Some ({CharSheet.Empty with Name = "Mary Sue"; Stats = { Str = 18; Dex = 18; Con = 18; Int = 18; Wis = 18; Cha = 22 }}) else None) }
  proc "save"
  Assert.Equal("Mengar", fileName)
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
  let ctx = GameState((fun _ -> i <- i + 1; i), UpdateStatus = fun summary -> output := summary)
  let proc cmd =
    let cmds = ParseArgs.Init cmd |> Froggy.Dnd5e.CharGen.parse
    Assert.NotEmpty(cmds)
    ctx.Execute cmds
  let failproc cmd =
    let cmds = ParseArgs.Init cmd |> Froggy.Dnd5e.CharGen.parse
    Assert.Empty(cmds)
  Assert.Equal("", !output)
  proc "new"
  Assert.Contains("Name: Unnamed", !output)
  proc "name     Uncanny John, eater of fish  " // deliberate extraneous whitespace and commas
  Assert.Contains("Name: Uncanny John, eater of fish", !output)
  failproc "namebob" // not a valid name, should change nothing
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
  ctx.IO <- { save = (fun _ _ -> ()); load = (fun fileName -> if fileName = "Mary Sue" then Some ({CharSheet.Empty with Name = "Mary Sue"; Stats = { Str = 18; Dex = 18; Con = 18; Int = 18; Wis = 18; Cha = 22 }}) else None) }
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
  proc "human"
  Assert.Contains("Human", !output)
  Assert.Contains ("Str 19", !output)
  Assert.Contains ("Dex 19", !output)
  Assert.Contains ("Con 19", !output)
  Assert.Contains ("Int 19", !output)
  Assert.Contains ("Wis 19", !output)
  Assert.Contains ("Cha 22", !output) // bonuses can't raise total above 20
  proc "wood elf"
  Assert.Contains("Wood elf", !output)
  Assert.Contains ("Str 18", !output)
  Assert.Contains ("Dex 20", !output)
  Assert.Contains ("Con 18", !output)
  Assert.Contains ("Int 18", !output)
  Assert.Contains ("Wis 19", !output)
  Assert.Contains ("Cha 22", !output)
  proc "human str dex HAM"
  Assert.Contains("VHuman", !output)
  Assert.Contains ("Str 20", !output) // should know that HAM comes with a built-in +1
  Assert.Contains ("Dex 19", !output)
  Assert.Contains ("HAM", !output)
