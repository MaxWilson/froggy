module Froggy.Tests

open Xunit
open Froggy.Packrat

#nowarn "40" // ignore warnings about recursive active patterns via pack. It's fine in this case.
[<Fact>]
let ``Packrat tests: make sure that pack can define left-recursive grammars``() =
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

[<Fact>]
let ``Packrat tests: grammar for converting yesno to binary-ish``() =
    let (|YesNo|_|) = pack <| function
      | Str "yes" rest -> Some(1I, rest)
      | Str "no" rest -> Some(0I, rest)
      | _ -> None
    let rec (|YesNos|_|) = pack <| function
      | YesNos(v1, OWS( YesNo(v2, rest) )) -> Some(v1*10I + v2, rest)
      | YesNo(v, rest) -> Some(v, rest)
      | _ -> None
    // It's a CompoundExpression, and it's also an E
    match ParseContext.Init "yes no no yesYesNOnoYES" with
    | YesNos(v, End) -> Assert.Equal(10011001I, v)
    | _ -> failwith "Could not parse"
    match ParseContext.Init " yes " with
    | YesNos(_) -> failwith "expected parse failure"
    | _ -> ()


