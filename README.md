# MonParsing

C# library for writing any parser using monadic parser combinators and LINQ syntax.

[![Nuget](https://img.shields.io/nuget/v/MonParsing)](https://www.nuget.org/packages/MonParsing)
[![Nuget](https://img.shields.io/nuget/dt/MonParsing)](https://www.nuget.org/packages/MonParsing)
[![Mutation testing badge](https://img.shields.io/endpoint?style=flat&url=https%3A%2F%2Fbadge-api.stryker-mutator.io%2Fgithub.com%2Frcjorgensen%2Fmonparsing-csharp%2Fmain)](https://dashboard.stryker-mutator.io/reports/github.com/rcjorgensen/monparsing-csharp/main)

```csharp
using MonParsing.Core;
using static MonParsing.Core.Parsers;

// Example of simplified SemVer parser
// https://semver.org/
public static class SemVer
{
    public static Parser<string> PositiveDigit =
        from d in Sat(x => '1' <= x && x <= '9')
        select d.ToString();

    public static Parser<string> ZeroDigit =
        from z in Char('0')
        select z.ToString();

    private static Parser<string> Digit = ZeroDigit.Or(PositiveDigit);

    private static Parser<string> DigitsWithoutLeadingZero =
        from p in PositiveDigit
        from ds in OneOrMore(Digit)
        select p + ds;

    private static Parser<string> NumericIdentifier =
        Digit.Or(DigitsWithoutLeadingZero);

    private static Parser<char> Dot = Char('.');

    private static Parser<string> VersionCore =
        from major in NumericIdentifier
        from dot1 in Dot
        from minor in NumericIdentifier
        from dot2 in Dot
        from patch in NumericIdentifier
        select major + dot1 + minor + dot2 + patch;

    // ... pre-release syntax omitted

    public static Parser<string> Parse = VersionCore;
}

// Parses a valid SemVer in this case just returning the input
IResult<IParseResult<string>> valid = SemVer.Parse("1.2.3");
// { Value: "1.2.3", Error: null }

// Returns an error result since leading zeros are not allowed
IResult<IParseResult<string>> invalid = SemVer.Parse("00.1.0");
// { Value: null, Error: "Invalid input: 00.1.0" }
```

See [the examples library](src/MonParsing.Examples) for more examples.

# Acknowledgements

This library is based on https://www.cs.nott.ac.uk/~pszgmh/monparsing.pdf
