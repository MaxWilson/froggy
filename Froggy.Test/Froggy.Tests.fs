module Froggy.Tests

open Xunit
open Froggy.Packrat
open Froggy.Dnd5e.CharGen

#nowarn "40" // ignore warnings about recursive active patterns via pack. It's fine in this case.
[<Fact(DisplayName = "Packrat tests: make sure that pack can define left-recursive grammars")>]
let VerifyPackrat() =
    let (|Next|Empty|) ((ctx, pos): Input) =
      if pos < ctx.input.Length then Next(ctx.input.[pos], (ctx, pos+1))
      else Empty
    // define an intermediate production "E" to make recursion indirect
    let rec (|CompoundExpression|_|) = pack (function
        | E(v, Next('+', Next('x', next))) -> Some(v+1, next)
        | Next('x', next) -> Some(1, next)
        | _ -> None)
    and (|E|_|) = pack (function
        | CompoundExpression(v, next) -> Some(v, next)
        | _ -> None)
    // It's a CompoundExpression, and it's also an E
    match ParseContext.Init "x+x" with
    | CompoundExpression(v, Empty) -> Assert.Equal(2, v)
    | _ -> failwith "Could not parse"
    match ParseContext.Init "x+x" with
    | E(v, Empty) -> Assert.Equal(2, v)
    | _ -> failwith "Could not parse"
    match ParseContext.Init "x+x+x+x" with
    | E(4, Empty) -> ()
    | _ -> failwith "Could not parse"

[<Fact(DisplayName="Usage tests: verify that chargen commands can be parsed correctly")>]
let VerifyChargen() =
  let ctx = StatBank()
  ()
