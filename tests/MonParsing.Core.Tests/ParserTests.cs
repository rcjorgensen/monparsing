using FluentAssertions;

namespace MonParsing.Core.Tests;

public class UnitTest1
{
    [Fact]
    public void Item_does_not_parse_empty_string() => Parser.Item("").Should().BeEmpty();

    [Fact]
    public void Item_parses_one_character() => Parser.Item("foo").Should().Equal(('f', "oo"));

    [Fact]
    public void Seq_does_not_parse_empty_string() =>
        Parser.Seq(Parser.Item, Parser.Item)("").Should().BeEmpty();

    [Fact]
    public void Seq_does_not_parse_string_with_length_1() =>
        Parser.Seq(Parser.Item, Parser.Item)("f").Should().BeEmpty();

    [Fact]
    public void Seq_does_parses_string_with_length_greater_than_1() =>
        Parser.Seq(Parser.Item, Parser.Item)("foo").Should().Equal((('f', 'o'), "o"));
}
