// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System
open Froggy.Packrat
open Froggy.CharGen
open System.IO
open Froggy.Data
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
  let roll = resolve <| (random.Next >> (+) 1)
  let queryFromConsole x =
    printfn "%s" x
    System.Console.ReadLine()
  let io = { save = save; load = load; query = queryFromConsole; output = printfn "%s" }

  let exec cmd (state: GameState) =
    Game.update io roll cmd state

  let initialState = GameState.Empty |> exec (Game.Commands.CharGenCommands [CharGen.Commands.Command.RollStats])

  let rec commandLoop (previousCommands: ParseInput option) state =
    printf "> "
    let execute commandString =
      match commandString with
      | Game.Grammar.Commands(cmds, End) ->
        commandLoop (Some commandString) (exec cmds state)
      | Data.Grammar.Roll(r, End) ->
        roll r |> printfn "%d"
        commandLoop (Some commandString) state
      | _ ->
        printfn "Sorry, come again? (Type 'quit' to quit)"
        commandLoop previousCommands state
    match ParseArgs.Init <| Console.ReadLine() with
    | Word (AnyCase("q" | "quit"), End) -> 0
    | End when previousCommands.IsSome -> // on ENTER, repeat
      execute previousCommands.Value
    | v ->
      execute v
  commandLoop None initialState