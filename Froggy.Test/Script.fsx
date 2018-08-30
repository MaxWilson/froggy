// Trial code here for Shining Sword economic model
#I __SOURCE_DIRECTORY__
#load @"..\Froggy\Common.fs"
#nowarn "0040"
#load @"..\Froggy\Packrat.fs"
#load @"..\Dnd5E\Data.fs"

open Froggy.Data
open System.Windows.Forms

type Hotspot = { intensity: float; discovered: bool; distance: int }
type Parameters = { shortTermDamage: Roll.Request; longTermDamage: Roll.Request; hotspotGrowthRate: float; reward: Roll.Request; hotspotDistance: Roll.Request; noiseFactor: float }
type Problem = { name: string; parameters: Parameters; sourceIntensity: int; sourceDiscovered: bool; hotspots: Hotspot list }
type World = { day: int; treasury: int; upkeep: int; lastIncome: int; shortTermPenalty: int; growthPenalty: int; problems: Problem list }

let incomeGrowth income hiddenPenalty = int (float income * 1.3) - hiddenPenalty

let tick world =
  let updateHotspot parameters hotspot =
    let h =
      { hotspot with intensity = hotspot.intensity + parameters.hotspotGrowthRate }
    if h.discovered || (Roll.eval(Roll.Dice(1,100)).value |> float) > 5. * parameters.noiseFactor then
      h
    else
      { h with discovered = true }
  let updateProblem problem =
    let p = problem.parameters
    let hotspots = problem.hotspots |> List.map (updateHotspot p)
    let hotspots =
      if Roll.eval(Roll.Dice(1, 10)).value > problem.sourceIntensity then hotspots
      else { intensity = 1.; discovered = false; distance = (Roll.eval p.hotspotDistance).value }::hotspots
    if problem.sourceDiscovered || (Roll.eval(Roll.Dice(1,100)).value |> float) > 1. * p.noiseFactor then
      { problem with hotspots = hotspots }
    else
      { problem with hotspots = hotspots; sourceDiscovered = true }
  let penalties = [
    for problem in world.problems do
      for hotspot in problem.hotspots do
        let i = hotspot.intensity |> int
        let penalty = Roll.eval (Roll.Combine(Roll.Sum,Roll.Repeat(i, problem.parameters.shortTermDamage)))
        yield penalty.value
    ]
  let longtermPenalty =
    if world.day % 7 > 0 then 0
    else
      [
      for problem in world.problems do
        for hotspot in problem.hotspots do
          let i = hotspot.intensity |> int
          let penalty = Roll.eval (Roll.Combine(Roll.Sum,Roll.Repeat(i, problem.parameters.longTermDamage)))
          yield penalty.value
      ]
      |> List.sum
  let world =
    { world
      with
        day = world.day + 1
        shortTermPenalty = world.shortTermPenalty + (penalties |> List.sum)
        growthPenalty = world.growthPenalty + longtermPenalty
        problems = world.problems |> List.map updateProblem }
  if world.day % 30 > 0 then world
  else
    let income = incomeGrowth world.lastIncome world.growthPenalty
    let actualIncome = income - world.shortTermPenalty
    let net = actualIncome - world.upkeep
    { world
      with
        shortTermPenalty = 0
        growthPenalty = 0
        treasury = world.treasury + net
        lastIncome = income
    }

let world = {
  day = 0;
  treasury = 2000
  upkeep = 800
  lastIncome = 2000
  shortTermPenalty = 0
  growthPenalty = 0
  problems = [
    { name = "Orcs"
      parameters = { shortTermDamage = Roll.d 3 6 0; longTermDamage = Roll.StaticValue 1; hotspotGrowthRate = 1.; reward = Roll.Dice(3,6); hotspotDistance = Roll.Dice(1,6); noiseFactor = 3. };
      sourceIntensity = 1; sourceDiscovered = false; hotspots = [{ intensity = 1.; discovered = true; distance = 0 };{ intensity = 1.; discovered = false; distance = 1 };{ intensity = 1.; discovered = false; distance = 1 }] }
    { name = "Mummies"
      parameters = { shortTermDamage = Roll.d 1 6 0; longTermDamage = Roll.StaticValue 5; hotspotGrowthRate = 1./7.; reward = Roll.Dice(3,6); hotspotDistance = Roll.Dice(1,6); noiseFactor = 1. };
      sourceIntensity = 1; sourceDiscovered = false; hotspots = [] }
    ]
  }

let repeat f report n arg =
  let rec loop i arg =
    report arg
    if i < n then
      loop (i+1) (f arg)
  loop 0 arg

// superman is the ultimate hero, immediately squashes every problem that is discovered
let superman world =
  { world with problems = world.problems |> List.filter (fun p -> if p.sourceDiscovered then printfn "Superman destroys %s HQ" p.name; false else true) |> List.map(fun p -> { p with hotspots = p.hotspots |> List.filter(fun h -> if h.discovered then printfn "Superman kills %s" p.name; false else true) } ) }

let status world =
  let hotspots = [
    for p in world.problems do
      for h in p.hotspots do
        if h.discovered then
          yield sprintf "%s level %d" p.name (h.intensity |> int)
      if p.sourceDiscovered then
        yield sprintf "%s HQ level %d" p.name p.sourceIntensity
    ]
  sprintf "Day %d: %d gp (%d - %d) %s" world.day world.treasury (incomeGrowth world.lastIncome 0) (world.shortTermPenalty) (System.String.Join(", ", hotspots))

repeat (tick >> superman) (status >> printfn "%s") 100 world
