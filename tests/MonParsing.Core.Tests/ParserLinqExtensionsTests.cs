using FluentAssertions;

namespace MonParsing.Core.Tests;

public class ParserLinqExtensionsTests
{
    [Fact]
    public void Linq_syntax_behaves_as_expected()
    {
        var fParser = Parser.Char('f');
        var oParser = Parser.Char('o');
        var sParser = Parser.Char(' ');
        var bParser = Parser.Char('b');
        var aParser = Parser.Char('a');
        var rParser = Parser.Char('r');

        var fooBar =
            from f in fParser
            from o1 in oParser
            from o2 in oParser
            from s in sParser
            from b in bParser
            from a in aParser
            from r in rParser
            select (f, o1, o2, s, b, a, r);

        fooBar("foo bar").Should().Equal((('f', 'o', 'o', ' ', 'b', 'a', 'r'), ""));
    }
}
