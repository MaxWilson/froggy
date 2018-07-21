module Froggy.Game
open Froggy
open Froggy.Common
open Froggy.CharGen
open Froggy.Data

module Commands =
  type Command =
    | CharGenCommands of CharGen.Commands.Command list
    | AdventureCommands of Data.AdventureData.Command list

module Grammar =
  open Froggy.Packrat
  open Commands
  let rec (|Commands|_|) = pack <| function
    | CharGen.Grammar.Commands(cmds, rest) -> Some(CharGenCommands cmds, rest)
    | Adventure.Grammar.Commands(cmds, rest) -> Some(AdventureCommands cmds, rest)
    | _ -> None

open Commands
let update io roll cmd state =
  match cmd with
  | CharGenCommands cmds ->
    let state = { state with party = CharGen.update io roll cmds state.party }
    view state.party |> io.output
    state
  | AdventureCommands cmds ->
    let adventure =
      state.adventure
      |> Option.defaultWith
        (fun _ ->
          Adventure.Init state.party.Party)
    { state with adventure = Some <| Adventure.update io roll cmds adventure }