module Froggy.CommonTests
open Froggy.Common
open Xunit

[<Theory>]
[<InlineData(null, null)>]
[<InlineData("Magister Arconi", "Magister")>]
[<InlineData("   xxx yy zz ", "xxx")>]
let VerifyFirstWord(input : string, expectedOutput: string) =
  Assert.Equal(expectedOutput, String.firstWord input)