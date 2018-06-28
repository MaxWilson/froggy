module Froggy.Dnd5e.CharGen
open Froggy.Packrat
open Froggy.Common

module Commands =
  type Command =
    | NewContext
    | SetName of string
    | RollStats
    | AssignStats of int list
    | Save of string option
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
  | Str "save" (WS(Words(fileName, rest))) -> Some(Save (Some fileName), rest)
  | Str "save" rest -> Some(Save None, rest)
  | Str "load" (WS(Words(fileName, rest))) -> Some(Load fileName, rest)
  | _ -> None

let rec (|Commands|_|) = pack <| function
  | Commands(cmds, Str ";" (OWS(Command(c, rest)))) -> Some(cmds @ [c], rest)
  | Command(c, rest) -> Some([c], rest)
  | _ -> None

let parse input =
  match input with
  | Commands(cmds, End) -> cmds
  | _ -> []

type StatId = Str | Dex | Con | Int | Wis | Cha
type StatArray = {
    Str: int
    Dex: int
    Con: int
    Int: int
    Wis: int
    Cha: int
  }

let statData = [
  StatId.Str, "Strength", Lens.lens (fun x -> x.Str) (fun v x -> { x with Str = v })
  StatId.Dex, "Dexerity", Lens.lens (fun x -> x.Dex) (fun v x -> { x with Dex = v })
  StatId.Con, "Constitution", Lens.lens (fun x -> x.Con) (fun v x -> { x with Con = v })
  StatId.Int, "Intelligence", Lens.lens (fun x -> x.Int) (fun v x -> { x with Int = v })
  StatId.Wis, "Wisdom", Lens.lens (fun x -> x.Wis) (fun v x -> { x with Wis = v })
  StatId.Cha, "Charisma", Lens.lens (fun x -> x.Cha) (fun v x -> { x with Cha = v })
]

type StatBlock = {
    Name : string
    Stats: StatArray
    HP: int
  }
  with
  static member Empty = {
    Name = "Unnamed"
    Stats =
      {
      Str = 10
      Dex = 10
      Con = 10
      Int = 10
      Wis = 10
      Cha = 10
      }
    HP = 1
  }

type State = {
  Current: int option
  Party: StatBlock list
}


module Prop =
  let Current = Lens.lens
                  (function { Current = Some(v); Party = P } -> Some P.[v] | { Current = None } -> None)
                  (fun v state  -> match v, state with
                                | Some(v), { Current = Some(ix); Party = P } -> { state with Party = P |> List.mapi (fun i v' -> if i = ix then v else v') }
                                | Some(v), _ -> { state with Current = Some 0; Party = [v] }
                                | _ -> state)
  let OptionLens = Lens.lens (function Some(v) -> v | None -> None) (fun v state -> Some v)
  let StatArray = Lens.lens (function { Stats = stats } -> stats) (fun v state -> { state with Stats = v })
  let getStat id =
    match statData |> List.tryFind (function (id', _, _) when id = id' -> true | _ -> false) with
    | Some(_, _, lens) -> Lens.view (Current >> OptionLens >> StatArray >> lens)
  let Dex = Lens.lens (fun x -> x.Dex) (fun v x -> { x with Dex = v })
  let Con = Lens.lens (fun x -> x.Con) (fun v x -> { x with Con = v })
  let Int = Lens.lens (fun x -> x.Int) (fun v x -> { x with Int = v })
  let Wis = Lens.lens (fun x -> x.Wis) (fun v x -> { x with Wis = v })
  let Cha = Lens.lens (fun x -> x.Cha) (fun v x -> { x with Cha = v })
  let StatsInOrder = [Str;Dex;Con;Int;Wis;Cha]

let view state =
  sprintf "Name: %s\nStr %i Dex %i Con %i Int %i Wis %i Cha %i" state.Name state.Str state.Dex state.Con state.Int state.Wis state.Cha

type IO<'t> =
  {
    save: string -> 't -> unit
    load: string -> 't option
  }

type StatBank(roll) =
  let mutable state = StatBlock.Empty
  member val UpdateStatus = (fun (str: string) -> ()) with get, set
  member val IO = { save = (fun _ _ -> failwith "Not supported"); load = (fun _ -> failwith "Not supported") } with get, set
  member this.Execute(cmd: Command) =
    let update state =
      match cmd with
      | NewContext -> StatBlock.Empty
      | SetName v -> { state with Name = v }
      | RollStats ->
        {
          state with
            Str = roll (RollSpec.SumBestNofM(4,3,6))
            Dex = roll (RollSpec.SumBestNofM(4,3,6))
            Con = roll (RollSpec.SumBestNofM(4,3,6))
            Int = roll (RollSpec.SumBestNofM(4,3,6))
            Wis = roll (RollSpec.SumBestNofM(4,3,6))
            Cha = roll (RollSpec.SumBestNofM(4,3,6))
        }
      | AssignStats(order) ->
        let statValues = Prop.StatsInOrder |> List.map (flip Lens.view state) |> List.sortDescending
        let statsByPriority = Prop.StatsInOrder |> List.mapi (fun i prop -> order.[i], prop) |> List.sortBy fst
        let state = statsByPriority
                    |> List.mapi (fun i (_, prop) -> i, prop) // now that they're in order of priority, match each one up with a unique statValue index
                    |> List.fold (fun state (ix, prop) -> state |> Lens.set prop statValues.[ix]) state
        state
      | Save(fileName) ->
        this.IO.save (defaultArg fileName state.Name) state
        state
      | Load(fileName) ->
        match this.IO.load fileName with
        | Some(newChar) -> newChar
        | None -> state
    state <- (update state)
    view state |> this.UpdateStatus
  new() =
    let random = System.Random()
    StatBank(resolve <| fun x -> 1 + random.Next(x))
