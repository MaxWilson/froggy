// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System
open Froggy.Packrat
open Froggy.Dnd5e.CharGen
open System.IO
open Froggy.Dnd5e
open Froggy.Dnd5e.Data
open Microsoft.FSharpLu.Json
open Newtonsoft.Json
open Froggy.Dnd5e.Game

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
  let st = new GameStateWrapper(IO = { save = save; load = load }, UpdateStatus = printfn "%s\n")
  let resolve =
    let r = new Random()
    resolve (r.Next >> (+) 1)
  st.Execute(Commands.RollStats)

  let rec commandLoop (previousCommands: ParseInput option) =
    printf "> "
    let execute commandString =
      match commandString with
      | CharGen.Grammar.Commands(cmds, End) ->
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