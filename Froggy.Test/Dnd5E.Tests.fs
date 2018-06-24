module Froggy.Dnd5e.Tests

open Xunit

open Froggy.Dnd5e.CharGen

[<Fact>]
let ``Usage tests: verify that chargen commands can be parsed correctly``() =
  let ctx = StatBank()
  let output = ref ""
  ctx.UpdateStatus <- fun summary -> output := summary
  let proc cmd = Froggy.Dnd5e.CharGen.parse cmd |> ctx.Execute
  proc "new"
  Assert.Equal("", !output)
  proc "name Mengar the Magnificent"
  Assert.Contains("Name: Mengar the Magnificent", !output)
