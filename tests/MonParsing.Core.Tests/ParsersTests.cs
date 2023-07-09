using System.Diagnostics;
using FluentAssertions;

namespace MonParsing.Core.Tests;

public class ParsersTests
{
    [Fact]
    public void Zero_never_parses() => Parsers.Zero<char>("foo").Should().BeEmpty();

    [Fact]
    public void Result_does_not_consume_from_input_and_returns_given_value() =>
        Parsers.Result("foo")("bar").Should().Equal(("foo", "bar"));

    [Fact]
    public void Item_does_not_parse_empty_input() => Parsers.Item("").Should().BeEmpty();

    [Fact]
    public void Item_parses_first_item() => Parsers.Item("foo").Should().Equal(('f', "oo"));

    [Fact]
    public void Seq_does_not_parse_empty_input() =>
        Parsers.Item.Seq(Parsers.Item)("").Should().BeEmpty();

    [Fact]
    public void Seq_does_not_parse_input_with_length_1() =>
        Parsers.Item.Seq(Parsers.Item)("f").Should().BeEmpty();

    [Fact]
    public void Seq_does_parses_input_with_length_greater_than_1() =>
        Parsers.Item.Seq(Parsers.Item)("foo").Should().Equal((('f', 'o'), "o"));

    [Fact]
    public void Sat_does_not_parse_input_when_predicate_is_not_satisfied() =>
        Parsers.Sat(c => c == 'f')("bar").Should().BeEmpty();

    [Fact]
    public void Sat_parses_input_when_predicate_is_satisfied() =>
        Parsers.Sat(c => c == 'f')("foo").Should().Equal(('f', "oo"));

    [Fact]
    public void Char_does_not_parse_input_when_first_char_does_not_match() =>
        Parsers.Char('f')("bar").Should().BeEmpty();

    [Fact]
    public void Char_parses_input_when_first_char_matches() =>
        Parsers.Char('f')("foo").Should().Equal(('f', "oo"));

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
        Parsers.Digit(digit).Should().Equal((Convert.ToChar(digit), ""));

    [Fact]
    public void Digit_does_not_parse_non_digit() => Parsers.Digit("foo").Should().BeEmpty();

    [Fact]
    public void Lower_does_not_parse_non_letter() => Parsers.Lower("_oo").Should().BeEmpty();

    [Fact]
    public void Lower_does_not_parse_upper_case_character() =>
        Parsers.Lower("Foo").Should().BeEmpty();

    [Fact]
    public void Lower_parses_lower_case_character() =>
        Parsers.Lower("foo").Should().Equal(('f', "oo"));

    [Fact]
    public void Lower_parses_a() => Parsers.Lower("a").Should().Equal(('a', ""));

    [Fact]
    public void Lower_parses_z() => Parsers.Lower("z").Should().Equal(('z', ""));

    [Fact]
    public void Upper_does_not_parse_non_letter() => Parsers.Upper("_oo").Should().BeEmpty();

    [Fact]
    public void Upper_does_not_parse_lower_case_character() =>
        Parsers.Upper("foo").Should().BeEmpty();

    [Fact]
    public void Upper_parses_upper_case_character() =>
        Parsers.Upper("Foo").Should().Equal(('F', "oo"));

    [Fact]
    public void Upper_parses_A() => Parsers.Upper("A").Should().Equal(('A', ""));

    [Fact]
    public void Upper_parses_Z() => Parsers.Upper("Z").Should().Equal(('Z', ""));

    [Fact]
    public void Word_parses_input_with_multiple_results() =>
        Parsers.Word("Yes!").Should().Equal(("Yes", "!"), ("Ye", "s!"), ("Y", "es!"), ("", "Yes!"));

    [Fact]
    public void String_of_empty_string_parses_empty_input() =>
        Parsers.String("")("").Should().Equal(("", ""));

    [Fact]
    public void String_parses_input_that_matches() =>
        Parsers.String("hello")("hello world").Should().Equal(("hello", " world"));

    [Fact]
    public void String_does_not_parse_input_that_does_not_match() =>
        Parsers.String("hello")("helicopter").Should().BeEmpty();

    [Fact]
    public void Int_does_not_parse_non_integer() => Parsers.Int("foo").Should().BeEmpty();

    [Fact]
    public void Int_parses_positive_number() =>
        Parsers.Int("10").Should().Equal((10, ""), (1, "0"));

    [Fact]
    public void Int_parses_negative_number() =>
        Parsers.Int("-10").Should().Equal((-10, ""), (-1, "0"));

    [Fact]
    public void PlusD_short_circuits()
    {
        IEnumerable<(string, string)> ThrowingParser(string input)
        {
            throw new UnreachableException();
        }

        Parser<string> colour = Parsers.String("yellow").PlusD(ThrowingParser);

        colour("yellow").ToList().Should().Equal(("yellow", ""));
    }

    [Fact]
    public void PlusD_calls_second_parser_when_first_does_not_parse_input()
    {
        Parser<string> colour = Parsers.String("yellow").PlusD(Parsers.String("orange"));

        colour("orange").ToList().Should().Equal(("orange", ""));
    }

    [Fact]
    public void Plus_does_not_short_circut()
    {
        var expectedMessage = "This exception should be thrown";
        IEnumerable<(string, string)> ThrowingParser(string input)
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
