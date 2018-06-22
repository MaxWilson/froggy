module Froggy.Test

open Xunit

[<Fact>]
let ``Check Name``() =
  Assert.Equal("Froggy", Froggy.Library.Name)

type Simple =
| Simple of int * int
| Adv of int * int
| Disadv of int * int

type Compound =
| Single of Simple
| Sum of Compound * Compound
| MultByConstant of int * Compound
| DivByConstant of int * Compound
| Repeat of int * Compound
| Check of roll : Compound * resultSpecs : (int * Compound) list * fallbackResult : int

type Command =
| Roll of Compound
| Average of Compound

module Util =
  let nomatch() = failwith "No match"

#nowarn "40" // recursive object references from memoization
module Parser =

  open Froggy.Packrat

  type Impl() =
      (* Throughout this file we use string * int as a data type for parse inputs,
      representing a string and a position within it. I can't figure out how to get
      the type inference to work with type aliases though so I'm just using a raw
      string * int tuple. Wherever you see "string * int" in a type, think "parser
      input."
      *)
      let alpha = Set<char>['A'..'Z'] + Set<char>['a'..'z']
      let numeric = Set<char>['0'..'9']
      let arithmeticOperators = Set<_>['+'; '-']
      let advantageDisadvantage = Set<_>['A'; 'D'; 'a'; 'd']
      let alphanumeric = alpha + numeric
      let memoize = fun name x -> x

      let (|Next|Empty|) = function
          | (input : string), pos when pos < input.Length -> Next(input.[pos], (input, pos+1))
          | _ -> Empty

      let (|Char|_|) alphabet = function
          | Empty -> None
          | s, i when Set.contains s.[i] alphabet -> Some(s, i+1)
          | _ -> None

      let rec (|MaybeChars|) alphabet = function
          | Char alphabet (MaybeChars alphabet it)
          | it -> it

      let rec (|Chars|_|) alphabet = function
          | Char alphabet (MaybeChars alphabet it) -> Some it
          | it -> None

      let (|NextChar|_|) alphabet = function
          | Empty -> None
          | s, i when Set.contains s.[i] alphabet -> Some(s.[i], (s, i+1))
          | _ -> None

      let sub (s: string, i0) (_, i1) =
          s.Substring(i0, i1-i0)

      let (|NextWord|_|) (word : string) input =
          let letters = word |> List.ofSeq
          let rec (|NextChar|_|) letters = function
              | Next(x, rest) when x = List.head letters ->
                  match List.tail letters with
                  | [] -> Some rest
                  | letters ->
                      match rest with
                      | NextChar letters rest -> Some rest
                      | _ -> None
              | _ -> None
          match input with
          | NextChar letters rest -> Some rest
          | _ -> None

      let rec (|Number|_|) = function
          | Chars numeric i1 as i0 -> (System.Int32.Parse(sub i0 i1), i1) |> Some
          | _ -> None
      and (|CompoundExpression|_|) = memoize "CompoundExpression" (function
          | CompoundExpression(lhs, Next('+', CompoundExpressionTerm(rhs, next))) -> Some(Sum(lhs, rhs), next)
          | CompoundExpression(lhs, Next('-', CompoundExpressionTerm(rhs, next))) -> Some(Sum(lhs, MultByConstant(-1, rhs)), next)
          | CheckTerm(v, next) -> Some(v, next)
          | CompoundExpressionTerm(lhs, next) -> Some(lhs, next)
          | _ -> None)
      and (|CompoundExpressionTerm|_|) = memoize "CompoundExpressionTerm"  (function
          | CompoundExpressionTerm(v, Next('/', Number(n, next))) -> Some(DivByConstant(n, v), next)
          | Next('(', CompoundExpression(lhs, Next(')', next))) -> Some(lhs, next)
          | Number(n, Next('.', CompoundExpression(v, next))) ->
              Some(Repeat(n, v), next)
          | SimpleExpression(v, next) -> Some(Single(v), next)
          | _ -> None)
      and (|CheckTerm|_|) =
          let (|Predicate|_|) = function
              | CompoundExpression(roll, Next('?', Number(target, Next(':', next)))) ->
                  Some(roll, target, next)
              | CompoundExpression(roll, Next('?', Number(target, next))) ->
                  Some(roll, target, next)
              | Number(target, NextChar advantageDisadvantage (c, Next('?', next))) ->
                  let ctor = (if c = 'a' || c = 'A' then Adv else Disadv) >> Single
                  Some(ctor(1, 20), target, next)
              | Number(target, Next('?', next)) ->
                  Some(Single(Simple(1, 20)), target, next)
              | _ -> None
          let (|ResultTerm|_|) = function
              | CompoundExpression(result, next) ->
                  Some(result, next)
              | next -> Some(Single(Simple(1, 1)), next)
          memoize "CheckTerm" (function
              | Predicate(roll, target, ResultTerm(consequent, next)) ->
                  let rec double = function
                      | Single(Simple(n, 1)) as constant -> constant
                      | Single(Simple(n, d)) ->
                          Single(Simple(n*2, d))
                      | Single(Adv(n, d)) ->
                          Single(Adv(n*2, d))
                      | Single(Disadv(n, d)) ->
                          Single(Disadv(n*2, d))
                      | Sum(lhs, rhs) -> Sum(double lhs, double rhs)
                      | MultByConstant(k, rhs) -> MultByConstant(k, double rhs)
                      | DivByConstant(k, rhs) -> DivByConstant(k, double rhs)
                      | Check(_) -> Util.nomatch()
                      | Repeat(_) -> Util.nomatch()
                  let rec maximize = function
                      | Single(Simple(n, d))
                      | Single(Adv(n, d))
                      | Single(Disadv(n, d)) ->
                          n*d
                      | Sum(lhs, rhs) -> maximize lhs + maximize rhs
                      | MultByConstant(k, rhs) ->
                          if k > 0 then k * maximize rhs
                          else k * minimize rhs
                      | DivByConstant(k, rhs) -> maximize rhs / k
                      | Repeat(k, rhs) -> k * maximize rhs
                      | Check(_) -> Util.nomatch()
                  and minimize = function
                      | Single(Simple(n, d))
                      | Single(Adv(n, d))
                      | Single(Disadv(n, d)) ->
                          n
                      | _ -> Util.nomatch()
                  Some(Check(roll, [maximize roll, double consequent; target, consequent], 0), next)
              | _ -> None
          )
      and (|SimpleExpression|_|) input =
          let makeRoll n d input =
              match input with
              | Char advantageDisadvantage (Char arithmeticOperators _) as rest ->
                  let (s, i) = input
                  let roll = match s.[i] with
                              | 'A' | 'a' -> Adv(n, d)
                              | 'D' | 'd' -> Disadv(n, d)
                              | _ -> Util.nomatch()
                  Some (roll, (s, i+1))
              | Char advantageDisadvantage _ ->
                  let (s, i) = input
                  let roll = match s.[i] with
                              | 'A' | 'a' -> Adv(n, d)
                              | 'D' | 'd' -> Disadv(n, d)
                              | _ -> Util.nomatch()
                  Some (roll, (s, i+1))
              | rest -> Some(Simple(n, d), rest)
          match input with
          | Next('d', Number(dieSize, next)) -> makeRoll 1 dieSize next
          | Number (n, Next('d', Number(dieSize, next))) -> makeRoll n dieSize next
          | Number (n, Next('d', next)) -> Some (Simple(n, 6), next)
          | Number (n, next) -> Some (Simple(n, 1), next)
          | _ -> None
      and (|CommandExpression|_|) = function
          | NextWord "avg." (CompoundExpression(v, next)) -> Some (Average(v), next)
          | CompoundExpression(v, next) -> Some (Roll(v), next)
          | _ -> None

      member this.parseCompound txt =
          match (txt, 0) with
          | CompoundExpression(cmd, Empty) -> cmd
          | _ -> failwithf "failed to parse '%s'" txt

      member this.parseCommand txt =
          match (txt, 0) with
          | CommandExpression(cmd, Empty) -> cmd
          | _ -> failwithf "failed to parse '%s'" txt
      member this.Ctx = ctx

  let Parse txt =
      (Impl()).parseCompound txt
  let ParseCommand txt = (Impl()).parseCommand txt

[<Fact>]
let Parsing() =
    Assert.Equal(Single(Simple(3,6)), Parser.Parse "3d6")
    Assert.Equal(Single(Simple(1,8)), Parser.Parse "d8")
    Assert.Equal(Single(Simple(4,6)), Parser.Parse "4d")
    Assert.Equal(Single(Simple(11,1)), Parser.Parse "11")

[<Fact>]
let Rolling() =
    let withinBounds low high spec seed =
        let roller = Resolver(new System.Random(seed))
        let result = roller.Resolve(spec : Compound) |> function Audit(_, v, _) -> v
        low <= result && result <= high
    Check.QuickThrowOnFailure (withinBounds 3 18 (Single(Simple(3,6))))

[<Fact>]
let Average() =
    let eq(expected, actual) = Assert.InRange(actual, expected - 0.0001, expected + 0.0001)
    Assert.Equal(3.5, Dice.Instance.Average(Single(Simple(1,6))))
    Assert.Equal(10.5, Dice.Instance.Average(Single(Simple(3,6))))
    eq (4./216., Dice.Instance.Average(Check(Sum(Single(Simple(1,6)), Single(Simple(2,6))), [17, Single(Simple(1,1))], 0)))

[<Theory>]
[<InlineData("3d6", 10.5)>] // Basic roll
[<InlineData("10.3d6", 105.)>] // Multiple rolls
[<InlineData("10.3d6+4", 145.)>] // Multiple rolls with complex result
[<InlineData("10.3d6-2", 85.)>] // Multiple rolls with subtraction
[<InlineData("1d2A", 1.75)>] // Roll with advantage
[<InlineData("1d2D", 1.25)>] // Roll with disadvantage
[<InlineData("20.18?", 3.)>] // Multiple checks
[<InlineData("(d20+11)-(d20+8)", 3.)>] // Subtraction
[<InlineData("10.18A?", 2.775)>] // Simple check with advantage
[<InlineData("10.18D?", 0.225)>] // Simple check with disadvantage
[<InlineData("20.18A?100", 555.)>] // Check with advantage and result
[<InlineData("20.20?1d10+d6-2", 16.)>] // Check with result
[<InlineData("20.d20?14", 7)>] // Check with explicit roll syntax
[<InlineData("20.(d20a-d20d)?0", 16.984)>] // Check with explicit roll syntax and complex roll
[<InlineData("2.d3/2", 1.333)>] // Division, rounds down
[<InlineData("2.d4/2", 2.)>] // Division, rounds down
[<InlineData("(d20+20)?31:1d10", 3.025)>] // Doubling for crits applies on max roll
[<InlineData("d20+20?31:1d10", 3.025)>] // Parens should be optional
[<InlineData("(d2-d2)?1", 0.25)>]
let ``Complete-ish list of example roll specs``(input: string, expectedAverage: float) =
    let spec = Parser.Parse(input)
    let round (x : float) = System.Math.Round(x, 3) // round to three places
    Assert.Equal(round expectedAverage, round (Dice.Instance.Average(spec)))
    Dice.Instance.Resolve(spec) |> ignore // must not throw

[<Fact>]
let ``Sums should be left-associative``() =
    Assert.Equal(65., Parser.Parse "20d6-d4-d4" |> Dice.Instance.Average)

[<Theory>]
[<InlineData("3.2d4", "(X,X,X)")>]
[<InlineData("3.4d6", "(X,X,X)")>]
[<InlineData("(3.4d6)?1:3d10", "(X,X,X)->X")>]
[<InlineData("(d20a+d4+7)?4:d10+d6+5", "X->X")>]
[<InlineData("4.(d20a+d4+7)?4:d10+d6+5", "(X->X,X->X,X->X,X->X)")>]
let ``Examples of explanations that should be checkable``(spec, expected) =
    let spec = Parser.ParseCommand(spec)
    let result, explain = Dice.Instance.Resolve(spec)
    let explain = System.Text.RegularExpressions.Regex.Replace(explain, "\d+", "X")
    Assert.Equal(expected, explain)

[<Theory>]
[<InlineData("3d6", null)>]
[<InlineData("avg.3d6", "10.50")>]
[<InlineData("avg.3d6-4", "6.50")>]
[<InlineData("avg.3d6-(d4-d4)", "10.50")>]
[<InlineData("avg.20d6-(d4-d4)", "70")>]
[<InlineData("avg.20d6-(d4-d4)", "70")>]
[<InlineData("avg.20d6-(d4-d4)", "70")>]
let ``Complete-ish list of example command specs``(input: string, expectedOutput: string) =
    let spec = Parser.ParseCommand(input)
    let output = Dice.Instance.Resolve(spec) |> fst
    if(expectedOutput <> null) then
        Assert.Equal<string>(expectedOutput, output)

[<Theory(Skip="Incomplete")>]
[<InlineData("20.18?20?", 4.)>]
[<InlineData("20.d4A+d10+d20D:18?d10+5+d6", 0.)>]
let ``Example roll specs that aren't working yet``(input: string, expectedAverage: float) =
    Assert.Equal(expectedAverage, Dice.Instance.Average(Parser.Parse(input)))

type Expr = Leaf of char | Interior of Expr * Expr
#nowarn "0040" // Allow object recursion without warnings so we can write recursive memoized rules
[<Fact>]
let ``Should be able to parse direct left-recursive left-associative grammers``() =
    let (|Next|Empty|) = function
    | (input : string), pos when pos < input.Length -> Next(input.[pos], (input, pos+1))
    | _ -> Empty
    let show = sprintf "%A"
    let c = ParserContext()
    let rec (|Xs|_|) = memoize c "Xs" (function
        | Xs(lhs, Next('+', Number(rhs, next))) -> Some(Interior(lhs, rhs), next)
        | Number(v, next) -> Some(v, next)
        | _ -> None)
    and (|Number|_|) = memoize c "Number" (function
        | Next(c, next) when System.Char.IsDigit(c) -> Some(Leaf c, next)
        | _ -> None)
    match("1+2+3",0) with
    | Xs(v, Empty) ->
        // Result should be left-associative
        Assert.Equal(show <| Interior(Interior(Leaf('1'),Leaf('2')),Leaf('3')), show v)
    | _ -> failwith "Could not parse"

#nowarn "0040" // Allow object recursion without warnings so we can write recursive memoized rules
[<Fact>]
let ``Should be able to parse indirect left-recursive grammers``() =
    let (|Next|Empty|) = function
    | (input : string), pos when pos < input.Length -> Next(input.[pos], (input, pos+1))
    | _ -> Empty

    let c = ParserContext()
    // define an intermediate production "E" to make recursion indirect
    let rec (|Xs|_|) = memoize c "Xs" (function
        | E(lhs, Next('+', Number(rhs, next))) -> Some(Interior(lhs, rhs), next)
        | Next(c, next) when System.Char.IsDigit(c) -> Some(Leaf c, next)
        | _ -> None)
    and (|E|_|) = memoize c "E" (function
        | Xs(v, next) -> Some(v, next)
        | _ -> None)
    and (|Number|_|) = memoize c "Number" (function
        | Next(c, next) when System.Char.IsDigit(c) -> Some(Leaf c, next)
        | _ -> None)
    // It's an Xs, and it's also an E
    match("1+2+3",0) with
    | Xs(v, Empty) ->
        Assert.Equal(Interior(Interior(Leaf('1'),Leaf('2')),Leaf('3')), v)
    | _ -> failwith "Could not parse"
    match("1+2+3",0) with
    | E(v, Empty) ->
        Assert.Equal(Interior(Interior(Leaf('1'),Leaf('2')),Leaf('3')), v)
    | _ -> failwith "Could not parse"

[<Fact>]
let ``More complex indirect left-recursive grammers``() =
    let (|Next|Empty|) = function
    | (input : string), pos when pos < input.Length -> Next(input.[pos], (input, pos+1))
    | _ -> Empty

    let c = ParserContext()
    // define an intermediate production "E" to make recursion indirect
    let rec (|CompoundExpression|_|) = memoize c "CompoundExpression" (function
        | E(v, Next('+', Next('x', next))) -> Some(v+1, next)
        | Next('x', next) -> Some(1, next)
        | _ -> None)
    and (|E|_|) = memoize c "E" (function
        | CompoundExpression(v, next) -> Some(v, next)
        | _ -> None)
    // It's an Xs, and it's also an E
    match("x+x",0) with
    | CompoundExpression(v, Empty) -> Assert.Equal(2, v)
    | _ -> failwith "Could not parse"
    match("x+x",0) with
    | E(v, Empty) -> Assert.Equal(2, v)
    | _ -> failwith "Could not parse"