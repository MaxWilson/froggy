module Froggy.Dnd5e.CharGen
open Froggy.Common
open Froggy.Dnd5e.Data

module Commands =
  type Command =
    | NewContext
    | SetName of string
    | RollStats
    | SwapStats of StatId * StatId
    | AssignStats of int list
    | Save of string option
    | Load of string
    | SetRace of RaceData option
open Commands
open Data

let statData = [
  StatId.Str, "Strength", Lens.lens (fun x -> x.Str) (fun v x -> { x with Str = v })
  StatId.Dex, "Dexerity", Lens.lens (fun x -> x.Dex) (fun v x -> { x with Dex = v })
  StatId.Con, "Constitution", Lens.lens (fun x -> x.Con) (fun v x -> { x with Con = v })
  StatId.Int, "Intelligence", Lens.lens (fun x -> x.Int) (fun v x -> { x with Int = v })
  StatId.Wis, "Wisdom", Lens.lens (fun x -> x.Wis) (fun v x -> { x with Wis = v })
  StatId.Cha, "Charisma", Lens.lens (fun x -> x.Cha) (fun v x -> { x with Cha = v })
]

module Grammar =
  open Froggy.Packrat
  let rec (|Ints|_|) = pack <| function
    | Ints(lst, Int(v, rest)) ->
      Some(lst @ [v], rest)
    | Int(v, rest) -> Some([v], rest)
    | _ -> None

  let commaAllowed = (alphanumeric + Set<_>[' '; ','])

  let (|Stat|_|) = pack <| function
    | Word(v, rest) ->
      match statData |> List.filter (fun (_, stringRep, _) -> stringRep.StartsWith(v, System.StringComparison.InvariantCultureIgnoreCase)) with
      | [(statId, _, _)] -> Some(statId, rest)
      | _ -> None
    | _ -> None

  let (|Race|_|) =
    let race (name, mods) =
      Some { RaceData.Name = name; Trait = None; Mods = mods |> List.map (fun (id, bonus) -> { StatMod.Stat = id; Bonus = bonus })}
    let raceTrait (name, mods, traitName) =
      Some { RaceData.Name = name; Trait = Some traitName; Mods = mods |> List.map (fun (id, bonus) -> { StatMod.Stat = id; Bonus = bonus })}
    pack <| function
    | Str "human" (Stat(s1, Stat(s2, Words(traitName, rest)))) -> Some(raceTrait("VHuman", [s1, +1; s2, +1], traitName), rest)
    | Str "human" (Words(traitName, Stat(s1, Stat(s2, rest)))) -> Some(raceTrait("VHuman", [s1, +1; s2, +1], traitName), rest)
    | Str "human" (Stat(s1, Stat(s2, rest))) -> Some(race("VHuman", [s1, +1; s2, +1]), rest)
    | Str "human" rest -> Some(race("Human", statData |> List.map (fun (id, _, _) -> id, +1)), rest)
    | Str "wood elf" rest -> Some(race("Wood elf", [Dex, +2; Wis, +1]), rest)
    | Str "half-elf" (Stat(s1, Stat(s2, rest))) when s1 <> Cha && s2 <> Cha -> Some(race("Half-elf", [s1, +1; s2, +1; Cha, +2]), rest)
    | Str "none" rest | Str "N/A" rest -> Some(None, rest)
    | _ -> None

  let (|Command|_|) = pack <| function
    | Str "new" rest | Str "begin" rest -> Some(NewContext, rest)
    | Str "name" (WS (Chars commaAllowed (name, rest))) -> Some( SetName <| name.Trim(), rest)
    | Words (AnyCase("rollstats" | "roll stats" | "roll"), rest) -> Some(RollStats, rest)
    | Str "assign" (Ints(stats, rest)) when stats.Length = 6 -> // must be 6 numbers that will be interpreted as priorities for stats, in order
      Some(AssignStats stats, rest)
    | Str "save" (Words(fileName, rest)) -> Some(Save (Some fileName), rest)
    | Str "save" rest -> Some(Save None, rest)
    | Str "load" (Words(fileName, rest)) -> Some(Load fileName, rest)
    | Word (AnyCase("swap" | "sw"), (Stat(s1, Stat(s2, rest)))) -> Some(SwapStats(s1, s2), rest)
    | (Race(race, rest)) -> Some(SetRace race, rest)
    | _ -> None

  let rec (|Commands|_|) = pack <| function
    | Commands(cmds, Str ";" (OWS(Command(c, rest)))) -> Some(cmds @ [c], rest)
    | Command(c, rest) -> Some([c], rest)
    | _ -> None

let parse input =
  match input with
  | Grammar.Commands(cmds, Froggy.Packrat.End) -> cmds
  | _ -> []

module Prop =
  let Current = Lens.lens
                  (function { Current = Some(v); Party = P } -> P.[v] | { Current = None } -> StatBlock.Empty)
                  (fun v state  -> match state with
                                    | { Current = Some(ix); Party = P } -> { state with Party = P |> List.mapi (fun i v' -> if i = ix then v else v') }
                                    | _ -> { state with Current = Some 0; Party = [v] }
                                    )
  let StatArray = Lens.lens (function { Stats = stats } -> stats) (fun v state -> { state with Stats = v })
  let statLens id =
    match statData |> List.find (function (id', _, _) when id = id' -> true | _ -> false) with
    | (_, _, lens) -> Current << StatArray << lens
  let Str = statLens Str
  let Dex = statLens Dex
  let Con = statLens Con
  let Int = statLens Int
  let Wis = statLens Wis
  let Cha = statLens Cha
  let StatsInOrder = [Str;Dex;Con;Int;Wis;Cha]
open Prop

module View =
  let statView id statBlock =
    match statData |> List.find (function (id', _, _) when id = id' -> true | _ -> false) with
    | (_, _, lens) ->
      let v = Lens.view (StatArray << lens) statBlock
      let mods = match statBlock.Race with Some(r) -> r.Mods |> List.sumBy (fun m -> if id = m.Stat then m.Bonus else 0) | None -> 0
      match v + mods with
      | total when total > 20 || total < 1 -> v // don't use mods if that would push values out of bounds
      | total -> total

let view (state: State) =
  let statBlock = Lens.view Current state
  let stats =
    statData
    |> List.map (fun (id, stringRep, _) -> sprintf "%s %d" (stringRep.Substring(0,3)) (statBlock |> View.statView id))
    |> String.join ", "
  [
    (sprintf "Name: %s" (Lens.view Prop.Current state).Name)
    (match statBlock.Race with
      | Some({ Name = name; Trait = Some(traitName) }) -> sprintf "%s [%s]" name traitName
      | Some(r) -> r.Name
      | None -> emptyString)
    stats
  ]
  |> List.filter ((<>) emptyString)
  |> String.join "\n"

type IO<'t> =
  {
    save: string -> 't -> unit
    load: string -> 't option
  }

type StatBank(roll) =
  let mutable state = { State.Empty with Current = Some 0; Party = [StatBlock.Empty] }
  member val UpdateStatus = (fun (str: string) -> ()) with get, set
  member val IO = { save = (fun _ _ -> failwith "Not supported"); load = (fun _ -> failwith "Not supported") } with get, set
  member this.Execute(cmds: Command list) =
    for cmd in cmds do
      state <-
        let hasCurrent = state.Current.IsSome
        match cmd with
        | NewContext -> { State.Empty with Current = Some 0; Party = [StatBlock.Empty] }
        | SetName v when hasCurrent ->
           state |> Lens.over Prop.Current (fun st -> { st with Name = v })
        | RollStats when hasCurrent->
          state |> Lens.over Prop.Current (fun st ->
            { st with
                Stats =
                {
                Str = roll (RollSpec.SumBestNofM(4,3,6))
                Dex = roll (RollSpec.SumBestNofM(4,3,6))
                Con = roll (RollSpec.SumBestNofM(4,3,6))
                Int = roll (RollSpec.SumBestNofM(4,3,6))
                Wis = roll (RollSpec.SumBestNofM(4,3,6))
                Cha = roll (RollSpec.SumBestNofM(4,3,6))
                }
            })
        | AssignStats(order) when hasCurrent ->
          let statValues = Prop.StatsInOrder |> List.map (flip Lens.view state) |> List.sortDescending
          let statsByPriority = Prop.StatsInOrder |> List.mapi (fun i prop -> order.[i], prop) |> List.sortBy fst
          let state = statsByPriority
                      |> List.mapi (fun i (_, prop) -> i, prop) // now that they're in order of priority, match each one up with a unique statValue index
                      |> List.fold (fun state (ix, prop) -> state |> Lens.set prop statValues.[ix]) state
          state
        | SwapStats(s1, s2) when hasCurrent ->
          let lenses = [s1;s2] |> List.map statLens
          let vals = lenses |> List.map (fun l -> Lens.view l state) |> List.rev
          (vals |> List.zip lenses) |> List.fold (fun st (l, v) -> Lens.set l v st) state
        | SetRace(race) when hasCurrent ->
          state |> Lens.over Prop.Current (fun st ->
            { st with
                Race = race
            })
        | Save(fileName) when hasCurrent ->
          this.IO.save (defaultArg fileName (Lens.view Current state).Name) (Lens.view Current state)
          state
        | Load(fileName) ->
          match this.IO.load fileName with
          | Some(newChar) ->
            Lens.set Current newChar state
          | None -> state
    view state |> this.UpdateStatus
  member this.Execute(cmd) = this.Execute [cmd]
  new() =
    let random = System.Random()
    StatBank(resolve <| fun x -> 1 + random.Next(x))
