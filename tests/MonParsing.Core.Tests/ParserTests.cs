using FluentAssertions;

namespace MonParsing.Core.Tests;

public class ParserTests
{
    [Fact]
    public void Item_does_not_parse_empty_input() => Parser.Item("").Should().BeEmpty();

    [Fact]
    public void Item_parses_first_item() => Parser.Item("foo").Should().Equal(('f', "oo"));

    [Fact]
    public void Seq_does_not_parse_empty_input() =>
        Parser.Item.Seq(Parser.Item)("").Should().BeEmpty();

    [Fact]
    public void Seq_does_not_parse_input_with_length_1() =>
        Parser.Item.Seq(Parser.Item)("f").Should().BeEmpty();

    [Fact]
    public void Seq_does_parses_input_with_length_greater_than_1() =>
        Parser.Item.Seq(Parser.Item)("foo").Should().Equal((('f', 'o'), "o"));

    [Fact]
    public void Sat_does_not_parse_input_when_predicate_is_not_satisfied() =>
        Parser.Sat(c => c == 'f')("bar").Should().BeEmpty();

    [Fact]
    public void Sat_parses_input_when_predicate_is_satisfied() =>
        Parser.Sat(c => c == 'f')("foo").Should().Equal(('f', "oo"));

    [Fact]
    public void Char_does_not_parse_input_when_first_char_does_not_match() =>
        Parser.Char('f')("bar").Should().BeEmpty();

    [Fact]
    public void Char_parses_input_when_first_char_matches() =>
        Parser.Char('f')("foo").Should().Equal(('f', "oo"));

    [Theory]
    [InlineData("0")]
    [InlineData("1")]
    [InlineData("2")]
    [InlineData("3")]
    [InlineData("4")]
    [InlineData("5")]
    [InlineData("6")]
    [InlineData("7")]
    [InlineData("8")]
    [InlineData("9")]
    public void Digit_parses_digits_from_0_to_9(string digit) =>
        Parser.Digit(digit).Should().Equal((Convert.ToChar(digit), ""));

    [Fact]
    public void Digit_does_not_parse_non_digit() => Parser.Digit("foo").Should().BeEmpty();

    [Fact]
    public void Word_parses_input_with_multiple_results() =>
        Parser.Word("Yes!").Should().Equal(("Yes", "!"), ("Ye", "s!"), ("Y", "es!"), ("", "Yes!"));

    [Fact]
    public void String_of_empty_string_parses_empty_input() =>
        Parser.String("")("").Should().Equal(("", ""));

    [Fact]
    public void String_parses_input_that_matches() =>
        Parser.String("hello")("hello world").Should().Equal(("hello", " world"));

    [Fact]
    public void String_does_not_parse_input_that_does_not_match() =>
        Parser.String("hello")("helicopter").Should().BeEmpty();
}
