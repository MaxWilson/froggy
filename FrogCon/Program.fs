// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.

open System
open Froggy.Packrat
open Froggy.CharGen
open System.IO
open Froggy.Data
open Froggy
open Common

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
    | Transform(roll, t) ->
      sprintf "(%s) -> %d" (render (result.sublog.Head)) result.value
    | Branch((_,mods),_) ->
      let b,m,v = match result.sublog with [b;m;v] -> b,m,v | v -> failwithf "No match for %A" v
      let test = match mods with StaticValue 0 -> render b | _ -> (sprintf "%s+%s" (render b) (render m))
      sprintf "(%s) -> %s" test (render v)
    | _ ->
      result.value.ToString()

module IO =
  open Newtonsoft.Json

  let jsonConverter = Fable.JsonConverter() :> JsonConverter

  // Serialization
  let toJson value = JsonConvert.SerializeObject(value, Formatting.Indented, [|jsonConverter|])

  let ofJson<'t> json =
    // Deserialization
    JsonConvert.DeserializeObject<'t>(json, [|jsonConverter|])


[<EntryPoint>]
let main argv =

  let roll = Roll.eval >> Roll.Result.getValue
  let queryFromConsole x =
    printfn "%s" x
    System.Console.ReadLine()
  let io =
    { new IO() with
        override this.save fileNameNoExtension data =
          use file = File.OpenWrite (fileNameNoExtension + ".txt")
          use writer = new StreamWriter(file)
          writer.WriteLine(IO.toJson data)
        override this.load<'t> fileNameNoExtension =
          try
            let json = File.ReadAllText (fileNameNoExtension + ".txt")
            IO.ofJson<'t>(json) |> Some
            //BackwardCompatible.deserialize<'t>(json) |> Some
          with
            exn -> None
        override this.query v = queryFromConsole v
        override this.output v = printfn "%s" v
           }

  let exec cmd (state: GameState) =
    Game.update io roll cmd state

  let initialState =
    { GameState.Empty with monsterTemplates = ref <| Some (Froggy.Data.Data.empty) }
    |> exec (Game.Commands.LoadMonsterTemplate "monsters")
    |> exec (Game.Commands.CharGenCommands [CharGen.Commands.Command.RollStats])

  let rec commandLoop (previousCommands: ParseInput option) state =
    printf "> "
    let execute commandString =
      match commandString with
      | CharGen.Grammar.Commands(cmds, End) ->
        commandLoop (Some commandString) (exec (Game.Commands.CharGenCommands cmds) state)
      | Roll.Grammar.Roll(roll, End) ->
        Roll.eval roll |> Roll.render |> printfn "%s"
        commandLoop (Some commandString) state
      | Roll.Grammar.Aggregate(rolls, End) ->
        let results = rolls |> Roll.evaluateAggregate Froggy.Common.rollOneDie
        for result in results.value do
          result |> Roll.render |> printfn "%s"
        commandLoop (Some commandString) state
      | Froggy.Packrat.Str "avg." (Roll.Grammar.Roll(roll, End))
      | Froggy.Packrat.Word(AnyCase("avg" | "average"), (Roll.Grammar.Roll(roll, End))) ->
        Roll.mean roll |> printfn "%.4f"
        commandLoop (Some commandString) state
      | _ ->
        printfn "Sorry, come again? (Type 'quit' to quit)"
        commandLoop previousCommands state
    match ParseArgs.Init <| Console.ReadLine() with
    | Word (AnyCase("q" | "quit"), End) ->
      // before exiting, save templates
      state |> exec (Game.Commands.SaveMonsterTemplate "monsters") |> ignore
      0
    | End when previousCommands.IsSome -> // on ENTER, repeat
      execute previousCommands.Value
    | v ->
      execute v
  commandLoop None initialState