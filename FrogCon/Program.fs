// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System
open Froggy.Packrat
open Froggy.Dnd5e.CharGen
open System.IO
open Newtonsoft.Json
open Froggy.Dnd5e
open Froggy.Dnd5e.Data

module Roll =
  open Roll
  open Froggy.Common
  let rec render (result: Result) =
    match result.source with
    | Combine(Sum, (Aggregate(_) | Repeat(_))) ->
      sprintf "[%s] => %d" (String.join ", " (result.sublog |> List.map render)) result.value
    | Combine(Max, (Aggregate(_) | Repeat(_))) ->
      sprintf "max(%s) => %d" (String.join ", " (result.sublog |> List.map render)) result.value
    | Combine(Min, (Aggregate(_) | Repeat(_))) ->
      sprintf "min(%s) => %d" (String.join ", " (result.sublog |> List.map render)) result.value
    | Branch((_,mods),_) ->
      let b,m,v = match result.sublog with [b;m;v] -> b,m,v | v -> failwithf "No match for %A" v
      let test = match mods with StaticValue 0 -> render b | _ -> (sprintf "%s+%s" (render b) (render m))
      sprintf "(%s) -> %d" test result.value
    | _ ->
      result.value.ToString()


[<EntryPoint>]
let main argv =
  let save characterName (data: CharSheet) =
    use file = File.OpenWrite (characterName + ".txt")
    use writer = new StreamWriter(file)
    writer.WriteLine(JsonConvert.SerializeObject data)
  let load characterName =
    try
      let json = File.ReadAllText (characterName + ".txt")
      JsonConvert.DeserializeObject<CharSheet>(json) |> Some
    with
      exn -> None
  let st = new GameState(IO = { save = save; load = load }, UpdateStatus = printfn "%s\n")
  st.Execute(Commands.RollStats)

  let rec commandLoop (previousCommands: ParseInput option) =
    printf "> "
    let execute commandString =
      match commandString with
      | CharGen.Grammar.Commands(cmds, End) ->
        st.Execute cmds
        commandLoop (Some commandString)
      | Roll.Grammar.Roll(roll, End) ->
        Roll.eval roll |> Roll.render |> printfn "%s"
        commandLoop (Some commandString)
      | Roll.Grammar.Aggregate(rolls, End) ->
        let results = rolls |> Roll.evaluateAggregate Froggy.Common.rollOneDie
        for result in results.value do
          result |> Roll.render |> printfn "%s"
        commandLoop (Some commandString)
      | Froggy.Packrat.Str "avg." (Roll.Grammar.Roll(roll, End))
      | Froggy.Packrat.Word(AnyCase("avg" | "average"), (Roll.Grammar.Roll(roll, End))) ->
        Roll.mean roll |> printfn "%f"
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
    | _ ->
      commandLoop previousCommands
  commandLoop None