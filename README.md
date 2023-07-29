# MonParsing

C# library for writing any parser using monadic parser combinators and LINQ syntax.

[![Nuget](https://img.shields.io/nuget/v/MonParsing)](https://www.nuget.org/packages/MonParsing)
[![Nuget](https://img.shields.io/nuget/dt/MonParsing)](https://www.nuget.org/packages/MonParsing)
[![GitHub Workflow Status (with event)](https://img.shields.io/github/actions/workflow/status/rcjorgensen/monparsing-csharp/ci.yaml?event=push)](https://github.com/rcjorgensen/monparsing-csharp/actions/workflows/ci.yaml)
[![Mutation testing badge](https://img.shields.io/endpoint?label=mutation%20score&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2Frcjorgensen%2Fmonparsing-csharp%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/rcjorgensen/monparsing-csharp/main)

```csharp
using MonParsing.Core;
using static MonParsing.Core.Parser;

public class SemVer
{
    public required SemVerCore VersionCore { get; init; }

    public PreRelease? PreRelease { get; init; }

    public Build? Build { get; init; }

    public static Parser<SemVer> Parser { get; private set; }

    static SemVer()
    {
        var letter = Lower.Or(Upper);
        var positive = If(x => '1' <= x && x <= '9');
        var zero = Char('0');
        var digit = zero.Or(positive);
        var digits = OneOrMore(digit);
        var dash = Char('-');
        var nonDigit = letter.Or(dash);
        var identifierCharacter = digit.Or(nonDigit);
        var numericIdentifier = String(zero)
            .Or(from p in positive from ds in ZeroOrMore(digit) select ds.Prepend(p));
        var alphanumericIdentifier = (
            from nd in nonDigit
            from ics in ZeroOrMore(identifierCharacter)
            select ics.Prepend(nd)
        ).Or(OneOrMore(identifierCharacter));
        var buildIdentifier = alphanumericIdentifier.Or(digits);
        var preReleaseIdentifier = alphanumericIdentifier.Or(numericIdentifier);
        var dot = Char('.');
        var build =
            from bis in OneOrMoreSeparated(String(buildIdentifier), dot)
            select new Build { Identifiers = bis };
        var preRelease =
            from pris in OneOrMoreSeparated(String(preReleaseIdentifier), dot)
            select new PreRelease { Identifiers = pris };
        var version = from n in String(numericIdentifier) select int.Parse(n);
        var semVerCore =
            from major in version
            from minor in dot.And(version)
            from patch in dot.And(version)
            select new SemVerCore
            {
                Major = major,
                Minor = minor,
                Patch = patch
            };
        var plus = Char('+');

        // The main parser
        Parser =
            from vc in semVerCore
            from pr in ZeroOrOne(dash.And(preRelease))
            from b in ZeroOrOne(plus.And(build))
            select new SemVer
            {
                VersionCore = vc,
                PreRelease = pr.Value,
                Build = b.Value
            };
    }
}

public record struct SemVerCore(int Major, int Minor, int Patch);

public record PreRelease
{
    public required IEnumerable<string> Identifiers { get; init; }
}

public record Build
{
    public required IEnumerable<string> Identifiers { get; init; }
}

SemVer.Parser("1.0.0");
// {
//   "Value": {
//     "Result": {
//       "VersionCore": {
//         "Major": 1,
//         "Minor": 0,
//         "Patch": 0
//       },
//     },
//     "Input": ""
//   },
//  "Error": null
//}

SemVer.Parser("01.0.0")
// {
//   "Value": null,
//   "Error": "Expectation: The character ., Actual: 1, Input 1.0.0"
// }
```

# Examples

A number of example parsers can be found in [MonParsing.Examples](https://github.com/rcjorgensen/monparsing/tree/main/src/MonParsing.Examples). In addition to the examples the [tests](https://github.com/rcjorgensen/monparsing/tree/main/tests) are a good place to become familiar with how the library works.

# Acknowledgements

This library is based on https://www.cs.nott.ac.uk/~pszgmh/monparsing.pdf
