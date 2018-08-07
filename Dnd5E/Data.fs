/// Shared data structures, especially ones that get serialized to JSON
module Froggy.Data

open Froggy
open Froggy.Common

module Roll =
  open System.Numerics
  open Froggy.Common

  type Predicate = AtLeast of int | AtMost of int | Natural of min: int * max: int | Else
  module Predicate =
    let eval = function
      | AtLeast rhs -> fun (r, m) -> r + m >= rhs
      | AtMost rhs -> fun (r, m) -> r + m <= rhs
      | Natural(min, max) -> fun (r, _) -> betweenInclusive min max r
      | Else -> thunk true

  type Transform = Div of int | Times of int
  module Transform =
    let eval = function
      | Div x -> flip (/) x
      | Times x -> (*) x
  type Aggregation = Sum | Max | Min
  type Request =
    | Dice of n:int * die:int
    | StaticValue of n:int
    | Combine of Aggregation * AggregateRequest
    | Branch of baseRollPlusMods: (Request * Request) * branches: (Predicate * Request) list
    | Transform of Request * tranform: Transform
  and AggregateRequest =
    | Aggregate of Request list
    | Repeat of n: int * Request
    | Best of n:int * AggregateRequest
  type ResultValue = int
  type Result =
    { value: ResultValue; source: Request; sublog: Result list }
  and AggregateResult =
    { value: Result list; source: AggregateRequest; sublog: Result list }
  module Result =
    let getValue (r: Result) = r.value
  module AggregateResult =
    let getValue (r: AggregateResult) = r.value
  type DistributionResult =
    DistributionResult of Map<ResultValue, BigInteger>
  type FractionalResult = float

  // Some convenience values and functions
  let d20 = Dice(1,20)
  let adv bonus = Combine(Max, Repeat(2, d20)), StaticValue bonus
  let disadv bonus = Combine(Min, Repeat(2, d20)), StaticValue bonus
  let normal bonus = d20, StaticValue bonus
  let Crit = Natural(20, 20)
  let d nDice dieSize bonus = Combine(Sum, Aggregate[Dice(nDice, dieSize); StaticValue bonus])
  // multiplies the dice that actually get rolled, e.g. for crits
  let multiplyResultDice n roll =
    let rec mapRoll = function
      | Dice(nDice, d) -> Dice(nDice * n, d)
      | StaticValue _ as r -> r
      | Combine(op, agg) -> Combine(op, mapAgg (agg))
      | Branch(test, branches) -> Branch(test, branches |> List.map (fun (pred, roll) -> (pred, mapRoll roll)))
      | Transform(roll, t) -> Transform(mapRoll roll, t)
    and mapAgg = function
      | Aggregate rs -> Aggregate (rs |> List.map mapRoll)
      | Repeat(n, roll) -> Repeat (n, mapRoll roll)
      | Best(n, agg) -> Best(n, mapAgg agg)
    mapRoll roll
  let doubleDice = multiplyResultDice 2

  let rec evaluate (r: int -> int) roll =
    let toResult priors v = { Result.value = v; source = roll; sublog = priors }
    match roll with
    | Dice(nDice, d) -> seq { for _ in 1..nDice -> r d } |> Seq.sum |> toResult []
    | StaticValue v -> v |> toResult []
    | Combine(op, aggregate) ->
      let vResult = evaluateAggregate r aggregate
      let vs = vResult.value |> List.map Result.getValue
      match op with
      | Sum -> vs |> List.sum |> toResult vResult.sublog
      | Max -> vs |> List.max |> toResult vResult.sublog
      | Min -> vs |> List.min |> toResult vResult.sublog
    | Branch((baseRoll, modsRoll), branches) ->
      let bResult = evaluate r baseRoll
      let mResult = evaluate r modsRoll
      let b,mods = bResult.value, mResult.value
      let rec evalBranch = function
        | [] -> 0 |> toResult [bResult;mResult; evaluate r (StaticValue 0)] // fallback case
        | (condition, roll)::rest ->
          if Predicate.eval condition (b, mods) then
            let toResult (v:Result) =
              v.value |> toResult [bResult;mResult;v]
            evaluate r roll |> toResult
          else
            evalBranch rest
      evalBranch branches
    | Transform(roll, t) ->
      let v = evaluate r roll
      v.value |> Transform.eval t |> toResult [v]
  and evaluateAggregate (r: int -> int) aggregate =
    let toResult priors v = { AggregateResult.value = v; source = aggregate; sublog = priors }
    match aggregate with
    | Aggregate rs ->
      let priors = rs |> List.map (evaluate r)
      priors |> toResult priors // all of the priors are included in the results
    | Repeat(n, roll) ->
      let priors = [for _ in 1..n -> evaluate r roll]
      priors |> toResult priors // all of the priors are included in the results
    | Best(n, agg) ->
      let priors = (evaluateAggregate r agg).value
      let results = priors |> List.sortByDescending Result.getValue |> List.take n
      // finally, a case where some priors are excluded from results!
      results |> toResult priors

  let eval = evaluate rollOneDie // convenience method for ubiquitous case
  let distribution (roll:Request) : DistributionResult =
    let distributionOfList valuesAndCounts =
      let add distr (v: ResultValue, count:BigInteger) =
        match Map.tryFind v distr with
        | Some(count') -> Map.add v (count + count') distr
        | None -> Map.add v count distr
      valuesAndCounts |> List.fold add Map.empty
    let combine op lhs rhs =
      let distributionValues =
        // compute the probability of each value based on the probability of its inputs
        [for KeyValue(lval, lcount) in lhs do
          for KeyValue(rval, rcount) in rhs do
            yield (op lval rval), lcount * rcount
          ]
      // now need to group counts by value
      distributionValues |> distributionOfList
    let sum = combine (+)
    let rec dist = function
      | Dice(n, d) ->
        let oneDie = [for i in 1..d -> i, 1I] |> Map.ofSeq
        let allDice = List.init n (thunk oneDie)
        List.reduce sum allDice
      | StaticValue v -> Map<_,_>[v, 1I]
      | Combine(op, agg) ->
        let f =
          match op with
          | Sum -> (+)
          | Min -> min
          | Max -> max
        distAgg agg |> List.reduce (combine f)
      | Branch((b,m), branches) ->
        let bdist, mdist = dist b, dist m
        // we group by rolls BEFORE computing distributions
        // because rolls are smaller and ought to be more performant,
        // and we're trying to avoid computing and summing distributions
        // more often than necessary
        let rollCounts =
          [for KeyValue(bv, bcount) in bdist do
            for KeyValue(mv, mcount) in mdist do
              let testVar = (bv, mv)
              let count = bcount * mcount
              match branches |> List.tryFind (fun (cond, roll) -> Predicate.eval cond testVar) with
              | Some(_, roll) ->
                yield roll, count
              | None ->
                yield StaticValue 0, count
            ]
          |> Seq.groupBy fst
          |> Seq.map (fun (roll, rollCounts) -> roll, (rollCounts |> Seq.sumBy snd))
          |> Map.ofSeq
        let rollDistributionSize =
          rollCounts |> Seq.sumBy (fun (KeyValue(_roll,vSize)) -> vSize)
        let rollDistributions =
          rollCounts
          |> Map.map (fun roll count ->
                        let d = dist roll
                        let vSize = d |> Seq.sumBy (function KeyValue(_roll, count) -> count)
                        d, count, vSize)
        // the joint distribution size is the product of all the individual roll distributions
        let jointDistributionSize =
          rollDistributions |> Seq.fold (fun product (KeyValue(_roll,(_, _rollFrequency, vSize))) -> vSize * product) rollDistributionSize
        [for KeyValue(roll, (dist, rollFrequency, vSize)) in rollDistributions do
          let rollWeight = jointDistributionSize * rollFrequency / rollDistributionSize // number of total outcomes which belong to this set of rolls
          let valueWeight = rollWeight / vSize // normalization factor so that dist will sum to rollWeight
          for KeyValue(v, vcount) in dist do
            yield v, valueWeight * vcount
          ]
        |> distributionOfList
      | Transform(roll, trans) ->
        let t = Transform.eval trans
        [for KeyValue(v, count) in (dist roll) do
            yield (t v), count
          ]
        |> distributionOfList
    and distAgg = function
      | Aggregate rs -> rs |> List.map dist
      | Repeat(n, roll) -> [for _ in 1..n -> dist roll]
      | Best(n, agg) ->
        let dists = distAgg agg
        let sizeOf dist =
          dist |> Seq.sumBy (fun (KeyValue(_, count)) -> count)
        let jointDistributionSize = dists |> List.fold (fun product dist -> sizeOf dist) 1I
        let bestOf n (lst: ResultValue list) =
          lst |> List.sortDescending |> List.take n |> List.sum
        let cross combine x y =
          seq {
            for x' in x do
              for y' in y do
                yield (combine x' y') }
        let crossAll combine initial lst =
          lst |> List.fold (fun st dist -> cross combine dist st) initial
        let combine (KeyValue(v1, vcount)) (vs, count) =
          v1::vs, count * vcount
        crossAll combine [[], 1I] dists
          |> Seq.map (fun (vs, count) -> (bestOf n vs), count)
          |> List.ofSeq
          |> distributionOfList
          |> List.singleton // bestOf produces only one output stream
    dist roll |> DistributionResult

  let mean (roll:Request) =
    match distribution roll with
    | DistributionResult(dist) ->
      let count, total =
        [for KeyValue(v, count) in dist -> count, (BigInteger v) * count]
        |> List.reduce (fun (count, total) (count', total') -> count + count', total + total')
      Fraction.ratio 3 total count

  module Grammar =
    open Froggy.Packrat
    let rec (|SumOfSimpleRolls|_|) = pack <| function
      | SumOfSimpleRolls(lhs, OWS(Char('+', OWS(SimpleRoll(r, rest))))) -> Some(lhs@[r], rest)
      | SumOfSimpleRolls(lhs, OWS(Char('+', OWS(Int(n, rest))))) -> Some(lhs@[StaticValue n], rest)
      | SumOfSimpleRolls(lhs, OWS(Char('-', OWS(Int(n, rest))))) -> Some(lhs@[StaticValue -n], rest)
      | SimpleRoll(roll, rest) -> Some([roll], rest)
      | _ -> None
    and (|SimpleRoll|_|) = pack <| function
      | Int(n, Char ('d', Int(d, Char ('k', Int(m, rest))))) -> Some (Combine(Sum, Best(m, (Repeat(n, Dice(1, d))))), rest)
      | Int(n, Char ('d', Int(d, rest))) -> Some (Dice(n, d), rest)
      | Int(n, Char ('d', rest)) -> Some (Dice(n, 6), rest)
      | OWS(Char ('d', Int(d, Char('a', rest)))) -> Some (Combine(Max, Repeat(2, Dice(1,d))), rest)
      | OWS(Char ('d', Int(d, Char('d', rest)))) -> Some (Combine(Min, Repeat(2, Dice(1,d))), rest)
      | OWS(Char ('d', Int(d, rest))) -> Some (Dice(1,d), rest)
      | Int(n, rest) -> Some(StaticValue n, rest)
      | _ -> None
    let (|RollsWithModifiers|_|) = pack <| function
      | SumOfSimpleRolls([v], rest) -> Some(v, rest)
      | SumOfSimpleRolls(rolls, rest) -> Some(Combine(Sum, Aggregate rolls), rest)
      | _ -> None
    let rec (|CommaSeparatedRolls|_|) = pack <| function
      | CommaSeparatedRolls(rolls, OWS(Str "," (OWS (Roll(r, rest))))) -> Some(rolls @ [r], rest)
      | Roll(r, rest) -> Some([r], rest)
      | _ -> None
    and (|PlusSeparatedRolls|_|) = pack <| function
      | PlusSeparatedRolls(rolls, OWS(Str "+" (OWS (Roll(r, rest))))) -> Some(rolls @ [r], rest)
      | Roll(r, rest) -> Some([r], rest)
      | _ -> None
    and (|NumericBonus|_|) = pack <| function
      | OWS(Optional "+" (Chars numeric (v, OWS(rest)))) ->
        match System.Int32.TryParse(v) with
        | true, v -> Some(v, rest)
        | _ -> None
      | OWS(Str "-" (Chars numeric (v, OWS(rest)))) ->
        match System.Int32.TryParse(v) with
        | true, v -> Some(-v, rest)
        | _ -> None
      | _ -> None
    and (|Attack|_|) = pack <| function
      // multiple shorthands for specifying advantage and disadvantage
      | Word(AnyCase("att" | "attack"), Int(ac, Char('a', Roll(dmg, rest)))) -> Some(Branch(adv 0, [Crit, doubleDice dmg; AtLeast ac, dmg]), rest)
      | Word(AnyCase("att" | "attack"), Int(ac, Char('d', Roll(dmg, rest)))) -> Some(Branch(disadv 0, [Crit, doubleDice dmg; AtLeast ac, dmg]), rest)
      | Word(AnyCase("att" | "attack"), Int(ac, Char('a', NumericBonus(toHit, Roll(dmg, rest))))) -> Some(Branch(adv toHit, [Crit, doubleDice dmg; AtLeast ac, dmg]), rest)
      | Word(AnyCase("att" | "attack"), Int(ac, Char('d', NumericBonus(toHit, Roll(dmg, rest))))) -> Some(Branch(disadv toHit, [Crit, doubleDice dmg; AtLeast ac, dmg]), rest)
      | Word(AnyCase("att" | "attack"), Int(ac, Roll(dmg, rest))) -> Some(Branch(normal 0, [Crit, doubleDice dmg; AtLeast ac, dmg]), rest)
      | Word(AnyCase("att" | "attack"), Int(ac, NumericBonus(toHit, Char('a', Roll(dmg, rest))))) -> Some(Branch(adv toHit, [Crit, doubleDice dmg; AtLeast ac, dmg]), rest)
      | Word(AnyCase("att" | "attack"), Int(ac, NumericBonus(toHit, Char('d', Roll(dmg, rest))))) -> Some(Branch(disadv toHit, [Crit, doubleDice dmg; AtLeast ac, dmg]), rest)
      | Word(AnyCase("att" | "attack"), Int(ac, NumericBonus(toHit, Roll(dmg, rest)))) -> Some(Branch(normal toHit, [Crit, doubleDice dmg; AtLeast ac, dmg]), rest)
      | _ -> None
    and (|TestVariable|_|) =
      let toBaseMods = function
        | Combine(Sum, AggregateRequest.Aggregate [b; mods]) -> b, mods // optimize this representation
        | r -> r, StaticValue 0
      pack <| function
      | Int(n, Str "?" rest) -> Some((normal 0, AtLeast n), rest)
      | Int(n, Str "a?" rest) -> Some((adv 0, AtLeast n), rest)
      | Int(n, Str "d?" rest) -> Some((disadv 0, AtLeast n), rest)
      | Str "(" (Roll(r, Word("at", Word("least", Int(n, Str ")?" rest))))) -> Some((r |> toBaseMods, AtLeast n), rest)
      | Str "(" (Roll(r, Word("at", Word("most", Int(n, Str ")?" rest))))) -> Some((r |> toBaseMods, AtMost n), rest)
      | _ -> None
    and (|Branch|_|) = pack <| function
      | TestVariable((tv,condition), Roll(r1, Str ":"(Roll(r2, rest)))) -> Some(Request.Branch(tv, [condition, r1; Else, r2]), rest)
      | TestVariable((tv,condition), Roll(r, rest)) -> Some(Request.Branch(tv, [condition, r]), rest)
      | TestVariable((tv,condition), rest) -> Some(Request.Branch(tv, [condition, StaticValue 1]), rest)
      | _ -> None
    and (|Repeat|_|) = pack <| function
      | Int(n, Str "." (Roll(r, rest))) -> Some(AggregateRequest.Repeat(n, r), rest)
      | Int(n, Str "x" (Roll(r, rest))) -> Some(AggregateRequest.Repeat(n, r), rest)
      | _ -> None
    and (|Aggregate|_|) = pack <| function
      | CommaSeparatedRolls(rolls,  rest) when rolls.Length >= 2 -> Some(AggregateRequest.Aggregate rolls, rest)
      | _ -> None
    and (|Best|_|) = pack <| function
      | Word (AnyCase "best", (Int(n, Word(AnyCase "of", Aggregatation(rolls, rest))))) -> Some(AggregateRequest.Best(n, rolls), rest)
      | _ -> None
    and (|Aggregatation|_|) = pack <| function
      | Best(rolls, rest) -> Some(rolls, rest)
      | Repeat(r, rest) -> Some(r, rest)
      | Aggregate(r, rest) -> Some(r, rest)
      | _ -> None
    and (|Roll|_|) = pack <| function
      | Roll(r, Str "/" (Int(rhs, rest))) -> Some(Transform(r, Div rhs), rest)
      | Roll(r, Str "*" (Int(rhs, rest))) -> Some(Transform(r, Times rhs), rest)
      | PlusSeparatedRolls(rolls, rest) when rolls.Length >= 2 -> Some(Combine(Sum,Aggregate rolls), rest)
      | Repeat(rolls, rest) -> Some(Combine(Sum, rolls), rest)
      | Best(rolls, rest) -> Some(Combine(Sum, rolls), rest)
      | Branch(r, rest) -> Some(r, rest)
      | RollsWithModifiers(r, rest) -> Some(r, rest)
      | Str "(" (OWS (Roll(r, OWS(Str ")" rest)))) -> Some(r, rest)
      | Word(AnyCase("max"), Str "(" (Aggregatation(rolls, (Str ")" rest)))) ->
        Some(Combine(Max, rolls), rest)
      | Word(AnyCase("min"), Str "(" (Aggregatation(rolls, (Str ")" rest)))) ->
        Some(Combine(Min, rolls), rest)
      | Attack(roll, rest) -> Some(roll, rest)
      | _ -> None

type StatId = Str | Dex | Con | Int | Wis | Cha
type ClassId = Bard | Barbarian | Cleric | Druid | Fighter | Monk | Paladin | Ranger | Rogue | Sorcerer | Warlock | Wizard
type ClassData = { Id: ClassId; StringRep: string; AverageHP: int; Subclasses: string list }
  with
  static member Table =
    [
      // ClassId, string rep, average HP
      Bard, "Bard", 5, ["Lore Bard"; "Valor Bard"]
      Barbarian, "Barbarian", 7, ["Barbearian"; "Ancestor Barbarian"; "Zealot"; "Berserker"; "Battlerager"]
      Cleric, "Cleric", 5, ["Knowledge Cleric"; "Life Cleric"; "Light Cleric"; "Nature Cleric"; "Tempest Cleric"; "Trickery Cleric"; "War Cleric"; "Death Cleric"; "Arcana Cleric"; "Forge Cleric"; "Grave Cleric"]
      Druid, "Druid", 5, ["Moon Druid"; "Shepherd Druid"; "Dream Druid"]
      Fighter, "Fighter", 6, ["Eldritch Knight"; "Battlemaster"; "Champion"; "Cavalier"; "Samurai"; "Purple Dragon Knight"; "Banneret"]
      Monk, "Monk", 5, ["Shadow Monk"; "Long Death Monk"; "Open Hand Monk"; "Elemental Monk"]
      Paladin, "Paladin", 6, ["Paladin of Devotion"; "Paladin of Ancients"; "Paladin of Vengeance"; "Paladin of Conquest"]
      Ranger, "Ranger", 6, ["Hunter"; "Beastmaster"; "Gloomstalker"]
      Rogue, "Rogue", 5, ["Thief"; "Assassin"; "Swashbuckler"; "Mastermind"; "Inquisitive"]
      Sorcerer, "Sorcerer", 4, ["Wild Mage"; "Shadow Sorcerer"; "Divine Soul"; "Dragon Sorcerer"]
      Wizard, "Wizard", 4, ["Abjuror"; "Conjuror"; "Diviner"; "Evoker"; "Enchanter"; "Illusionist"; "Necromancer"; "Transmuter"; "Bladesinger"; "War Mage"]
      Warlock, "Warlock", 5, ["Fiendlock"; "Celestialock"; "Cthulock"; "Feylock"; "Deathlock"; "Hexblade"; "Blade Pact"; "Tome Pact"; "Chain Pact"; "Bladelock"; "Tomelock"; "Chainlock"]
    ]
    |> List.map (fun (id, str, hp, subclasses) -> { Id = id; StringRep = str; AverageHP = hp; Subclasses = subclasses })

type StatArray = {
    Str: int
    Dex: int
    Con: int
    Int: int
    Wis: int
    Cha: int
  }

type StatMod = { Stat: StatId; Bonus: int }
type RaceData = { Name: string; Mods: StatMod list; Trait: string option }

type ClassLevel = { Id: ClassId; Level: int }

type CharSheet = {
    Name : string
    Stats: StatArray
    HP: int
    Race: RaceData option
    XP: int
    Levels: ClassLevel list
    IntendedLevels: ClassLevel list
    Subclasses: (ClassId * string) list
    Notes: string list
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
    Race = None
    HP = 1
    XP = 0
    Levels = []
    IntendedLevels = []
    Notes = []
    Subclasses = []
  }

type Party = {
    Current: int option
    Party: CharSheet list
  }
  with
  static member Empty = { Current = None; Party = [] }

type PCXP = { Level: int; XPRequired: int }
  with
  static member Table =
    [
      // level, XP required, daily XP budget, easy, medium, hard, deadly
      0, 0
      1, 0
      2, 300
      3, 900
      4, 2700
      5, 6500
      6, 14000
      7, 23000
      8, 34000
      9, 48000
      10, 64000
      11, 85000
      12, 100000
      13, 120000
      14, 140000
      15, 165000
      16, 195000
      17, 225000
      18, 265000
      19, 305000
      20, 355000
      ] |> List.map (fun (level, xp) -> { Level = level; XPRequired = xp })

let combatBonus statVal =
  (statVal/2) - 5
let skillBonus statVal =
  ((statVal+1)/2) - 5

type MonsterTemplate = { TypeName : string; Namelist: string[]; HP: Roll.Request }

module Properties =

  type PropertyName = string
  type PropertyValue = Text of string | Number of int
  let asNumber = function Number n -> n | v -> failwithf "Invalid cast: %A is not a number" v
  let asText = function Text n -> n | v -> failwithf "Invalid cast: %A is not text" v

  [<AbstractClass>]
  type Property(name : PropertyName, origin : (CharSheet -> PropertyValue) option, output : (PropertyValue -> CharSheet -> CharSheet) option, fromTemplate: (MonsterTemplate -> PropertyValue) option) =
    member this.Name = name
    member this.Origin = origin
    member this.Output = output
    member this.FromTemplate = fromTemplate
    abstract member TryParse: string -> PropertyValue option
  type NumberProperty(name, ?origin : (CharSheet -> PropertyValue), ?output : (PropertyValue -> CharSheet -> CharSheet), ?fromTemplate: (MonsterTemplate -> PropertyValue)) =
    inherit Property(name, origin, output, fromTemplate)
    override this.TryParse input =
      match System.Int32.TryParse input with
      | true, v -> Some <| Number v
      | _ -> None
  type TextProperty(name, ?origin : (CharSheet -> PropertyValue), ?output : (PropertyValue -> CharSheet -> CharSheet), ?fromTemplate: (MonsterTemplate -> PropertyValue)) =
    inherit Property(name, origin, output, fromTemplate)
    override this.TryParse input = Some <| Text input

  let Name = TextProperty("Name", origin = fun sb -> Text sb.Name)
  let HP = NumberProperty("HP", origin = (fun sb -> Number sb.HP), fromTemplate = (fun t -> t.HP |> Roll.eval |> Roll.Result.getValue |> Number))
  let SP = NumberProperty("SP", origin = fun sb -> Number sb.HP)
  let XP = NumberProperty("XP", origin = (fun sb -> Number sb.XP), output = fun pv sb -> { sb with XP = asNumber pv })
  let Properties =
    ([ Name; HP; SP; XP ] : Property list)
    |> List.map (fun t -> t.Name, t)
    |> Map.ofList

module AdventureData =
  open Properties
  open Data

  type Id = int
  type StatBank = Map<Id*PropertyName, PropertyValue>
  type Data = {
    roster: Map<string, Id>
    reverseRoster: Map<Id, string>
    mapping: StatBank
    properties: Map<string, Property>
  }
  with
    static member Empty = { roster = Map.empty; reverseRoster = Map.empty; mapping = Map.empty; properties = Properties }

  // gets value from the user
  let acquireValue (query: string -> string) (name: string) (prop: Property) =
    let response = query (sprintf "What is %s's %s?" name prop.Name)
    let rec getPropertyValue (response: string) =
      match prop.TryParse response with
      | Some v -> v
      | None ->
        getPropertyValue (query (sprintf "Sorry, I didn't understand '%s'.\nWhat is %s's %s?" response name prop.Name))
    getPropertyValue response

  let lookup query (prop: Property) id (data : Data) =
    match Map.tryFind (id, prop.Name) data.mapping with
    | Some(v) -> v, data
    | None ->
      let v = acquireValue query data.reverseRoster.[id] prop
      v, { data with mapping = data.mapping |> Map.add (id, prop.Name) v }

  type Command =
    | Set of Property * PropertyValue * Id
    | Increment of NumberProperty * int * Id
    | Deduct of NumberProperty * int * Id

type GameState = { party: Party; adventure: Froggy.Data.SimpleProperties.SimpleStore option; monsterTemplates: Froggy.Data.SimpleProperties.SimpleStore option ref }
module GameState =
  let Empty = { party = { Party.Empty with Current = Some 0; Party = [CharSheet.Empty] }; adventure = None; monsterTemplates = ref None }
