module Froggy.Dnd5e.Game

open Froggy.Common
open Froggy.Dnd5e.Data
open CharGen
open CharGen.Commands
open CharGen.Prop

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
