module App.View

open Elmish
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open Fulma.FontAwesome
open Froggy.Data
open Froggy.Packrat

type Model =
    { Input : string
      Output : string
    }

type Msg =
    | ChangeInput of string
    | ComputeOutput

let init _ = { Input = ""; Output = "" }, Cmd.none

module RollHelper =
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

  let execute commandString =
    match ParseArgs.Init commandString with
    | Roll.Grammar.Roll(roll, End) ->
      Roll.eval roll |> render
    | Roll.Grammar.Aggregate(rolls, End) ->
      let results = rolls |> Roll.evaluateAggregate Froggy.Common.rollOneDie
      [for result in results.value ->
        result |> render]
      |> String.join ";"
    | Froggy.Packrat.Str "avg." (Roll.Grammar.Roll(roll, End))
    | Froggy.Packrat.Word(AnyCase("avg" | "average"), (Roll.Grammar.Roll(roll, End))) ->
      Roll.mean roll |> sprintf "%.4f"
    | _ ->
      "Sorry, come again?"

let private update msg model =
    match msg with
    | ChangeInput newValue ->
        { model with Input = newValue }, Cmd.none
    | ComputeOutput ->

        { model with Output = RollHelper.execute model.Input }, Cmd.none

module Pixi =
  open Fable.Core.JsInterop
  open Fable.Core
  open Fable.Import.React

  [<Import("RotatingBunny", "./RotatingBunny.tsx")>]
  type IRotatingBunny =
    abstract name : unit -> string
  [<Import("BunnyStage", "./RotatingBunny.tsx")>]
  let BunnyStage(txt:string) : ReactElement = jsNative
  let RotatingBunny : JsConstructor<IRotatingBunny> = importDefault "./RotatingBunny.tsx"

  type BunnyStage1 =
    inherit StatelessComponent<int>

let prf = PixiReact.``react-pixi-fiber``
let private view model dispatch =
    Hero.hero [ Hero.IsFullHeight ]
        [ Hero.body [ ]
            [ Container.container [ ]
                [ Columns.columns [ Columns.CustomClass "has-text-centered" ]
                    [ Column.column [ Column.Width(Screen.All, Column.IsThreeFifths)
                                      Column.Offset(Screen.All, Column.IsOneFifth) ]
                        [ Image.image [ Image.Is128x128
                                        Image.Props [ Style [ Margin "auto"] ] ]
                            [ img [ Src "assets/fulma_logo.svg" ] ]
                          (Pixi.BunnyStage(model.Output))
                          prf.Stage.Create()
                          Image.image [ Image.Is128x128
                                        Image.Props [ Style [ Margin "auto"] ] ]
                            [ img [ Src "assets/fulma_logo.svg" ] ]
                          Field.div [ ]
                            [ Label.label [ ]
                                [ str "Enter a die roll" ]
                              Control.div [ ]
                                [ Input.text [ Input.OnChange (fun ev -> dispatch (ChangeInput ev.Value))
                                               Input.Value model.Input
                                               Input.Props [ AutoFocus true; OnKeyDown (fun ev -> if (ev.key = "Enter") then dispatch ComputeOutput) ] ] ] ]
                          Content.content [ ]
                            [ str model.Output ] ] ] ] ] ]

open Elmish.React
open Elmish.Debug
open Elmish.HMR

Program.mkProgram init update view
#if DEBUG
|> Program.withHMR
#endif
|> Program.withReactUnoptimized "elmish-app"
#if DEBUG
|> Program.withDebugger
#endif
|> Program.run
