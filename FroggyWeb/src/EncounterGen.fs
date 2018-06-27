#if INTERACTIVE
#else
module EncounterGen
#endif

let monsters = [
  "Wolf", 50
  "Troll", 1800
  "Hill Giant", 1800
  "Orc", 100
  "Hobgoblin", 100
  "Hobgoblin Devastator", 1100
  "Hobgoblin Captain", 700
  "Goblin", 50
  "Earth Elemental", 1800
  "Frost Giant", 3900
  "Winter Wolf", 700
  "Orc War Chief", 1100
  "Orog", 450
  "Young White Dragon", 2300
  "Mind Flayer", 2900
  "Mind Flayer Arcanist", 3900
  "Adult White Dragon", 10000
  "Intellect Devourer", 450
  "Darkling", 100
  "Beholder", 10000
  ]

let monsterParties = [
  ["Orc", 10; "Goblin", 8; "Orog", 3; "Orc War Chief", 1]
  ["Frost Giant", 4; "Winter Wolf", 5; "Troll", 2]
  ["Hobgoblin", 12; "Goblin", 6; "Troll", 1; "Hobgoblin Devastator", 1; "Hobgoblin Captain", 1; "Wolf", 4]
  ["Adult White Dragon", 1; "Young White Dragon", 6]
  ["Young White Dragon", 1; "Mind Flayer", 6; "Mind Flayer Arcanist", 1; "Intellect Devourer", 4; "Goblin", 12]
  ["Beholder", 1; "Darkling", 20; "Goblin", 20]
  ]

let pcmetrics = [
  // level, XP required, daily XP budget, easy, medium, hard, deadly
  0, 0,       0,      0, 0, 0, 0
  1, 0,       300,    25, 50, 75, 100
  2, 300,     600,    50, 100, 150, 200
  3, 900,     1200,   75, 150, 225, 400
  4, 2700,    1700,   125, 250, 375, 500
  5, 6500,    3500,   250, 500, 750, 1100
  6, 14000,   4000,   300, 600, 900, 1400
  7, 23000,   5000,   350, 750, 1100, 1700
  8, 34000,   6000,   450, 900, 1400, 2100
  9, 48000,   7500,   550, 1150, 1600, 2400
  10, 64000,  9000,   600, 1200, 1900, 2800
  11, 85000,  10500,  800, 1600, 2400, 3600
  12, 100000, 11500,  1000, 2000, 3000, 4500
  13, 120000, 13500,  1100, 2200, 3400, 5100
  14, 140000, 15000,  1250, 2500, 3800, 5700
  15, 165000, 18000,  1400, 2800, 4300, 6400
  16, 195000, 20000,  1600, 3200, 4800, 7200
  17, 225000, 25000,  2000, 3900, 5900, 8800
  18, 265000, 27000,  2100, 4200, 6300, 9500
  19, 305000, 30000,  2400, 4900, 7300, 10900
  20, 355000, 40000,  2800, 5700, 8500, 12700
  ]

let xpMultiplier nPCs nMonsters =
  let thresh = [0.5; 1.; 1.5; 2.; 2.5; 3.; 4.; 5.]
  let index =
    if nMonsters <= 1 then 1
    elif nMonsters <= 2 then 2
    elif nMonsters <= 6 then 3
    elif nMonsters <= 10 then 4
    elif nMonsters <= 14 then 5
    else 6
    + if nPCs <= 2 then -1 elif nPCs >= 6 then +1 else 0
  thresh.[index]

let r = System.Random()

let xpBudgets adapt revert pcLevels =
  let e,m,h,d =
    pcLevels
    |> List.fold(fun (e,m,h,d) level ->
        let (_, _, _, easy, medium, hard, deadly) = pcmetrics.[level]
        let adapt = float >> adapt
        (e + adapt easy, m + adapt medium, h + adapt hard, d + adapt deadly)
        )(0.,0.,0.,0.)
  let revert = revert >> int
  [0;revert e; revert m; revert h; revert d]

let xpBudget (xpBudgets: int list) difficulty =
  if difficulty > 4 then xpBudgets.[4] * (difficulty - 3)
  else xpBudgets.[difficulty]

let getXPValue monsterName =
  match monsters |> List.tryFind (fst >> (=) monsterName) with
  | Some(_, v) -> v
  | None -> failwithf "Monster '%s' has no XP cost assigned" monsterName
let calculateCost (pcLevels: _ list) roster = (roster |> List.sumBy(getXPValue) |> float) * (xpMultiplier pcLevels.Length (List.length roster)) |> int

let generate calculateCost monsterParties xpBudgets difficulty =
  let xpBudget = xpBudget xpBudgets difficulty
  let pickFrom (source: (string * int) list) =
    let total = source |> List.sumBy snd
    let normalized = source |> List.fold (fun (prevSum, accum) (name, weight) -> (prevSum + weight), (name, prevSum)::accum) (0, []) |> snd
    let ix = r.Next(total)
    normalized |> List.find(fun (name, minBound) -> ix >= minBound) |> fst
  let rec createRoster source roster =
    let newMonster = pickFrom source
    let roster' = (newMonster :: roster)
    let newCost = calculateCost roster'
    if newCost < xpBudget then createRoster source roster'
    elif newCost = xpBudget || roster = [] then roster' // always have at least one monster
    else
      // we might be over budget. Take extra monster on a probabilistic basis
      let oldCost = calculateCost roster
      let costDiff = newCost - oldCost
      let overage = newCost - xpBudget
      let fractionOver = (float overage/float costDiff)
      if r.NextDouble() <= fractionOver then
        printfn "Decided not to use %s (%f%% probability)" newMonster (100.*fractionOver)
        roster
      else
        printfn "Decided to use %s (%f%% probability)" newMonster (100.*(1.-fractionOver))
        roster'
  let roster =
      createRoster (monsterParties: (string * int) list list).[r.Next(monsterParties.Length)] []
  roster

let pack roster =
  let roster =
      roster
      |> List.groupBy id
      |> List.map (fun (name, lst) -> name, List.length lst)
  roster

let showCost roster cost (xpBudgets: _ list) =
  let actualDifficulty =
    if cost >= xpBudgets.[4] * 2 then sprintf "Deadly x%d" (cost / xpBudgets.[4])
    else ["Trivial";"Easy";"Medium";"Hard";"Deadly"].[xpBudgets |> List.findIndexBack (fun threshold -> cost >= threshold)]
  printfn "%A\nDifficulty: %s (%i)\n%A" roster actualDifficulty cost xpBudgets

let generateStandard pcLevels difficulty =
  let calculateCost = calculateCost pcLevels
  let xpBudgets = xpBudgets id id pcLevels
  let roster = generate calculateCost monsterParties xpBudgets difficulty
  let cost = calculateCost roster
  let roster = pack roster
  showCost roster cost xpBudgets
  roster

type Encounter = {
  roster: (string * int) list
  difficulty: string
  standardDifficulty: string
  earnedXP: int
  cost: int
  standardCost: int
  xpBudgets: int list
  standardXPBudgets: int list
}

let generateVariant monsterParties pcLevels difficulty =
  let scale = 1.4
  let downscale x = (x/50.) ** (1./scale)
  let upscale x = x ** scale * 50.
  let calculateStandardCost = calculateCost pcLevels
  let calculateCost roster =
    roster |> List.sumBy(getXPValue) |> float |> downscale |> upscale |> int
  let standardXPBudgets = xpBudgets id id pcLevels
  let xpBudgets = xpBudgets (float >> downscale) (fun x -> float x/3. |> upscale |> (*) 3.) pcLevels
  let roster = generate calculateCost monsterParties xpBudgets difficulty
  let cost = calculateCost roster
  let standardCost = calculateStandardCost roster
  let standardDifficulty =
    if standardCost >= standardXPBudgets.[4] * 2 then sprintf "Deadly x%d" (standardCost / standardXPBudgets.[4])
    else ["Trivial";"Easy";"Medium";"Hard";"Deadly"].[standardXPBudgets |> List.findIndexBack (fun threshold -> standardCost >= threshold)]
  let actualDifficulty =
    if cost >= xpBudgets.[4] * 2 then sprintf "Deadly x%d" (cost / xpBudgets.[4])
    else ["Trivial";"Easy";"Medium";"Hard";"Deadly"].[xpBudgets |> List.findIndexBack (fun threshold -> cost >= threshold)]
  let sumXP = roster |> List.sumBy(getXPValue)
  { roster = pack roster; difficulty = actualDifficulty; standardDifficulty = standardDifficulty; cost = cost; standardCost = standardCost; xpBudgets = xpBudgets; standardXPBudgets = standardXPBudgets; earnedXP = sumXP }

