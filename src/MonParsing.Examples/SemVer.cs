using MonParsing.Core;

using static MonParsing.Core.Parser;

namespace MonParsing.Examples;

public class SemVer
{
    public required SemVerCore VersionCore { get; init; }

    public PreRelease? PreRelease { get; init; }

    public Build? Build { get; init; }

    private static Parser<SemVer> _parser;
    public static Parser<SemVer> Parser => _parser;

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

        _parser =
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
