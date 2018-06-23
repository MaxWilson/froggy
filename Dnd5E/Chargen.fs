module Froggy.Dnd5e.CharGen
open Froggy.Packrat

module Commands =
  type Command =
    | NewContext
    | SetName of string
    | RollStats
    | AssignStats of int list
    | Save of string
    | Load of string
open Commands

type StatBank() =
  member val UpdateStatus = (fun (str: string) -> ()) with get, set
  member this.Execute(cmd: Command) = ()

