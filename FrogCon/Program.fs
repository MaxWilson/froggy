// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System
open Froggy.Packrat
open Froggy.Dnd5e.CharGen
open System.IO
open Froggy.Dnd5e
open Froggy.Dnd5e.Data
open Microsoft.FSharpLu.Json
open Froggy
open Common

[<EntryPoint>]
let main argv =
  let save characterName (data: CharSheet) =
    use file = File.OpenWrite (characterName + ".txt")
    use writer = new StreamWriter(file)
    writer.WriteLine(Compact.serialize data)
  let load characterName =
    try
      let json = File.ReadAllText (characterName + ".txt")
      BackwardCompatible.deserialize<CharSheet>(json) |> Some
    with
      exn -> None
  let io = { save = save; load = load }
  let roll = resolve <| (random.Next >> (+) 1)
  let exec cmds (state: GameState) =
    match cmds with
    | Game.Commands.CharGenCommands cmds ->
      let state = { state with party = CharGen.update io roll cmds state.party }
      view state.party |> printfn "%s\n"
      state
    | Game.Commands.AdventureCommands cmds ->
      let adventure =
        state.adventure
        |> Option.defaultWith
          (fun _ ->
            Adventure.Init state.party.Party)
      let state = Adventure.update io roll cmds state
      view state |> printfn "%s\n"
      state

  let initialState = GameState.Empty |> exec (Game.Commands.CharGenCommands [CharGen.Commands.Command.RollStats])

  let rec commandLoop (previousCommands: ParseInput option) =
    printf "> "
    let execute commandString =
      match commandString with
      | Game.Grammar.Commands(cmds, End) ->
        st.Execute cmds
        commandLoop (Some commandString)
      | Data.Grammar.Roll(roll, End) ->
        resolve roll |> printfn "%d"
        commandLoop (Some commandString)
      | _ ->
        printfn "Sorry, come again? (Type 'quit' to quit)"
        commandLoop previousCommands
    match ParseArgs.Init <| Console.ReadLine() with
    | Word (AnyCase("q" | "quit"), End) -> 0
    | End when previousCommands.IsSome -> // on ENTER, repeat
      execute previousCommands.Value
    | v ->
      execute v
  commandLoop None