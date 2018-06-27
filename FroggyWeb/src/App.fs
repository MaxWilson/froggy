module Froggy.Web

open Elmish
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

type Model = { number: int; level: int; difficulty: int; encounter: Encounter option }
type Msg = Generate | AlterLevel of int | AlterPCNumber of int | AlterDifficulty of int

let update msg model =
  let st =
    match msg with
    | AlterLevel(n) -> { model with level = model.level + n |> max 1 |> min 20 }
    | AlterPCNumber(n) -> { model with number = model.number + n |> max 1 }
    | AlterDifficulty(n) -> { model with difficulty = model.difficulty + n |> max 1 }
    | Generate ->
      { model with encounter = Some <| generateVariant monsterParties (List.init model.number (thunk model.level)) model.difficulty }
  st, Cmd.Empty
let init() = { encounter = None; level = 1; difficulty = 4; number = 4 }, Cmd.ofMsg Generate


let view model d =
  let targetDifficulty =
    match model.difficulty with
    | n when n <= 4 -> ["Easy";"Medium";"Hard";"Deadly"].[n-1]
    | n -> "Deadly x" + (n - 3).ToString()
  let descr model =
    let lev =
      match model.level with
      | 1 -> "1st"
      | 2 -> "2nd"
      | n -> (n.ToString()) + "th"
    sprintf "%s encounter for %i %s-level PCs" targetDifficulty model.number lev
  div [Style [Margin 20]]
    [ div [] [ str <| descr model ]
      button [ OnClick (fun _ -> d <| AlterPCNumber +1) ] [ str "Add PC" ]
      button [ OnClick (fun _ -> d <| AlterPCNumber -1) ] [ str "Remove PC" ]
      button [ OnClick (fun _ -> d <| AlterLevel +1) ] [ str "Stronger PCs" ]
      button [ OnClick (fun _ -> d <| AlterLevel -1) ] [ str "Weaker PCs" ]
      button [ OnClick (fun _ -> d <| AlterDifficulty +1) ] [ str "Harder" ]
      button [ OnClick (fun _ -> d <| AlterDifficulty -1) ] [ str "Easier" ]
      button [ OnClick (fun _ -> Generate |> d) ] [ str "Go" ]
      div [] <|
        match model.encounter with
        | None -> []
        | Some(e) ->
          [
          div [Style [FontWeight "bold"]] [str (sprintf "Difficulty %s" e.difficulty)]
          div [] [str (sprintf "(DMG standard difficulty %s)" e.standardDifficulty)]
          div [] [
            span [Style [FontWeight "bold"]] [str (sprintf "Cost: %i XP" e.cost)]
            span [Style [FontStyle "italic"]] [str (sprintf "  Budget: %i XP" e.xpBudgets.[model.difficulty])]
            ]
          div [] [
            span [] [str (sprintf "(DMG standard cost: %i XP)" e.standardCost)]
            span [Style [FontStyle "italic"]] [str (sprintf "  Budget: %i XP" e.standardXPBudgets.[model.difficulty])]
            ]
          div [] <|
            (e.roster |>
              List.map (fun (monster, n) -> div [] [str <| sprintf "%i %s" n (if n <= 1 || monster.EndsWith("s") then monster else monster + "s")] ))
          div [Style [FontWeight "bold"]] [str (sprintf "XP earned: %i XP" e.earnedXP)]
          ]
]


open Elmish.React
open Elmish.Debug
open Elmish.HMR

// App
Program.mkProgram init update view
#if DEBUG
|> Program.withDebugger
|> Program.withHMR
#endif
|> Program.withReact "elmish-app"
|> Program.run

