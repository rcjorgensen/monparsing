using System.Diagnostics;
using FluentAssertions;
using static MonParsing.Core.Parser;
using static MonParsing.Core.Result;

namespace MonParsing.Core.Tests;

public class ParserTests
{
    [Fact]
    public void Zero_never_parses() =>
        Zero<char>("foo").Should().BeEquivalentTo(Error<IParseResult<char>>("Invalid input: foo"));

    [Fact]
    public void Result_does_not_consume_from_input_and_returns_given_value() =>
        Result("foo")("bar").Should().BeEquivalentTo(Ok(ParseResult.Of("foo", "bar")));

    [Fact]
    public void Item_does_not_parse_empty_input() =>
        Item("").Should().BeEquivalentTo(Error<IParseResult<char>>("Empty input"));

    [Fact]
    public void Item_parses_first_item() =>
        Item("foo").Should().BeEquivalentTo(Ok(ParseResult.Of('f', "oo")));

    [Fact]
    public void Sat_does_not_parse_input_when_predicate_is_not_satisfied() =>
        Sat(c => c == 'f')("bar")
            .Should()
            .BeEquivalentTo(Error<IParseResult<char>>("Invalid input: bar"));

    [Fact]
    public void Sat_parses_input_when_predicate_is_satisfied() =>
        Sat(c => c == 'f')("foo").Should().BeEquivalentTo(Ok(ParseResult.Of('f', "oo")));

    [Fact]
    public void Char_does_not_parse_input_when_first_char_does_not_match() =>
        Char('f')("bar").Should().BeEquivalentTo(Error<IParseResult<char>>("Invalid input: bar"));

    [Fact]
    public void Char_parses_input_when_first_char_matches() =>
        Char('f')("foo").Should().BeEquivalentTo(Ok(ParseResult.Of('f', "oo")));

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
        Digit(digit).Should().BeEquivalentTo(Ok(ParseResult.Of(Convert.ToChar(digit), "")));

    [Fact]
    public void Digit_does_not_parse_non_digit() =>
        Digit("foo").Should().BeEquivalentTo(Error<IParseResult<char>>("Invalid input: foo"));

    [Fact]
    public void Lower_does_not_parse_non_letter() =>
        Lower("_oo").Should().BeEquivalentTo(Error<IParseResult<char>>("Invalid input: _oo"));

    [Fact]
    public void Lower_does_not_parse_upper_case_character() =>
        Lower("Foo").Should().BeEquivalentTo(Error<IParseResult<char>>("Invalid input: Foo"));

    [Fact]
    public void Lower_parses_lower_case_character() =>
        Lower("foo").Should().BeEquivalentTo(Ok(ParseResult.Of('f', "oo")));

    [Fact]
    public void Lower_parses_a() => Lower("a").Should().BeEquivalentTo(Ok(ParseResult.Of('a', "")));

    [Fact]
    public void Lower_parses_z() => Lower("z").Should().BeEquivalentTo(Ok(ParseResult.Of('z', "")));

    [Fact]
    public void Upper_does_not_parse_non_letter() =>
        Upper("_oo").Should().BeEquivalentTo(Error<IParseResult<char>>("Invalid input: _oo"));

    [Fact]
    public void Upper_does_not_parse_lower_case_character() =>
        Upper("foo").Should().BeEquivalentTo(Error<IParseResult<char>>("Invalid input: foo"));

    [Fact]
    public void Upper_parses_upper_case_character() =>
        Upper("Foo").Should().BeEquivalentTo(Ok(ParseResult.Of('F', "oo")));

    [Fact]
    public void Upper_parses_A() => Upper("A").Should().BeEquivalentTo(Ok(ParseResult.Of('A', "")));

    [Fact]
    public void Upper_parses_Z() => Upper("Z").Should().BeEquivalentTo(Ok(ParseResult.Of('Z', "")));

    [Fact]
    public void String_of_empty_string_parses_empty_input() =>
        String("")("").Should().BeEquivalentTo(Ok(ParseResult.Of("", "")));

    [Fact]
    public void String_parses_input_that_matches() =>
        String("hello")("hello world")
            .Should()
            .BeEquivalentTo(Ok(ParseResult.Of("hello", " world")));

    [Fact]
    public void String_does_not_parse_input_that_does_not_match() =>
        String("hello")("helicopter123456")
            .Should()
            .BeEquivalentTo(Error<IParseResult<char>>("Invalid input: icopter123..."));

    [Fact]
    public void Integer_does_not_parse_non_integer() =>
        Integer("foo").Should().BeEquivalentTo(Error<IParseResult<char>>("Invalid input: foo"));

    [Fact]
    public void Integer_parses_positive_number() =>
        Integer("10").Should().BeEquivalentTo(Ok(ParseResult.Of(10, "")));

    [Fact]
    public void Integer_parses_negative_number() =>
        Integer("-10").Should().BeEquivalentTo(Ok(ParseResult.Of(-10, "")));

    [Fact]
    public void Or_short_circuits()
    {
        IResult<IParseResult<string>> ThrowingParser(string input)
        {
            throw new UnreachableException();
        }

        Parser<string> colour = String("yellow").Or(ThrowingParser);

        colour("yellow").Should().BeEquivalentTo(Ok(ParseResult.Of("yellow", "")));
    }

    [Fact]
    public void Or_calls_second_parser_when_first_does_not_parse_input()
    {
        Parser<string> colour = String("yellow").Or(String("orange"));

        colour("orange").Should().BeEquivalentTo(Ok(ParseResult.Of("orange", "")));
    }
}
