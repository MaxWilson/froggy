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
    | Noop
open Commands

let parse commandString =
  match ParseContext.Init commandString with
  | Str "new" End | Str "begin" End -> NewContext
  | Str "name" (Words (name, End)) -> SetName name
  | _ -> Noop

type StatBank() =
  member val UpdateStatus = (fun (str: string) -> ()) with get, set
  member this.Execute(cmd: Command) = ()

