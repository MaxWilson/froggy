module Frost.Domain
open Froggy.Data

let hitSound = "assets/whack.mp3"
let missSound = "assets/swoosh.mp3"
let deathSound = "assets/gutPunch.mp3"
type Range = Melee of reach:int | Ranged of short: int * long: int
type Attack = {
  name: string
  hitVerb: string
  toHit: int
  damage: Roll.Request
  range: Range
  }
let sword prof strMod = { name = "sword"; range = Melee 5; hitVerb = "stabs"; toHit = prof + strMod; damage = (Roll.d 1 8 strMod) }
let bow prof dexMod = { name = "bow"; range = Ranged (150, 600); hitVerb = "shoots"; toHit = prof + dexMod; damage = (Roll.d 1 8 dexMod) }
let snipe prof dexMod = { name = "bow"; range = Ranged (600, 600); hitVerb = "bulleyes"; toHit = prof + dexMod - 5; damage = (Roll.d 1 8 (10 + dexMod)) }

type Creature = {
  name: string
  icon: string
  str: int
  dex: int
  con: int
  int: int
  wis: int
  cha: int
  hp: int
  ac: int
  attacks: Attack list list
  speed: int
  size: int * int
  }

let frostGiant name =
  let giantAxe = { name = "axe"; range = Melee 10; hitVerb = "cuts"; toHit = +9; damage = Roll.d 3 12 6 }
  let rock = { name = "rock"; range = Ranged (60, 240); hitVerb = "throws a boulder at"; toHit = +9; damage = Roll.d 4 10 6 }
  {
    name = name
    icon = "https://vignette.wikia.nocookie.net/forgottenrealms/images/4/46/Monster_Manual_5e_-_Giant%2C_Frost_-_p155.jpg/revision/latest?cb=20141112160119"
    str = 23
    dex = 9
    con = 21
    int = 9
    wis = 10
    cha = 12
    hp = 138
    ac = 15
    speed = 40
    size = 4,3
    attacks = [
        [giantAxe;giantAxe]
        [rock]
      ]
    }

let cat name =
  let bite = { name = "bite"; range = Melee 5; hitVerb = "bites"; toHit = +6; damage = Roll.d 1 10 5 }
  let claw = { name = "claw"; range = Melee 5; hitVerb = "slashes"; toHit = +6; damage = Roll.d 2 6 5 }
  {
    name = name
    icon = "https://encrypted-tbn0.gstatic.com/images?q=tbn:ANd9GcSBtzOPfVEC0ANMuXJVgPBA9SSAq1jKcncqyDNdEBi-QOxHd7Vl"
    str = 23
    dex = 9
    con = 21
    int = 9
    wis = 10
    cha = 12
    hp = 138
    ac = 15
    speed = 40
    size = 2,3
    attacks = [
        [claw]
        [bite]
      ]
    }
