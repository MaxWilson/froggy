// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System
open Froggy.Packrat
open Froggy.Dnd5e.CharGen

[<EntryPoint>]
let main argv =
  let st = new StatBank(UpdateStatus = printfn "%s\n")
  let rec commandLoop previousCommand =
    printf "> "
    match ParseContext.Init <| Console.ReadLine() with
    | Word (AnyCase("q" | "quit"), End) -> 0
    | End -> // on ENTER, repeat
      st.Execute previousCommand
      commandLoop previousCommand
    | v -> match parse v with
           | Commands.Noop ->
              printfn "Sorry, come again? (Type 'quit' to quit)"
              commandLoop previousCommand
           | cmd ->
              st.Execute cmd
              commandLoop cmd
  commandLoop Commands.Noop