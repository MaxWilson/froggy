module Froggy.Test

open Xunit

[<Fact>]
let ``Check Name``() =
  Assert.Equal("Froggy", Froggy.Library.Name)