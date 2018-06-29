// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System
open Froggy.Packrat
open Froggy.Dnd5e.CharGen
open System.IO
open Newtonsoft.Json
open Froggy.Dnd5e

[<EntryPoint>]
let main argv =
  let save characterName (data:  StatBlock) =
    use file = File.OpenWrite (characterName + ".txt")
    use writer = new StreamWriter(file)
    writer.WriteLine(JsonConvert.SerializeObject data)
  let load characterName =
    try
      let json = File.ReadAllText (characterName + ".txt")
      JsonConvert.DeserializeObject<StatBlock>(json) |> Some
    with
      exn -> None
  let st = new StatBank(IO = { save = save; load = load }, UpdateStatus = printfn "%s\n")
  st.Execute(Commands.RollStats)
  let rec commandLoop previousCommands =
    printf "> "
    match ParseContext.Init <| Console.ReadLine() with
    | Word (AnyCase("q" | "quit"), End) -> 0
    | End -> // on ENTER, repeat
      st.Execute (previousCommands: _ list)
      commandLoop previousCommands
    | v -> match parse v with
           | [] ->
              printfn "Sorry, come again? (Type 'quit' to quit)"
              commandLoop previousCommands
           | cmds ->
              st.Execute cmds
              commandLoop cmds
  commandLoop []