module Froggy.Game
open Froggy
open Froggy.Common
open Froggy.CharGen
open Froggy.Data

module Commands =
  type Command =
    | CharGenCommands of CharGen.Commands.Command list
    | LoadMonsterTemplate of filename: string
    | SaveMonsterTemplate of filename: string

module Grammar =
  open Froggy.Packrat
  open Commands
  let rec (|Commands|_|) = pack <| function
    | CharGen.Grammar.Commands(cmds, rest) -> Some(CharGenCommands cmds, rest)
    | _ -> None

open Commands
let update (io:IO) roll cmd state =
  match cmd with
  | CharGenCommands cmds ->
    let state = { state with party = CharGen.update io roll cmds state.party }
    view state.party |> io.output
    state
  | SaveMonsterTemplate filename ->
    match !state.monsterTemplates with
    | Some(templates) ->
      io.save filename templates
    | None -> ()
    state
  | LoadMonsterTemplate filename ->
    match io.load<Froggy.Data.SimpleProperties.SimpleStore> filename with
    | Some templates ->
      { state with monsterTemplates = ref (Some templates) }
    | None -> state
