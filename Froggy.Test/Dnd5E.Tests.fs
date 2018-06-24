module Froggy.Dnd5e.Tests

open Xunit

open Froggy.Packrat
open Froggy.Dnd5e.CharGen

[<Fact(DisplayName="Usage tests: verify that chargen commands can be parsed correctly")>]
let UsageTest() =
  let ctx = StatBank()
  let output = ref ""
  ctx.UpdateStatus <- fun summary -> output := summary
  let proc cmd = ParseContext.Init cmd |> Froggy.Dnd5e.CharGen.parse |> ctx.Execute
  Assert.Equal("", !output)
  proc "new"
  Assert.Equal("Name: Unnamed", !output)
  proc "name Mengar the Magnificent"
  Assert.Contains("Name: Mengar the Magnificent", !output)
  proc "name     Uncanny John, eater of fish  " // deliberate extraneous whitespace and commas
  Assert.Contains("Name: Uncanny John, eater of fish", !output)
  proc "namebob" // not a valid name, should change nothing
  Assert.Contains("Name: Uncanny John, eater of fish", !output)

