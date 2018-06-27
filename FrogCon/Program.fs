// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System
open Froggy.Packrat
open Froggy.Dnd5e.CharGen

[<EntryPoint>]
let main argv =
  let st = new StatBank(UpdateStatus = printfn "%s\n")
  st.Execute(Commands.RollStats)
  let rec commandLoop previousCommands =
    printf "> "
    match ParseContext.Init <| Console.ReadLine() with
    | Word (AnyCase("q" | "quit"), End) -> 0
    | End -> // on ENTER, repeat
      previousCommands |> List.iter st.Execute
      commandLoop previousCommands
    | v -> match parse v with
           | [] ->
              printfn "Sorry, come again? (Type 'quit' to quit)"
              commandLoop previousCommands
           | cmds ->
              cmds |> List.iter st.Execute
              commandLoop cmds
  commandLoop []