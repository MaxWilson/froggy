module Froggy.Dnd5e.CharGen
open Froggy.Packrat
open Froggy.Common

module Commands =
  type Command =
    | NewContext
    | SetName of string
    | RollStats
    | AssignStats of int list
    | Save of string
    | Load of string
open Commands
open Data

let rec (|Ints|_|) = pack <| function
  | Ints(lst, Int(v, rest)) ->
    Some(lst @ [v], rest)
  | Int(v, rest) -> Some([v], rest)
  | _ -> None

let commaAllowed = (alphanumeric + Set<_>[' '; ','])

let (|Command|_|) = pack <| function
  | Str "new" rest | Str "begin" rest -> Some(NewContext, rest)
  | Str "name" (WS (Chars commaAllowed (name, rest))) -> Some( SetName <| name.Trim(), rest)
  | Words (AnyCase("rollstats" | "roll stats" | "roll"), rest) -> Some(RollStats, rest)
  | Str "assign" (Ints(stats, rest)) when stats.Length = 6 -> // must be 6 numbers that will be interpreted as priorities for stats, in order
    Some(AssignStats stats, rest)
  | _ -> None

let rec (|Commands|_|) = pack <| function
  | Commands(cmds, Str ";" (OWS(Command(c, rest)))) -> Some(cmds @ [c], rest)
  | Command(c, rest) -> Some([c], rest)
  | _ -> None

let parse input =
  match input with
  | Commands(cmds, End) -> cmds
  | _ -> []

type State = {
    Name : string
    Str: int
    Dex: int
    Con: int
    Int: int
    Wis: int
    Cha: int
    HP: int
  }
  with
  static member Empty = {
    Name = "Unnamed"
    Str = 10
    Dex = 10
    Con = 10
    Int = 10
    Wis = 10
    Cha = 10
    HP = 1
  }

module Prop =
  let Str = Lens.lens (fun x -> x.Str) (fun v x -> { x with Str = v })
  let Dex = Lens.lens (fun x -> x.Dex) (fun v x -> { x with Dex = v })
  let Con = Lens.lens (fun x -> x.Con) (fun v x -> { x with Con = v })
  let Int = Lens.lens (fun x -> x.Int) (fun v x -> { x with Int = v })
  let Wis = Lens.lens (fun x -> x.Wis) (fun v x -> { x with Wis = v })
  let Cha = Lens.lens (fun x -> x.Cha) (fun v x -> { x with Cha = v })
  let StatsInOrder = [Str;Dex;Con;Int;Wis;Cha]

let update resolve cmd state =
  match cmd with
  | NewContext -> State.Empty
  | SetName v -> { state with Name = v }
  | RollStats ->
    {
      state with
        Str = resolve (RollSpec.SumBestNofM(4,3,6))
        Dex = resolve (RollSpec.SumBestNofM(4,3,6))
        Con = resolve (RollSpec.SumBestNofM(4,3,6))
        Int = resolve (RollSpec.SumBestNofM(4,3,6))
        Wis = resolve (RollSpec.SumBestNofM(4,3,6))
        Cha = resolve (RollSpec.SumBestNofM(4,3,6))
    }
  | AssignStats(order) ->
    let statValues = Prop.StatsInOrder |> List.map (flip Lens.view state) |> List.sortDescending
    let statsByPriority = Prop.StatsInOrder |> List.mapi (fun i prop -> order.[i], prop) |> List.sortBy fst
    let state = statsByPriority
                |> List.mapi (fun i (_, prop) -> i, prop) // now that they're in order of priority, match each one up with a unique statValue index
                |> List.fold (fun state (ix, prop) -> state |> Lens.set prop statValues.[ix]) state
    state

let view state =
  sprintf "Name: %s\nStr %i Dex %i Con %i Int %i Wis %i Cha %i" state.Name state.Str state.Dex state.Con state.Int state.Wis state.Cha

type StatBank(roll) =
  let mutable state = State.Empty
  member val UpdateStatus = (fun (str: string) -> ()) with get, set
  member this.Execute(cmd: Command) =
    state <- (update roll cmd state)
    view state |> this.UpdateStatus
  new() =
    let random = System.Random()
    StatBank(resolve (fun x -> 1 + random.Next(x)))