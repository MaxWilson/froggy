// Encounter generation tools and data
module Froggy.Encounter

open Froggy.Common
open Froggy.Data

let orcNames = [|"Gronk"; "Spud"; "Thud"; "Narmsh"; "Bargle"; "Quonk"; "Varak"; "Skorn"|]
let templates = [
  { TypeName = "Orc"; Namelist = orcNames; HP = Roll.StaticValue 15 }
]

let encounter : (string * int) list -> MonsterTemplate list =
  let map =
    templates
    |> List.map (fun t -> t.TypeName.ToLowerInvariant(), t) // ignore case
    |> Map.ofList
  let lookup (name : string, count) =
    match Map.tryFind (name.ToLowerInvariant()) map with
    | None -> failwithf "No such monster: '%s'" name
    | Some t -> List.init count (thunk t)
  fun spec ->
    spec |> List.collect lookup
