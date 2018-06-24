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
    | Noop
open Commands
open Data

let rec (|Ints|_|) = pack <| function
  | Ints(lst, Int(v, rest)) ->
    Some(lst @ [v], rest)
  | Int(v, rest) -> Some([v], rest)
  | _ -> None

let parse = function
  | Str "new" End | Str "begin" End -> NewContext
  | Str "name" (WS (Any (name, End))) -> SetName <| name.Trim()
  | Words (AnyCase("rollstats" | "roll stats" | "roll"), End) -> RollStats
  | Str "assign" (Ints(stats, rest)) when stats.Length = 6 && stats |> Set.ofSeq |> Seq.length = stats.Length && (stats |> Seq.exists (betweenInclusive 1 6) |> not) -> // must be 6 unique numbers 1-6
    AssignStats stats
  | _ -> Noop

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

let update state resolve = function
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
    let { Str = a; Dex = b; Con = c; Int = d; Wis = e; Cha = f } = state
    let stats = [a;b;c;d;e;f]
    let fetch x = stats.[order.[x]-1]
    { state with Str = fetch 0; Dex = fetch 1; Con = fetch 2; Int = fetch 3; Wis = fetch 4; Cha = fetch 5 }
  | Noop -> state

let view state =
  sprintf "Name: %s\nStr %i Dex %i Con %i Int %i Wis %i Cha %i" state.Name state.Str state.Dex state.Con state.Int state.Wis state.Cha

type StatBank(roll) =
  let mutable state = State.Empty
  member val UpdateStatus = (fun (str: string) -> ()) with get, set
  member this.Execute(cmd: Command) =
    state <- (update state roll cmd)
    view state |> this.UpdateStatus
  new() =
    let random = System.Random()
    StatBank(resolve (fun x -> 1 + random.Next(x)))