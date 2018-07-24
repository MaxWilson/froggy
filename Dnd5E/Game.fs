module Froggy.Dnd5e.Game

open Froggy.Common
open Froggy.Dnd5e.Data
open CharGen
open CharGen.Commands
open CharGen.Prop

type GameStateWrapper(roll) =
  let mutable state = { Party.Empty with Current = Some 0; Party = [CharSheet.Empty] }
  member val UpdateStatus = (fun (str: string) -> ()) with get, set
  member val IO = { save = (fun _ _ -> failwith "Not supported"); load = (fun _ -> failwith "Not supported") } with get, set
  member this.Execute(cmds: Command list) =
    state <- update this.IO roll cmds state
    view state |> this.UpdateStatus
  member this.Execute(cmd) = this.Execute [cmd]
  new() =
    GameStateWrapper(resolve <| fun x -> 1 + random.Next(x))
