module Frost.App.View

open Elmish
open Fable.Helpers.React
open Fable.Helpers.React.Props
open Fulma
open Froggy.Data
open Froggy.Packrat
open Fable.Core.JsInterop
open Froggy.Common

type Icon = string
type IconDefinition = IconDefinition of Icon * url:string
let Frog, Bear, Wolf = "Frog", "Bear", "Wolf"
open Fable.Import.ReactPixiFable
open Fable.Import
open System
open Frost.Domain

module rs = Fable.Import.ReactSelect
module rpf = Fable.Import.ReactPixiFable
let frog = rpf.Texture.fromImage("assets/frog-icon.png")
let bear = rpf.Texture.fromImage("data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAHsAAAB7CAMAAABjGQ9NAAAAY1BMVEX///8AAAD8/Pzd3d3y8vLBwcE1NTV+fn7T09Ph4eEjIyMaGhrm5ubMzMw4ODj5+fkREREtLS2rq6tlZWVFRUUoKCjs7Ox4eHhKSkpvb29SUlK0tLRYWFidnZ2QkJAKCgqGhobuInimAAAD2ElEQVRoge2aabeqIBSGVTA1M+cph/z/v/JqNgDJducJW3ct3i+n0xIegT2wIcPQ0tLS0tLS0tL630Tt06/QhWmaDr2/xs5v0Y9s8xxOb1Ee/ZLuiLbqiW2mlmHH04dhR3bY3tjmkMx/fbIf+z7ul5r92Eb3Mza1LwLb2cvagrP5pnO/C12c74dKWzXZGiTo2+AJDUNaeIQQT8E8AOhJWZpWh9unS+99l0xbGM2rssPvob3sE/S0Cs23pr6LPkSPirvi7+CiOXxOvik6XNJy6LvGDka5rjta4qmgyPWgblduJS/IP+aHrGrT2ukDasFoMXp/VbmTQJlIJXpSdC5dmVXkquGjDk6wyJZF0W+rd9/Zpz0GPilqgzfrT3Zij8qSX836pDwR3K7fEW62gtMFxz3pDg8Pu08TyV9UCSZP7XJDMtmo6N3di6YX94mqtLT3DYuk3WXxJfs/L1nYq35d0iLTVZrdbsrkKd5TTofKHaKYDgx8lK3W6RcSGyNaqmSvlTqNwoAzrLANog7er7EN4qtid6tsI1DFxpwprFSIm7W8f+Tl/ZBtxGrYsH8rnfQIxXaVsHHndqcvloqfso1UBfuIYyvZwR9xRzZKvAzJtlQEdSTbUJFLsWwV9SLS1pTkE+y5fKigSEffCYD1ynVTio+wbNDDo1PR1R8nnCsqnhsrIT2aLLYgTZ3lvDPC84Fle9c19k0WCZok6fqh75OkCQiYCLBsWgGd+DJPtcCTadTeYZIDdJJL6zqwtEGzIWOLpWzwJAfNtoFOztKTbNA90OwC6KSSnuELsdjnrAZ//QOwW2lF2fAP1ty74NmAr9bSRsJK2VyYwN83Ak7mSBsJIenEjeDtgFMqIIXLizo+JKUW53SIeuwuwF3kndBIeI5dhPU69CHWTCL+QEJuNCFnJaM7eMwB2rCJnVPuFAzY/LDsbPzfYrbb5SZ2yjsP0Io9J7zZFrN0cvcQxQb0wCiYdB3j2HPkZSy/RbOZWR5zR8hkKLmLcex5mNbriwpJpuyU1/y6ATGC3WPe0/VrwjIcOuHsdVy44jUNwF6X9ajLHHjpqydorZb7GBVyA5JPHddsjmLsj0iOGLSw/RjnysrETpeasY38+TvWP64YdsGfaSdcXj5Kk/fCsLm9to9hh/zu3OUKcnlAZdnZPcWzNcaAYXOx/DJF0NfUQY7CGOjDHpmXznH39rQ+x3keH7I6IbO5Po5/WqgD+wl/2oT7TC5X9JbJsCh/l+86h2ueNvDlOhmm2BKz17Dkvn7ln34YZ1krt/rzQ6H4GHVtmyCaamlpaWlpaWlp/Vr/AGrtNhOIbghJAAAAAElFTkSuQmCC")
let wolf = rpf.Texture.fromImage("https://png.icons8.com/ios/1600/wolf.png", true)
let mutable icons = Map.ofSeq [Frog, frog; Bear, bear; Wolf, wolf]

module Coords =
  type D = { x: int; y : int }
  let ofPair (x,y) = { x = x; y = y}
  let add lhs rhs = ofPair (lhs.x + rhs.x, lhs.y + rhs.y)
  let diff lhs rhs =
    let dx = lhs.x - rhs.x
    let dy = lhs.y - rhs.y
    dx * dx + dy * dy |> float |> sqrt
open Coords
type Stats = {
    id: int
    creature: Creature
    coords: Coords.D
    hp: int
    moveUsed: int
    status: string option
  }

type Model =
    { Input : string
      SelectedCreature: Stats option
      LastCommand: string
      OutputLabel: string
      Output : string
      Stats: Stats list
    }

type Name = string
type Action =
  | Move of Coords.D
  | Attack of target: Name
  | Impose of condition: string
  | Relax of condition: string
  | Cast of string
type Msg =
  | AddCreature of Creature
  | CommandCreature of Name * Action
  | SelectCreature of Name
  | ChangeCommandInput of string
  | ExecuteCommand

let init _ =
  {
    Input = "";
    LastCommand  = "";
    OutputLabel = "";
    Output = "";
    SelectedCreature = None;
    Stats = []
  },
  Cmd.none

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

let defineIcon (IconDefinition(tag, url)) =
  match icons |> Map.tryFind(tag) with
  | Some(texture) -> ()
  | None ->
    let texture = rpf.Texture.fromImage(url, true)
    icons <- icons |> Map.add tag texture
let getIcon tag =
  match icons |> Map.tryFind(tag) with
  | Some(icon) -> icon
  | None ->
    let texture = rpf.Texture.fromImage(tag)
    icons <- icons |> Map.add tag texture
    texture
let getIconList() =
  [
    for KeyValue(tag, (texture)) in icons do
      if(texture.valid) then
        yield rs.Option tag
    ] |> Array.ofList

let (|Lookup|_|) model name =
  model.Stats |> List.tryFind (fun st -> st.creature.name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase))
let isSameCreature stats1 stats2 =
  stats1.creature.name.StartsWith(stats2.creature.name, StringComparison.InvariantCultureIgnoreCase)
let updateSameCreature stats' stats =
  if isSameCreature stats' stats then stats' else stats
let updateStats (stats: Stats) (model: Model) =
  { model with Stats = model.Stats |> List.map (updateSameCreature stats)
               SelectedCreature = model.SelectedCreature |> Option.map (updateSameCreature stats) }

let (|IntOr|) n = function
  | Int(n, rest) -> n, rest
  | rest -> 1, rest

let (|Direction|_|) = function
  | Str "up" (IntOr 1 (n, rest)) -> Some((0, n), rest)
  | Str "down" (IntOr 1 (n, rest)) -> Some((0, -n), rest)
  | Str "left" (IntOr 1 (n, rest)) -> Some((-n, 0), rest)
  | Str "right" (IntOr 1 (n, rest)) -> Some((n, 0), rest)
  | _ -> None

let private update msg model =
  match msg with
  | ChangeCommandInput newValue ->
    { model with Input = newValue }, Cmd.none
  | SelectCreature (Lookup model stats) ->
    { model with SelectedCreature = Some stats }, Cmd.none
  | CommandCreature(Lookup model creature, Move coords) ->
    let moveUsed = Coords.diff coords creature.coords
    model |> updateStats { creature with coords = coords; moveUsed = creature.moveUsed + (5. * moveUsed |> int)}, Cmd.none
  | ExecuteCommand ->
    match model.Input.Trim(), model.LastCommand with
    | "", cmd
    | cmd, _ ->
      match ParseArgs.Init cmd with
      | Str "add" (Word(name, (Word(AnyCase("giant" | "cat" as tag), Int(x, Str "," (Int(y, End))))))) ->
        let creature = (if tag = "giant" then frostGiant else cat) name
        let id = model.Stats.Length + 1
        let name = sprintf "%s #%d" tag id
        let stats = { creature = creature; id = id; coords = { x = x; y = y }; hp = creature.hp; moveUsed = 0; status = None }
        { model with Input = ""; Stats = model.Stats @ [ stats ]; SelectedCreature = Some stats }, Cmd.none
      | Str "move" (Word(name, Int(x, Str "," (Int(y, End))))) ->
        { model with Input = ""; OutputLabel = ""; Output = cmd; LastCommand = cmd }, Cmd.ofMsg(CommandCreature(name, Move { x = x; y = y }))
      | Str "move" (Int(x, Str "," (Int(y, End)))) when model.SelectedCreature.IsSome ->
        { model with Input = ""; OutputLabel = ""; Output = cmd; LastCommand = cmd }, Cmd.ofMsg(CommandCreature(model.SelectedCreature.Value.creature.name, Move { x = x; y = y }))
      | Str "move" (OWS(Direction(delta, End))) when model.SelectedCreature.IsSome ->
        let coords = Coords.add (Coords.ofPair delta) model.SelectedCreature.Value.coords
        { model with Input = ""; OutputLabel = ""; Output = cmd; LastCommand = cmd }, Cmd.ofMsg(CommandCreature(model.SelectedCreature.Value.creature.name, Move coords))
      | Str "move" (OWS(Direction(delta, Direction(delta2, End)))) when model.SelectedCreature.IsSome ->
        let coords = Coords.add (Coords.add (Coords.ofPair delta) (Coords.ofPair delta2)) model.SelectedCreature.Value.coords
        { model with Input = ""; OutputLabel = ""; Output = cmd; LastCommand = cmd }, Cmd.ofMsg(CommandCreature(model.SelectedCreature.Value.creature.name, Move coords))
      | Str "select" (Word(name, End)) ->
        match model.Stats |> List.tryFind (fun st -> st.creature.name.StartsWith(name, StringComparison.InvariantCultureIgnoreCase)) with
        | None ->
          { model with Input = ""; Output = sprintf "No such creature '%s'" name }, Cmd.ofMsg (SelectCreature name)
        | Some creature ->
          { model with Input = ""; Output = sprintf "Selected '%s'" creature.creature.name }, Cmd.ofMsg (SelectCreature name)
      | _ ->
        match RollHelper.execute cmd with
        | None, msg ->
          { model with OutputLabel = model.LastCommand; Output = msg }, Cmd.none
        | Some(qty), msg ->
          { model with Input = ""; OutputLabel = (cmd + " = "); LastCommand = cmd; Output = msg }, Cmd.none
  | cmd ->
    { model with Input = ""; Output = sprintf "Not implemented: %A" cmd }, Cmd.none

let screenWidth, screenHeight = 800, 750
let opts = [|"Frog"; "Toad"|] |> Array.map rs.Option
let labelledValue label text =
  let bold = [Modifiers [Modifier.TextWeight TextWeight.Bold]]
  Control.div [] [
    Text.span bold [str (label + ": ")]
    Text.span [] [str text]
  ]
let scaleInPixels = 50
let private view model dispatch =
  Hero.hero [ Hero.IsFullHeight ] [
    Hero.body [ ] [
      Container.container [ ] [
        Columns.columns [ ] [
          Column.column [ Column.Width(Screen.All, Column.IsOneFifth) ] [
            Control.div [] [
              match model.SelectedCreature with
              | Some(stats) ->
                let cr = stats.creature
                yield Heading.h5 [] [str cr.name]
                yield hr[]
                yield labelledValue "Str" (cr.str.ToString())
                yield labelledValue "Dex" (cr.dex.ToString())
                yield labelledValue "Con" (cr.con.ToString())
                yield labelledValue "Int" (cr.int.ToString())
                yield labelledValue "Wis" (cr.wis.ToString())
                yield labelledValue "Cha" (cr.cha.ToString())
                yield hr[]
                yield labelledValue "AC" (cr.ac.ToString())
                yield labelledValue "HP" (sprintf "%d out of %d" stats.hp cr.hp)
                yield labelledValue "Move used" (sprintf "%d out of %d" stats.moveUsed cr.speed)
              | None -> ()
              ]
            ]
          Column.column [ Column.CustomClass "has-text-centered"; Column.Width(Screen.All, Column.IsThreeFifths)]
            [ stage { createEmpty<StageProperties> with width = screenWidth; height = screenHeight; options = { backgroundColor = 0x10bb99 } } [
                for i in 1..10 do
                  yield text { createEmpty<TextProperties> with text = "Hello world"; position = { x = 40+(i*4); y = 70+(i*15) }; alpha = (1.0 - (0.1 * float i)) } []
                for stats in model.Stats do
                  let { Coords.x = x; y = y } = stats.coords
                  let sizeX, sizeY = stats.creature.size
                  yield sprite { createEmpty<SpriteProperties> with interactive=true;pointerdown=(thunk1 dispatch (SelectCreature stats.creature.name));anchor={FractionalPoint.x = 0.5; y = 0.5};height=scaleInPixels*sizeX;width=scaleInPixels*sizeY; texture = getIcon stats.creature.icon; position = { x = stats.coords.x * 50; y = screenHeight - ((stats.coords.y + 1) * 50) }; alpha = 1. } []
                  yield text { createEmpty<TextProperties> with anchor={FractionalPoint.x = 0.5; y = 0.5}; text = stats.creature.name; position = { x = stats.coords.x * 50 + 5; y = screenHeight - ((stats.coords.y) * 50); }; alpha = 0.99; style = { fill = "Blue" } } []
                ]
              Level.level[] [
                Button.button [] [str "Attack"]
                Button.button [] [str "Move"]
                Button.button [] [str "Dodge"]
                Button.button [] [str "Cast"]
                Button.button [] [str "Wait"]
              ]
              Field.div [ ]
                [ Label.label [ ]
                    [ str "Enter a command" ]
                  Control.div [ ]
                    [ Input.input [
                        Input.OnChange (fun ev -> ChangeCommandInput ev.Value |> dispatch)
                        Input.Value model.Input
                        Input.Props [ AutoFocus true; OnKeyDown(fun ev -> if (ev.key = "Enter") then dispatch ExecuteCommand) ] ] ] ]
              Content.content [ ]
                [ Text.span [Modifiers [Modifier.TextWeight TextWeight.Bold]] [str <| if model.OutputLabel.Length > 0 then model.OutputLabel else ""]; str model.Output ]
              ]
          Column.column [Column.Width(Screen.All, Column.IsOneFifth); Column.Props[Style[Height "90vh"]]] [
            Level.level [] [
              Button.button [] [str "<<"]
              Button.button [] [str "Pause"]
              Button.button [] [str "Play"]
              Button.button [] [str ">>"]
            ]
            Heading.h6 [] [str "Round 2"]

            Heading.h5 [] [str "Log"]
            Control.div [Control.Props [Style [Height "70vh";OverflowY "auto"]]] [
              for x in 1..20 ->
                Control.div [] [str "So that happened..."]
              ]
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
