module Froggy.Data.Tests
open Froggy.Common
open Froggy.Properties
open Xunit

[<Fact(DisplayName = "Verify that property scoping, transforms, computed values all work")>]
let VerifyPropertyScoping() =
  let isDivisibleBy x arg = arg % x = 0
  let propertyValue = (PropertyValue [Temporary(isDivisibleBy 4), Value 3; Temporary (flip (>=) 5), Transform(flip (/) 2); Lasting, Transform((+) 2); Permanent, Computed id])
  for x in [-10..10] do
    Assert.Equal((computePermanentValue x propertyValue), x)
  for x in [-10..10] do
    Assert.Equal((computeCurrentValue x propertyValue), if x % 4 = 0 then 3 else (x + 2) / (if x >= 5 then 2 else 1))

[<Fact(DisplayName = "Verify that inheritance creates properties on demand")>]
let VerifyInheritanceScoping() =
  ()