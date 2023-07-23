using MonParsing.Core;
using static MonParsing.Core.Parser;

namespace MonParsing.Examples;

// Example of simiplified SemVer parser
// https://semver.org/
public static class SemVer
{
    public static Parser<string> PositiveDigit =
        from d in Sat(x => '1' <= x && x <= '9')
        select d.ToString();
    public static Parser<string> ZeroDigit = from z in Char('0') select z.ToString();
    private static Parser<string> Digit = ZeroDigit.Or(PositiveDigit);
    private static Parser<string> DigitsWithoutLeadingZero =
        from p in PositiveDigit
        from ds in OneOrMore(Digit)
        select p + ds;
    private static Parser<string> NumericIdentifier = Digit.Or(DigitsWithoutLeadingZero);
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
