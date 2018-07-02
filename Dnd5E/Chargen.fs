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
    | SetXP of int
    | SetClassGoal of ClassLevel
    | AmendNote of string
    | AddNote of string
    | ClearNotes
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
      let mods =
        let bonus =
          match (traitName : string).ToLowerInvariant() with
          | "ham" | "heavy armor master" -> Some Str
          | _ -> None
        match bonus with
        | Some(v) -> (v, +1) :: mods
        | None -> mods
      Some { RaceData.Name = name; Trait = Some traitName; Mods = mods |> List.map (fun (id, bonus) -> { StatMod.Stat = id; Bonus = bonus })}
    pack <| function
    | Str "human" (Stat(s1, Stat(s2, Words(traitName, rest)))) -> Some(raceTrait("VHuman", [s1, +1; s2, +1], traitName), rest)
    | Str "human" (Stat(s1, Stat(s2, rest))) -> Some(race("VHuman", [s1, +1; s2, +1]), rest)
    | Str "human" rest -> Some(race("Human", statData |> List.map (fun (id, _, _) -> id, +1)), rest)
    | Str "wood elf" rest -> Some(race("Wood elf", [Dex, +2; Wis, +1]), rest)
    | Str "half-elf" (Stat(s1, Stat(s2, rest))) when s1 <> Cha && s2 <> Cha -> Some(race("Half-elf", [s1, +1; s2, +1; Cha, +2]), rest)
    | Str "none" rest | Str "N/A" rest -> Some(None, rest)
    | _ -> None

  let (|ClassGoal|_|) =
    pack <| function
    | Word(className, (Int(level, rest))) ->
      match classData |> List.tryFind (fun (_, stringRep, _) -> System.String.Equals(stringRep, className, System.StringComparison.InvariantCultureIgnoreCase)) with
      | Some((id, _, _)) -> Some({ Class = id; Level = level }, rest)
      | _ -> None
    | _ -> None

  let commandSeparator = Set<_>[';']

  let (|Command|_|) = pack <| function
    | Str "new" rest | Str "begin" rest -> Some(NewContext, rest)
    | Str "name" (WS (CharsExcept commandSeparator (name, rest))) -> Some( SetName <| name.Trim(), rest)
    | Words (AnyCase("rollstats" | "roll stats" | "roll"), rest) -> Some(RollStats, rest)
    | Str "assign" (Ints(stats, rest)) when stats.Length = 6 -> // must be 6 numbers that will be interpreted as priorities for stats, in order
      Some(AssignStats stats, rest)
    | Str "save" (Words(fileName, rest)) -> Some(Save (Some fileName), rest)
    | Str "save" rest -> Some(Save None, rest)
    | Str "load" (Words(fileName, rest)) -> Some(Load fileName, rest)
    | Word (AnyCase("swap" | "sw"), (Stat(s1, Stat(s2, rest)))) -> Some(SwapStats(s1, s2), rest)
    | Race(race, rest) -> Some(SetRace race, rest)
    | ClassGoal(classLevel, rest) -> Some(SetClassGoal(classLevel), rest)
    | Int(xp, Word(AnyCase("xp"), rest)) | Word(AnyCase("xp"), (Int(xp, rest))) -> Some(SetXP xp, rest)
    | Str "note" (CharsExcept commandSeparator (txt, rest)) -> Some(AddNote <| txt.Trim(), rest)
    | Str "amendnote" (CharsExcept commandSeparator (txt, rest)) -> Some(AmendNote <| txt.Trim(), rest)
    | Str "clearnotes" rest -> Some(ClearNotes, rest)
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

let recomputeLevelDependentProperties (sb : StatBlock) =
  let levelMax = PCXP.Table |> List.findBack (fun levelReq -> sb.XP >= levelReq.XPRequired) |> fun x -> x.Level
  let x = Con
  let conMod = View.statView Con sb |> combatBonus
  let actualClassLevels, actualLevelsRev =
    sb.IntendedLevels @ [{ ClassLevel.Class = Fighter; Level = 20}] // default to fighter 20 as goal
    |> List.fold (
      fun (consumed: Map<_, _>, accum: ClassLevel list) classLevel ->
        let consumption = consumed |> Seq.sumBy(function KeyValue(_, v) -> v)
        if consumption >= levelMax then (consumed, accum)
        else
          let prevLevel = (match consumed |> Map.tryFind classLevel.Class with Some(prevLevel) -> prevLevel | _ -> 0)
          let consuming = classLevel.Level - prevLevel
          if consumption + consuming <= levelMax then
            (consumed |> Map.add classLevel.Class classLevel.Level), (classLevel :: accum)
          else // consume as much as possible
            let classLevel = { classLevel with Level = levelMax - (consumption - prevLevel) }
            (consumed |> Map.add classLevel.Class classLevel.Level), classLevel :: accum
      ) (Map.empty, [])
  let actualLevels = actualLevelsRev |> List.rev
  let classHp classId =
    classData |> List.sumBy(fun (id, _, hp) -> if classId = id then hp else 0)
  let hp =
    actualClassLevels
    |> Seq.sumBy (
        function
          KeyValue(classId, level) ->
            let hp =
              level * (classHp classId + conMod) +
                (if actualLevels.Head.Class = classId then classHp classId - 2 else 0)
            hp
        )
  let retval = {
    sb with
        HP = hp
        Levels = actualClassLevels
                  |> Seq.sortBy (function KeyValue(classId, level) -> actualLevels |> List.findIndex (fun x -> x.Class = classId))
                  |> Seq.map (function KeyValue(classId, level) -> { Class = classId; Level = level })
                  |> List.ofSeq
    }
  retval

let view (state: State) =
  let statBlock = Lens.view Current state
  let classToString (cl:ClassLevel) =
    classData |> List.find (fun (id, _, _) -> id = cl.Class) |> fun (_, stringRep, _) -> stringRep
  let stats =
    (statData
    |> List.map (fun (id, stringRep, _) -> sprintf "%s %d" (stringRep.Substring(0,3)) (statBlock |> View.statView id)))
    @ [sprintf "HP %i XP %i" statBlock.HP statBlock.XP]
    |> String.join ", "
  [
    (sprintf "Name: %s" (Lens.view Prop.Current state).Name)
    [
      (match statBlock.Race with
          | Some({ Name = name; Trait = Some(traitName) }) -> [(sprintf "%s [%s]" name traitName)]
          | Some(r) -> [r.Name]
          | None -> [])
      statBlock.Levels |> List.map (fun classLevel -> sprintf "%s %i" (classToString classLevel) classLevel.Level)
      [statBlock.IntendedLevels
        |> List.map (fun classLevel ->
            // Order N^2 rendering operation, but should be okay. Look up any other classLevels that are strictly less.
            match statBlock.IntendedLevels |> List.filter (fun cl' -> cl'.Class = classLevel.Class && cl'.Level < classLevel.Level) with
            | cl'::_ when cl'.Level + 1 = classLevel.Level ->
              sprintf "%s %i" (classToString cl') (classLevel.Level)
            | [] when classLevel.Level = 1 ->
              sprintf "%s %i" (classToString classLevel) (classLevel.Level)
            | cl'::_ ->
              sprintf "%s %i-%i" (classToString cl') (cl'.Level + 1) (classLevel.Level)
            | _ ->
              sprintf "%s 1-%i" (classToString classLevel) (classLevel.Level)
          )
        |> String.join "; " |> sprintf "[%s]"]
      ]
      |> List.concat
      |> String.join " "
    stats
  ]
  @ statBlock.Notes
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
          let statsInOrder = statData |> List.map (fun (_, _, v) -> Current << StatArray << v)
          let statValues = statsInOrder |> List.map (flip Lens.view state) |> List.sortDescending
          let statsByPriority = statsInOrder |> List.mapi (fun i prop -> order.[i], prop) |> List.sortBy fst
          statsByPriority
          |> List.mapi (fun i (_, prop) -> i, prop) // now that they're in order of priority, match each one up with a unique statValue index
          |> List.fold (fun state (ix, prop) -> state |> Lens.set prop statValues.[ix]) state
        | SwapStats(s1, s2) when hasCurrent ->
          let lenses = [s1;s2] |> List.map statLens
          let vals = lenses |> List.map (fun l -> Lens.view l state) |> List.rev
          (vals |> List.zip lenses) |> List.fold (fun st (l, v) -> Lens.set l v st) state
        | SetRace(race) when hasCurrent ->
          state |> Lens.over Prop.Current (fun st ->
            { st with
                Race = race
            })
        | AddNote(note) when hasCurrent ->
          state |> Lens.over Prop.Current (fun st ->
            { st with
                Notes = st.Notes @ [note]
            })
        | AmendNote(note) when hasCurrent ->
          state |> Lens.over Prop.Current (fun st ->
            let lastIx = st.Notes.Length - 1
            { st with
                Notes = st.Notes |> List.mapi (fun i x -> if i = lastIx then note else x)
            })
        | ClearNotes when hasCurrent ->
          state |> Lens.over Prop.Current (fun st ->
            { st with
                Notes = []
            })
        | SetXP(xp) when hasCurrent ->
          state |> Lens.over Prop.Current (fun st ->
            { st with
                XP = xp
            })
        | SetClassGoal(classLevel) when hasCurrent ->
          state |> Lens.over Prop.Current (fun st ->
            let currentMaxLevel =
              let classLevels = st.IntendedLevels |> List.filter (fun cl -> cl.Class = classLevel.Class)
              if classLevels = [] then 0
              else classLevels
                    |> List.maxBy (fun cl -> cl.Level)
                    |> fun x -> x.Level
            let lev =
              let newMaxLevel = classLevel.Level
              if newMaxLevel < currentMaxLevel then
                // delete now-redundant levels
                st.IntendedLevels |> List.fold (fun (alreadyAtMax, accum: ClassLevel list) cl' ->
                    match cl' with
                    | { Class = className; Level = l }
                      when className = classLevel.Class
                            && newMaxLevel <= 0 ->
                        (true, accum)
                    | { Class = className; Level = l }
                      when (className = classLevel.Class
                            && l > newMaxLevel) ->
                        // trim or delete, depending on whether or not there is a previous entry
                        (true, if alreadyAtMax then accum else { cl' with Level = newMaxLevel }::accum)
                    | v -> (alreadyAtMax, cl'::accum)
                  ) (false, [])
                  |> snd
                  |> List.rev
              else
                match st.IntendedLevels |> List.rev with
                  | h::rest when h.Class = classLevel.Class ->
                    classLevel :: rest
                  | rest -> classLevel :: rest
                |> List.rev
            // cap total levels at 20
            let lev =
              lev
              |> List.mapFold (fun (sums : Map<_, _>) cl' ->
                      let sum = Seq.sumBy (function KeyValue(_, level) -> level)
                      if (sum sums) > 20 then
                        [], sums
                      else
                        let sums' = (sums |> Map.add cl'.Class cl'.Level)
                        if sum sums' <= 20 then
                          [cl'], sums |> Map.add cl'.Class cl'.Level
                        else
                          let cl' = { cl' with Level = cl'.Level - (sum sums' - 20) }
                          [cl'], sums |> Map.add cl'.Class cl'.Level
                          ) Map.empty
              |> fst
              |> List.concat

            { st with
                IntendedLevels = lev
            })
        | Save(fileName) when hasCurrent ->
          this.IO.save (defaultArg fileName (Lens.view Current state).Name) (Lens.view Current state)
          state
        | Load(fileName) ->
          match this.IO.load fileName with
          | Some(newChar) ->
            Lens.set Current newChar state
          | None -> state
    state <- state |> Lens.over Prop.Current (recomputeLevelDependentProperties)
    view state |> this.UpdateStatus
  member this.Execute(cmd) = this.Execute [cmd]
  new() =
    let random = System.Random()
    StatBank(resolve <| fun x -> 1 + random.Next(x))
