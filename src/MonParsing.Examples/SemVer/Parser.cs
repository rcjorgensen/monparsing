using MonParsing.Core;
using static MonParsing.Core.Parser;

namespace MonParsing.Examples.SemVer;

// Example of SemVer parser
// https://semver.org/
public static class Parser
{
    private static Parser<string> Letter = Lower.Or(Upper).Convert();
    private static Parser<string> PositiveDigit = If(x => '1' <= x && x <= '9').Convert();
    public static Parser<string> ZeroDigit = String("0");
    private static Parser<string> Digit = ZeroDigit.Or(PositiveDigit);
    private static Parser<string> Digits = from ds in OneOrMore(Digit) select string.Concat(ds);
    private static Parser<string> Dash = String("-");
    private static Parser<string> NonDigit = Letter.Or(Dash);
    private static Parser<string> IdentifierCharacter = Digit.Or(NonDigit);
    private static Parser<string> IdentifierCharacters =
        from xs in OneOrMore(IdentifierCharacter)
        select string.Concat(xs);
    private static Parser<string> NumericIdentifier = Or(
        ZeroDigit,
        PositiveDigit,
        from p in PositiveDigit
        from ds in Digits
        select p + ds
    );
    private static Parser<string> AlphanumericIdentifier = Or(
        NonDigit,
        from nd in NonDigit
        from ics in IdentifierCharacters
        select nd + ics,
        from ics in IdentifierCharacters
        from nd in NonDigit
        select ics + nd,
        from ics1 in IdentifierCharacters
        from nd in NonDigit
        from ics2 in IdentifierCharacters
        select ics1 + nd + ics2
    );
    private static Parser<string> BuildIdentifier = AlphanumericIdentifier.Or(Digits);
    private static Parser<string> PreReleaseIdentifier = AlphanumericIdentifier.Or(
        NumericIdentifier
    );
    private static Parser<string> Dot = String(".");
    private static Parser<Build> Build =
        from bis in OneOrMoreSeparated(BuildIdentifier, Dot)
        select new Build { Identifiers = bis };
    private static Parser<PreRelease> PreRelease =
        from pris in OneOrMoreSeparated(PreReleaseIdentifier, Dot)
        select new PreRelease { Identifiers = pris };
    private static Parser<VersionCore> VersionCore =
        from major in NumericIdentifier
        from minor in Dot.And(NumericIdentifier)
        from patch in Dot.And(NumericIdentifier)
        select new VersionCore
        {
            Major = int.Parse(major),
            Minor = int.Parse(minor),
            Patch = int.Parse(patch)
        };

    public static Parser<string> Plus = String("+");

    public static Parser<SemVer> Parse = Or(
        from vc in VersionCore
        select new SemVer { VersionCore = vc },
        from vc in VersionCore
        from pr in Dash.And(PreRelease)
        select new SemVer { VersionCore = vc, PreRelease = pr },
        from vc in VersionCore
        from b in Plus.And(Build)
        select new SemVer { VersionCore = vc, Build = b },
        from vc in VersionCore
        from pr in Dash.And(PreRelease)
        from b in Plus.And(Build)
        select new SemVer
        {
            VersionCore = vc,
            PreRelease = pr,
            Build = b
        }
    );
}
