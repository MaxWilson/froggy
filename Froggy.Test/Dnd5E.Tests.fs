module Froggy.Dnd5e.Tests

open Xunit

open Froggy.Packrat
open Froggy.Dnd5e.CharGen

[<Fact(DisplayName="Usage tests: verify that chargen commands can be parsed correctly")>]
let UsageTest() =
  let output = ref ""
  let mutable i = 11
  let ctx = StatBank((fun _ -> i <- i + 1; i), UpdateStatus = fun summary -> output := summary)
  let proc cmd = ParseContext.Init cmd |> Froggy.Dnd5e.CharGen.parse |> ctx.Execute
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


[<Fact(DisplayName="Usage tests: verify corner cases for parse commands")>]
let CornerCasees() =
  let output = ref ""
  let mutable i = 11
  let ctx = StatBank((fun _ -> i <- i + 1; i), UpdateStatus = fun summary -> output := summary)
  let proc cmd = ParseContext.Init cmd |> Froggy.Dnd5e.CharGen.parse |> ctx.Execute
  Assert.Equal("", !output)
  proc "new"
  Assert.Contains("Name: Unnamed", !output)
  proc "name     Uncanny John, eater of fish  " // deliberate extraneous whitespace and commas
  Assert.Contains("Name: Uncanny John, eater of fish", !output)
  proc "namebob" // not a valid name, should change nothing
  Assert.Contains("Name: Uncanny John, eater of fish", !output)
  proc "roll"
  proc "assign 6 4 2 3 1 4" // invalid assignment (duplicates) should change nothing
  Assert.Contains ("Str 12", !output)
  Assert.Contains ("Dex 13", !output)
  Assert.Contains ("Con 14", !output)
  Assert.Contains ("Int 15", !output)
  Assert.Contains ("Wis 16", !output)
  Assert.Contains ("Cha 17", !output)
