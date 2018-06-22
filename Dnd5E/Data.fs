module Froggy.Dnd5e.Data

type RollSpec =
  | Sum of n:int * die:int
  | SumBestNofM of n:int * m:int * die:int
  | Compound of rolls: RollSpec list * bonus: int

type RollResolver = RollSpec -> int

let rec resolve (r: int -> int) = function
  | RollSpec.Sum(n, die) ->
    seq { for _ in 1..n -> r die } |> Seq.sum
  | RollSpec.SumBestNofM(n, m, die) ->
    seq { for _ in 1..n -> r die } |> Seq.sortDescending |> Seq.take m |> Seq.sum
  | RollSpec.Compound(rolls,  bonus) ->
    rolls |> Seq.sumBy (resolve r) |> (+) bonus
