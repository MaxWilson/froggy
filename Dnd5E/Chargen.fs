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

type State = { Name : string }
  with static member Empty = { Name = "Unnamed" }

let update state = function
  | NewContext -> State.Empty
  | SetName v -> { state with Name = v }
  | _ -> state

let view state =
  sprintf "Name: %s" state.Name

type StatBank() =
  let mutable state = State.Empty
  member val UpdateStatus = (fun (str: string) -> ()) with get, set
  member this.Execute(cmd: Command) =
    state <- (update state cmd)
    view state |> this.UpdateStatus
