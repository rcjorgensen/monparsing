using System.Diagnostics;

using FluentAssertions;
using static MonParsing.Core.Parser;

namespace MonParsing.Core.Tests;

[UsesVerify]
public class ParserTests
{
    [Fact]
    public void Result_does_not_consume_from_input_and_returns_given_value() =>
        Result("foo")("bar")
            .Should()
            .BeEquivalentTo(new { Value = new { Result = "foo", Input = "bar" } });

    [Fact]
    public Task Item_does_not_parse_empty_input() => Verify(Item(""));

    [Fact]
    public void Item_parses_first_item() =>
        Item("foo").Should().BeEquivalentTo(new { Value = new { Result = 'f', Input = "oo" } });

    [Fact]
    public Task If_does_not_parse_input_when_predicate_is_not_satisfied() =>
        Verify(If(c => c == 'f', expectation: "f")("bar"));

    [Fact]
    public void If_parses_input_when_predicate_is_satisfied() =>
        If(c => c == 'f')("foo")
            .Should()
            .BeEquivalentTo(new { Value = new { Result = 'f', Input = "oo" } });

    [Fact]
    public Task Char_does_not_parse_input_when_first_char_does_not_match() =>
        Verify(Char('f')("bar"));

    [Fact]
    public void Char_parses_input_when_first_char_matches() =>
        Char('f')("foo")
            .Should()
            .BeEquivalentTo(new { Value = new { Result = 'f', Input = "oo" } });

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
        Digit(digit)
            .Should()
            .BeEquivalentTo(new { Value = new { Result = Convert.ToChar(digit), Input = "" } });

    [Fact]
    public Task Digit_does_not_parse_non_digit() => Verify(Digit("foo"));

    [Fact]
    public Task Lower_does_not_parse_non_letter() => Verify(Lower("_oo"));

    [Fact]
    public Task Lower_does_not_parse_upper_case_character() => Verify(Lower("Foo"));

    [Fact]
    public void Lower_parses_lower_case_character() =>
        Lower("foo").Should().BeEquivalentTo(new { Value = new { Result = 'f', Input = "oo" } });

    [Fact]
    public void Lower_parses_a() =>
        Lower("a").Should().BeEquivalentTo(new { Value = new { Result = 'a', Input = "" } });

    [Fact]
    public void Lower_parses_z() =>
        Lower("z").Should().BeEquivalentTo(new { Value = new { Result = 'z', Input = "" } });

    [Fact]
    public Task Upper_does_not_parse_non_letter() => Verify(Upper("_oo"));

    [Fact]
    public Task Upper_does_not_parse_lower_case_character() => Verify(Upper("foo"));

    [Fact]
    public void Upper_parses_upper_case_character() =>
        Upper("Foo").Should().BeEquivalentTo(new { Value = new { Result = 'F', Input = "oo" } });

    [Fact]
    public void Upper_parses_A() =>
        Upper("A").Should().BeEquivalentTo(new { Value = new { Result = 'A', Input = "" } });

    [Fact]
    public void Upper_parses_Z() =>
        Upper("Z").Should().BeEquivalentTo(new { Value = new { Result = 'Z', Input = "" } });

    [Fact]
    public void String_of_empty_string_parses_empty_input() =>
        String("")("").Should().BeEquivalentTo(new { Value = new { Result = "", Input = "" } });

    [Fact]
    public void String_parses_input_that_matches() =>
        String("hello")("hello world")
            .Should()
            .BeEquivalentTo(new { Value = new { Result = "hello", Input = " world" } });

    [Fact]
    public Task String_does_not_parse_input_that_does_not_match() =>
        Verify(String("hello")("helicopter123456"));

    [Fact]
    public Task Integer_does_not_parse_non_integer() => Verify(Integer("foo"));

    [Fact]
    public void Integer_parses_positive_number() =>
        Integer("10").Should().BeEquivalentTo(new { Value = new { Result = 10, Input = "" } });

    [Fact]
    public void Integer_parses_negative_number() =>
        Integer("-10").Should().BeEquivalentTo(new { Value = new { Result = -10, Input = "" } });

    [Fact]
    public void Or_short_circuits()
    {
        static IResult<IStatePair<string>> ThrowingParser(string input)
        {
            throw new UnreachableException();
        }

        Parser<string> colour = String("yellow").Or(ThrowingParser);

        colour("yellow")
            .Should()
            .BeEquivalentTo(new { Value = new { Result = "yellow", Input = "" } });
    }

    [Fact]
    public void Or_calls_second_parser_when_first_does_not_parse_input()
    {
        Parser<string> colour = String("yellow").Or(String("orange"));

        colour("orange")
            .Should()
            .BeEquivalentTo(new { Value = new { Result = "orange", Input = "" } });
    }

    [Fact]
    public void Or_calls_third_parser_when_first_two_fails() =>
        Or(String("yellow"), String("orange"), String("blue"))("blue")
            .Should()
            .BeEquivalentTo(new { Value = new { Result = "blue", Input = "" } });
}
