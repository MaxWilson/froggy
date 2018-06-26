module Froggy.Web

open Elmish
open Elmish.Browser.Navigation
open Elmish.Browser.UrlParser
open Fable.Core
open Fable.Core.JsInterop
open Fable.Import
open Fable.Import.Browser
open App.State
open Global

importAll "../sass/main.sass"

open Fable.Helpers.React
open Fable.Helpers.React.Props
open Froggy.Common
open EncounterGen
module R = Fable.Helpers.React

type Model = { number: int; level: int; difficulty: int; msg: string }
type Msg = Generate | AlterLevel of int | AlterPCNumber of int | AlterDifficulty of int

let update msg model =
  match msg with
  | AlterLevel(n) -> { model with level = model.level + n |> max 1 |> min 20 }
  | AlterPCNumber(n) -> { model with number = model.number + n |> max 1 }
  | AlterDifficulty(n) -> { model with difficulty = model.difficulty + n |> max 1 }
  | Generate ->
    { model with msg = generateVariant monsterParties (List.init model.number (thunk model.level)) model.difficulty }

let init() = { msg = "Push the button to generate an encounter"; level = 1; difficulty = 4; number = 4 }

let descr model =
  let diff =
    match model.difficulty with
    | n when n <= 4 -> ["Easy";"Medium";"Hard";"Deadly"].[n-1]
    | n -> "Deadly x" + (n - 3).ToString()
  let lev =
    match model.level with
    | 1 -> "1st"
    | 2 -> "2nd"
    | n -> (n.ToString()) + "th"
  sprintf "%s encounter for %i %s-level PCs" diff model.number lev

let view model d =
  R.div []
      [ R.div [] [ R.str <| descr model ]
        R.button [ OnClick (fun _ -> d <| AlterLevel +1) ] [ R.str "Add PC" ]
        R.button [ OnClick (fun _ -> d <| AlterLevel -1) ] [ R.str "Remove PC" ]
        R.button [ OnClick (fun _ -> d <| AlterPCNumber +1) ] [ R.str "Stronger PCs" ]
        R.button [ OnClick (fun _ -> d <| AlterPCNumber -1) ] [ R.str "Weaker PCs" ]
        R.button [ OnClick (fun _ -> d <| AlterDifficulty +1) ] [ R.str "Harder" ]
        R.button [ OnClick (fun _ -> d <| AlterDifficulty -1) ] [ R.str "Easier" ]
        R.button [ OnClick (fun _ -> Generate |> d) ] [ R.str "Go" ]
        R.div [] [ R.str <| model.msg.Replace("\n", "<p>") ]
]

open Elmish.React
open Elmish.Debug
open Elmish.HMR

// App
Program.mkSimple init update view
#if DEBUG
|> Program.withDebugger
|> Program.withHMR
#endif
|> Program.withReact "elmish-app"
|> Program.run

