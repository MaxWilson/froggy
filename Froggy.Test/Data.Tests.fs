module Froggy.Data.Tests
open Froggy.Common
open Froggy.Properties
open Froggy.Properties.PropertyValue
open Xunit

[<Fact(DisplayName = "Verify that property scoping, transforms all work")>]
let VerifyPropertyScoping() =
  let isDivisibleBy x arg = arg % x = 0
  let noParent: Lens<int, int option, int option, int> = Lens.lens (fun _ -> None) (fun _ x -> x)
  for x in [-10..10] do
    let propertyValue = (PropertyValue [Temporary(isDivisibleBy 4), Value 3; Temporary (flip (>=) 5), Transform(flip (/) 2); Lasting, Transform((+) 2); Permanent, Value x])
    Assert.Equal((computePermanentValue x propertyValue), x)
  for x in [-10..10] do
    let propertyValue = (PropertyValue [Temporary(isDivisibleBy 4), Value 3; Temporary (flip (>=) 5), Transform(flip (/) 2); Lasting, Transform((+) 2); Permanent, Value x])
    Assert.Equal((computeCurrentValue x propertyValue), if x % 4 = 0 then 3 else (x + 2) / (if x >= 5 then 2 else 1))

[<Fact(DisplayName = "Verify that inheritance creates properties on demand")>]
let VerifyInheritanceScoping() =
  ()