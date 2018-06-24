module Froggy.Common

let flip f x y = f y x
let betweenInclusive bound1 bound2 x = (min bound1 bound2) <= x && x <= (max bound1 bound2)
