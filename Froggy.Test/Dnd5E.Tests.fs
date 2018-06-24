module Froggy.Dnd5e.Tests

open Xunit

open Froggy.Packrat
open Froggy.Dnd5e.CharGen

[<Fact(DisplayName="Usage tests: verify that chargen commands can be parsed correctly")>]
let UsageTest() =
  let output = ref ""
  let ctx = StatBank((fun _ -> 12), UpdateStatus = fun summary -> output := summary)
  let proc cmd = ParseContext.Init cmd |> Froggy.Dnd5e.CharGen.parse |> ctx.Execute
  proc "name Mengar the Magnificent"
  Assert.Contains("Name: Mengar the Magnificent", !output)
  proc "roll"
  Assert.Contains ("Str 12", !output)
  Assert.Contains ("Dex 12", !output)
  Assert.Contains ("Con 12", !output)
  Assert.Contains ("Int 12", !output)
  Assert.Contains ("Wis 12", !output)
  Assert.Contains ("Cha 12", !output)

[<Fact(DisplayName="Usage tests: verify corner cases for parse commands")>]
let CornerCasees() =
  let ctx = StatBank()
  let output = ref ""
  ctx.UpdateStatus <- fun summary -> output := summary
  let proc cmd = ParseContext.Init cmd |> Froggy.Dnd5e.CharGen.parse |> ctx.Execute
  Assert.Equal("", !output)
  proc "new"
  Assert.Equal("Name: Unnamed", !output)
  proc "name     Uncanny John, eater of fish  " // deliberate extraneous whitespace and commas
  Assert.Contains("Name: Uncanny John, eater of fish", !output)
  proc "namebob" // not a valid name, should change nothing
  Assert.Contains("Name: Uncanny John, eater of fish", !output)
