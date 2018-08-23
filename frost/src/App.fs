module App.View

open Elmish
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open Fulma.FontAwesome
open Froggy.Data
open Froggy.Packrat
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import.React

type Model =
    { Input : string
      Output : string
      Frogs: (int * int * int * float) list
    }

type Msg =
    | ChangeInput of string
    | ComputeOutput

let init _ = { Input = ""; Output = ""; Frogs = [] }, Cmd.none

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
    | Transform(roll, t) ->
      sprintf "(%s) -> %d" (render (result.sublog.Head)) result.value
    | Branch((_,mods),_) ->
      let b,m,v = match result.sublog with [b;m;v] -> b,m,v | v -> failwithf "No match for %A" v
      let test = match mods with StaticValue 0 -> render b | _ -> (sprintf "%s+%s" (render b) (render m))
      sprintf "(%s) -> %s" test (render v)
    | _ ->
      result.value.ToString()

  let execute commandString =
    match ParseArgs.Init commandString with
    | Roll.Grammar.Roll(roll, End) ->
      let result = Roll.eval roll
      Some result.value, result |> render
    | Roll.Grammar.Aggregate(rolls, End) ->
      let results = rolls |> Roll.evaluateAggregate Froggy.Common.rollOneDie
      let explain =
        [
        for result in results.value ->
          result |> render]
        |> String.join ";"
      results.value |> List.sumBy(fun r -> r.value) |> Some, explain
    | Froggy.Packrat.Str "avg." (Roll.Grammar.Roll(roll, End))
    | Froggy.Packrat.Word(AnyCase("avg" | "average"), (Roll.Grammar.Roll(roll, End))) ->
      None, Roll.mean roll |> sprintf "%.4f"
    | _ ->
      None, "Sorry, come again?"

let private update msg model =
    match msg with
    | ChangeInput newValue ->
        { model with Input = newValue }, Cmd.none
    | ComputeOutput ->
        match RollHelper.execute model.Input with
        | None, msg ->
          { model with Output = msg }, Cmd.none
        | Some(qty), msg ->
          let frogs = [for i in 1..qty -> i, ((50 * i) + Froggy.Common.random.Next(40)) % 780, ((20 * i) + Froggy.Common.random.Next(40)) % 480, (0.25 + Froggy.Common.random.NextDouble() * 1.75)]
          { model with Output = msg; Frogs = frogs }, Cmd.none


//module Pixi =
//  open Fable.Core.JsInterop
//  open Fable.Core
//  open Fable.Import.React

//  [<Import("RotatingBunny", "./RotatingBunny.tsx")>]
//  type IRotatingBunny =
//    abstract name : unit -> string
//  [<Import("BunnyStage", "./RotatingBunny.tsx")>]
//  let BunnyStage(txt:string) : ReactElement = jsNative
//  let RotatingBunny : JsConstructor<IRotatingBunny> = importDefault "./RotatingBunny.tsx"

//  type BunnyStage1 =
//    inherit StatelessComponent<int>

module rpf =
    open Fable.Import

    [<Pojo>]
    type ApplicationOptions = { backgroundColor: string }

    [<Pojo>]
    /// `Stage` component properties."
    type StageProperties =
      {
        width: int
        height: int
        options: ApplicationOptions
      }

    type Point = {
      x: int
      y: int
    }

    type TextStyle = {
      fill: string
    }

    [<Import("Texture", "pixi.js")>]
    type Texture =
      static member fromImage(imageUrl: string, ?crossOrigin: bool) : Texture = jsNative

    [<Pojo>]
    type SpriteProperties = {
      position: Point
      height: int
      width: int
      alpha: float
      texture: Texture
    }

    [<Pojo>]
    type TextProperties = {
      position: Point
      height: int
      width: int
      alpha: float
      text: string
      style: TextStyle
    }

    let stage (props: StageProperties) children : ReactElement = ofImport "Stage" "react-pixi-fiber" props children
    let text (props: TextProperties) children = ofImport "Text" "react-pixi-fiber" props children
    let sprite (props: SpriteProperties) children = ofImport "Sprite" "react-pixi-fiber" props children
open rpf
//let bunnyStage : unit -> Fable.Import.React.ReactElement = import "BunnyStage" "./RotatingBunny.tsx"
//let rb : JsConstructor<Fable.Import.React.ReactElement> = import "RotatingBunny" "./RotatingBunny.tsx"

let frog = rpf.Texture.fromImage("assets/frog-icon.png")

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
                          stage { createEmpty<StageProperties> with width = 800; height = 500; options = { backgroundColor = "0x10bb99" } } [
                            for i in 1..10 do
                              yield text { createEmpty<TextProperties> with text = "Hello world"; position = { x = 40+(i*4); y = 70+(i*15) }; alpha = (1.0 - (0.1 * float i)) } []
                            for (i,x,y,size) in model.Frogs do
                              yield sprite { createEmpty<SpriteProperties> with texture = frog; position = { x = x; y = y }; } []
                              yield text { createEmpty<TextProperties> with text = sprintf "#%d" i; position = { x = x - 10; y = y - 10; }; alpha = 0.90; style = { fill = "white" } } []
                            ]
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
