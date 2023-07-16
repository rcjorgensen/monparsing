using System.Diagnostics;
using FluentAssertions;

namespace MonParsing.Core.Tests;

public class ParsersTests
{
    private static Parser<char> Item = Parsers.Item;

    [Fact]
    public void Zero_never_parses() => Parsers.Zero<char>("foo").Should().BeEmpty();

    [Fact]
    public void Result_does_not_consume_from_input_and_returns_given_value() =>
        Parsers.Result("foo")("bar").Should().Equal(ParseResult.Default("foo", "bar"));

    [Fact]
    public void Item_does_not_parse_empty_input() => Parsers.Item("").Should().BeEmpty();

    [Fact]
    public void Item_parses_first_item() =>
        Parsers.Item("foo").Should().Equal(ParseResult.Default('f', "oo"));

    [Fact]
    public void Sat_does_not_parse_input_when_predicate_is_not_satisfied() =>
        Parsers.Sat(c => c == 'f')("bar").Should().BeEmpty();

    [Fact]
    public void Sat_parses_input_when_predicate_is_satisfied() =>
        Parsers.Sat(c => c == 'f')("foo").Should().Equal(ParseResult.Default('f', "oo"));

    [Fact]
    public void Char_does_not_parse_input_when_first_char_does_not_match() =>
        Parsers.Char('f')("bar").Should().BeEmpty();

    [Fact]
    public void Char_parses_input_when_first_char_matches() =>
        Parsers.Char('f')("foo").Should().Equal(ParseResult.Default('f', "oo"));

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
        Parsers.Digit(digit).Should().Equal(ParseResult.Default(Convert.ToChar(digit), ""));

    [Fact]
    public void Digit_does_not_parse_non_digit() => Parsers.Digit("foo").Should().BeEmpty();

    [Fact]
    public void Lower_does_not_parse_non_letter() => Parsers.Lower("_oo").Should().BeEmpty();

    [Fact]
    public void Lower_does_not_parse_upper_case_character() =>
        Parsers.Lower("Foo").Should().BeEmpty();

    [Fact]
    public void Lower_parses_lower_case_character() =>
        Parsers.Lower("foo").Should().Equal(ParseResult.Default('f', "oo"));

    [Fact]
    public void Lower_parses_a() => Parsers.Lower("a").Should().Equal(ParseResult.Default('a', ""));

    [Fact]
    public void Lower_parses_z() => Parsers.Lower("z").Should().Equal(ParseResult.Default('z', ""));

    [Fact]
    public void Upper_does_not_parse_non_letter() => Parsers.Upper("_oo").Should().BeEmpty();

    [Fact]
    public void Upper_does_not_parse_lower_case_character() =>
        Parsers.Upper("foo").Should().BeEmpty();

    [Fact]
    public void Upper_parses_upper_case_character() =>
        Parsers.Upper("Foo").Should().Equal(ParseResult.Default('F', "oo"));

    [Fact]
    public void Upper_parses_A() => Parsers.Upper("A").Should().Equal(ParseResult.Default('A', ""));

    [Fact]
    public void Upper_parses_Z() => Parsers.Upper("Z").Should().Equal(ParseResult.Default('Z', ""));

    [Fact]
    public void String_of_empty_string_parses_empty_input() =>
        Parsers.String("")("").Should().Equal(ParseResult.Default("", ""));

    [Fact]
    public void String_parses_input_that_matches() =>
        Parsers
            .String("hello")("hello world")
            .Should()
            .Equal(ParseResult.Default("hello", " world"));

    [Fact]
    public void String_does_not_parse_input_that_does_not_match() =>
        Parsers.String("hello")("helicopter").Should().BeEmpty();

    [Fact]
    public void Integer_does_not_parse_non_integer() => Parsers.Integer("foo").Should().BeEmpty();

    [Fact]
    public void Integer_parses_positive_number() =>
        Parsers.Integer("10").Should().Equal(ParseResult.Default(10, ""));

    [Fact]
    public void Integer_parses_negative_number() =>
        Parsers.Integer("-10").Should().Equal(ParseResult.Default(-10, ""));

    [Fact]
    public void Or_short_circuits()
    {
        IEnumerable<IParseResult<string>> ThrowingParser(string input)
        {
            throw new UnreachableException();
        }

        Parser<string> colour = Parsers.String("yellow").Or(ThrowingParser);

        colour("yellow").ToList().Should().Equal(ParseResult.Default("yellow", ""));
    }

    [Fact]
    public void Or_calls_second_parser_when_first_does_not_parse_input()
    {
        Parser<string> colour = Parsers.String("yellow").Or(Parsers.String("orange"));

        colour("orange").ToList().Should().Equal(ParseResult.Default("orange", ""));
    }

    [Fact]
    public void Plus_does_not_short_circut()
    {
        var expectedMessage = "This exception should be thrown";
        IEnumerable<IParseResult<string>> ThrowingParser(string input)
        {
            throw new InvalidOperationException(expectedMessage);
        }

        Parser<string> colour = Parsers.String("yellow").Plus(ThrowingParser);

        colour
            .Invoking(p => p("yellow").ToList())
            .Should()
            .Throw<InvalidOperationException>()
            .WithMessage(expectedMessage);
    }
}
