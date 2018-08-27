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

type Icon = Frog | Bear | Wolf | Arbitrary of string * string
type Model =
    { Input : string
      LastCommand: string
      Output : string
      Frogs: (int * int * int * float) list
      CurrentIcon: Icon
    }

type Msg =
    | ChangeInput of string
    | ComputeOutput
    | ChangeIcon of Icon

let init _ = { Input = ""; LastCommand  = ""; Output = ""; Frogs = []; CurrentIcon = Frog }, Cmd.none

module RollHelper =
  open Roll
  open Froggy.Common
  open Froggy.Packrat
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
    | Str "avg." (Roll.Grammar.Roll(roll, End))
    | Word(AnyCase("avg" | "average"), (Roll.Grammar.Roll(roll, End))) ->
      None, Roll.mean roll |> sprintf "%.4f"
    | _ ->
      None, "Sorry, come again?"

let private update msg model =
  match msg with
  | ChangeInput newValue ->
    { model with Input = newValue }, Cmd.none
  | ComputeOutput ->
    match model.Input.Trim(), model.LastCommand with
    | "", cmd
    | cmd, _ ->
      match ParseArgs.Init cmd with
      | Str "icon" (Word(tag, Any (url, End))) ->
        let cmd =
          match tag.ToLowerInvariant() with
            | "frog" -> Frog
            | "bear" -> Bear
            | "wolf" -> Wolf
            | _ -> Arbitrary(tag, url.Trim())
          |> ChangeIcon
        { model with Input = ""; Output = sprintf "Set icon to %s" tag }, Cmd.ofMsg cmd
      | _ ->
        match RollHelper.execute cmd with
        | None, msg ->
          { model with Output = msg }, Cmd.none
        | Some(qty), msg ->
          let frogs = [for i in 1..qty -> i, ((50 * i) + Froggy.Common.random.Next(40)) % 780, ((20 * i) + Froggy.Common.random.Next(40)) % 480, (0.25 + Froggy.Common.random.NextDouble() * 1.75)]
          { model with Input = ""; LastCommand = cmd; Output = msg; Frogs = frogs }, Cmd.none
  | ChangeIcon newIcon ->
    { model with CurrentIcon = newIcon }, Cmd.none


module rpf =
  open Fable.Import

  [<Pojo>]
  type ApplicationOptions = { backgroundColor: int }

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
  [<AbstractClass>]
  type Texture =
    abstract member valid: bool with get, set
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
open Fable.Import

let frog = rpf.Texture.fromImage("assets/frog-icon.png")
let bear = rpf.Texture.fromImage("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAHsAAAB7CAMAAABjGQ9NAAAAY1BMVEX///8AAAD8/Pzd3d3y8vLBwcE1NTV+fn7T09Ph4eEjIyMaGhrm5ubMzMw4ODj5+fkREREtLS2rq6tlZWVFRUUoKCjs7Ox4eHhKSkpvb29SUlK0tLRYWFidnZ2QkJAKCgqGhobuInimAAAD2ElEQVRoge2aabeqIBSGVTA1M+cph/z/v/JqNgDJducJW3ct3i+n0xIegT2wIcPQ0tLS0tLS0tL630Tt06/QhWmaDr2/xs5v0Y9s8xxOb1Ee/ZLuiLbqiW2mlmHH04dhR3bY3tjmkMx/fbIf+z7ul5r92Eb3Mza1LwLb2cvagrP5pnO/C12c74dKWzXZGiTo2+AJDUNaeIQQT8E8AOhJWZpWh9unS+99l0xbGM2rssPvob3sE/S0Cs23pr6LPkSPirvi7+CiOXxOvik6XNJy6LvGDka5rjta4qmgyPWgblduJS/IP+aHrGrT2ukDasFoMXp/VbmTQJlIJXpSdC5dmVXkquGjDk6wyJZF0W+rd9/Zpz0GPilqgzfrT3Zij8qSX836pDwR3K7fEW62gtMFxz3pDg8Pu08TyV9UCSZP7XJDMtmo6N3di6YX94mqtLT3DYuk3WXxJfs/L1nYq35d0iLTVZrdbsrkKd5TTofKHaKYDgx8lK3W6RcSGyNaqmSvlTqNwoAzrLANog7er7EN4qtid6tsI1DFxpwprFSIm7W8f+Tl/ZBtxGrYsH8rnfQIxXaVsHHndqcvloqfso1UBfuIYyvZwR9xRzZKvAzJtlQEdSTbUJFLsWwV9SLS1pTkE+y5fKigSEffCYD1ynVTio+wbNDDo1PR1R8nnCsqnhsrIT2aLLYgTZ3lvDPC84Fle9c19k0WCZok6fqh75OkCQiYCLBsWgGd+DJPtcCTadTeYZIDdJJL6zqwtEGzIWOLpWzwJAfNtoFOztKTbNA90OwC6KSSnuELsdjnrAZ//QOwW2lF2fAP1ty74NmAr9bSRsJK2VyYwN83Ak7mSBsJIenEjeDtgFMqIIXLizo+JKUW53SIeuwuwF3kndBIeI5dhPU69CHWTCL+QEJuNCFnJaM7eMwB2rCJnVPuFAzY/LDsbPzfYrbb5SZ2yjsP0Io9J7zZFrN0cvcQxQb0wCiYdB3j2HPkZSy/RbOZWR5zR8hkKLmLcex5mNbriwpJpuyU1/y6ATGC3WPe0/VrwjIcOuHsdVy44jUNwF6X9ajLHHjpqydorZb7GBVyA5JPHddsjmLsj0iOGLSw/RjnysrETpeasY38+TvWP64YdsGfaSdcXj5Kk/fCsLm9to9hh/zu3OUKcnlAZdnZPcWzNcaAYXOx/DJF0NfUQY7CGOjDHpmXznH39rQ+x3keH7I6IbO5Po5/WqgD+wl/2oT7TC5X9JbJsCh/l+86h2ueNvDlOhmm2BKz17Dkvn7ln34YZ1krt/rzQ6H4GHVtmyCaamlpaWlpaWlp/Vr/AGrtNhOIbghJAAAAAElFTkSuQmCC")
let wolf = rpf.Texture.fromImage("https://png.icons8.com/ios/1600/wolf.png", true)
let getIcon, getIconList =
  let mutable arbitraries = Map.empty
  let getIcon tag =
    match tag with
      | Frog -> frog | Bear -> bear | Wolf -> wolf
      | Arbitrary(tag, url) ->
        match arbitraries |> Map.tryFind tag with
        | Some(_, texture) -> texture
        | None ->
          let texture = rpf.Texture.fromImage(url, true)
          arbitraries <- arbitraries |> Map.add tag (url, texture)
          texture
  let getIconList() =
    [
      yield "Frog", Frog;
      yield "Bear", Bear;
      yield "Wolf", Wolf;
      for KeyValue(tag, (url, texture)) in arbitraries do
        if(texture.valid) then
          yield tag, Arbitrary(tag, url)
      ]
  getIcon, getIconList

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
module rs =
  [<Pojo>]
  type SelectRow<'T> = {
    value: 'T
    label: string
    }
  [<Pojo>]
  type ReactSelectProperties<'T> = {
    options: SelectRow<'T>[]
    onChange: SelectRow<'T> -> unit
    }
  let select (props: ReactSelectProperties<'T>) = ofImport "default" "react-select" props []
  let selectOfList inputs onChange =
    let lst = inputs |> List.map (fun (label, value) -> { value = value; label = label })
    select { options = lst |> Array.ofList; onChange = onChange }

//let bunnyStage : unit -> Fable.Import.React.ReactElement = import "BunnyStage" "./RotatingBunny.tsx"
//let rb : JsConstructor<Fable.Import.React.ReactElement> = import "RotatingBunny" "./RotatingBunny.tsx"

[<Pojo>]
type FastInputProps = {
  onChange: string -> unit
  options: Input.Option list
  }
[<Pojo>]
type FastInputState = {
  currentValue: string
  }
[<AbstractClass>]
type FastInput(props) as this =
  inherit React.Component<FastInputProps, FastInputState>(props)
  let reactProps, inputOptions =
    props.options |> List.partition(function Input.Props(lst) -> true | _ -> false)
  let reactProps =
    match reactProps with
    | Input.Props(lst)::_ -> lst
    | _ -> []
  let handler = Input.OnChange(fun ev -> this.setState { currentValue = ev.Value })
  let onKeyDown = reactProps |> List.tryPick(fun x -> if x:? DOMAttr then (match x :?> DOMAttr with OnKeyDown(f) -> Some f | _ -> None) else None)
                  |> Option.defaultValue (fun _ -> ())
  let onKeyDown = OnKeyDown (fun ev ->
                              if (ev.key = "Enter") then
                                props.onChange this.state.currentValue
                              onKeyDown ev)
  let onBlur = OnBlur (fun _ ->
    props.onChange this.state.currentValue)
  do
    this.state <- { currentValue = "" }
  override this.render() =
    let props' = Input.Props(reactProps @ [Value this.state.currentValue; onBlur; onKeyDown])
    Input.text (props'::handler::inputOptions)

let fastInput onChange props = ofType<FastInput, _, _> { onChange = onChange; options = props } []

let private view model dispatch =
    Hero.hero [ Hero.IsFullHeight ]
        [ Hero.body [ ]
            [ Container.container [ ]
                [ Columns.columns [ ]
                    [
                      Column.column [ Column.Width(Screen.All, Column.IsOneFifth) ] []
                      Column.column [ Column.CustomClass "has-text-centered"; Column.Width(Screen.All, Column.IsThreeFifths)]
                        [ Image.image [ Image.Is128x128
                                        Image.Props [ Style [ Margin "auto"] ] ]
                            [ img [ Src "assets/fulma_logo.svg" ] ]
                          stage { createEmpty<StageProperties> with width = 800; height = 500; options = { backgroundColor = 0x10bb99 } } [
                            for i in 1..10 do
                              yield text { createEmpty<TextProperties> with text = "Hello world"; position = { x = 40+(i*4); y = 70+(i*15) }; alpha = (1.0 - (0.1 * float i)) } []
                            for (i,x,y,size) in model.Frogs do
                              yield sprite { createEmpty<SpriteProperties> with height=50;width=50; texture = getIcon model.CurrentIcon; position = { x = x; y = y }; } []
                              yield text { createEmpty<TextProperties> with text = sprintf "#%d" i; position = { x = x - 10; y = y - 10; }; alpha = 0.90; style = { fill = "white" } } []
                            ]
                          Image.image [ Image.Is128x128
                                        Image.Props [ Style [ Margin "auto"] ] ]
                            [ img [ Src "assets/fulma_logo.svg" ] ]
                          Field.div [ ]
                            [ Label.label [ ]
                                [ str "Enter a die roll" ]
                              Control.div [ ]
                                [ fastInput (fun input -> dispatch (ChangeInput input)) [
                                    Input.Value model.Input
                                    Input.Props [ AutoFocus true; OnKeyDown(fun ev -> if (ev.key = "Enter") then dispatch ComputeOutput) ] ] ] ]
                          Content.content [ ]
                            [ Text.span [Modifiers [Modifier.TextWeight TextWeight.Bold]] [str <| if model.LastCommand.Length > 0 then (sprintf "%s = " model.LastCommand) else ""]; str model.Output ]
                          ]
                      Column.column [Column.Width(Screen.All, Column.IsOneFifth)] [
                        Image.image [ Image.Is128x128
                                      Image.Props [ Style [ Margin "auto"] ] ]
                          [ img [ Src "assets/fulma_logo.svg"] ]
                        rs.selectOfList (getIconList()) (fun (arg: rs.SelectRow<Icon>) -> dispatch (ChangeIcon arg.value))
                        ]
                        ] ] ] ]

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
