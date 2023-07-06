using FluentAssertions;

namespace MonParsing.Core.Tests;

public class UnitTest1
{
    [Fact]
    public void Item_does_not_parse_empty_input() => Parser.Item("").Should().BeEmpty();

    [Fact]
    public void Item_parses_first_item() => Parser.Item("foo").Should().Equal(('f', "oo"));

    [Fact]
    public void Seq_does_not_parse_empty_input() =>
        Parser.Seq(Parser.Item, Parser.Item)("").Should().BeEmpty();

    [Fact]
    public void Seq_does_not_parse_input_with_length_1() =>
        Parser.Seq(Parser.Item, Parser.Item)("f").Should().BeEmpty();

    [Fact]
    public void Seq_does_parses_input_with_length_greater_than_1() =>
        Parser.Seq(Parser.Item, Parser.Item)("foo").Should().Equal((('f', 'o'), "o"));

    [Fact]
    public void Sat_does_not_parse_input_when_predicate_is_not_satisfied() =>
        Parser.Sat(c => c == 'f')("bar").Should().BeEmpty();

    [Fact]
    public void Sat_parses_input_when_predicate_is_satisfied() =>
        Parser.Sat(c => c == 'f')("foo").Should().Equal(('f', "oo"));
}
