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
    public void Lower_does_not_parse_non_letter() => Parser.Lower("_oo").Should().BeEmpty();

    [Fact]
    public void Lower_does_not_parse_upper_case_character() =>
        Parser.Lower("Foo").Should().BeEmpty();

    [Fact]
    public void Lower_parses_lower_case_character() =>
        Parser.Lower("foo").Should().Equal(('f', "oo"));

    [Fact]
    public void Lower_parses_a() => Parser.Lower("a").Should().Equal(('a', ""));

    [Fact]
    public void Lower_parses_z() => Parser.Lower("z").Should().Equal(('z', ""));

    [Fact]
    public void Upper_does_not_parse_non_letter() => Parser.Upper("_oo").Should().BeEmpty();

    [Fact]
    public void Upper_does_not_parse_lower_case_character() =>
        Parser.Upper("foo").Should().BeEmpty();

    [Fact]
    public void Upper_parses_upper_case_character() =>
        Parser.Upper("Foo").Should().Equal(('F', "oo"));

    [Fact]
    public void Upper_parses_A() => Parser.Upper("A").Should().Equal(('A', ""));

    [Fact]
    public void Upper_parses_Z() => Parser.Upper("Z").Should().Equal(('Z', ""));

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

    [Fact]
    public void Int_does_not_parse_non_integer() => Parser.Int("foo").Should().BeEmpty();

    [Fact]
    public void Int_parses_positive_number() => Parser.Int("10").Should().Equal((10, ""), (1, "0"));

    [Fact]
    public void Int_parses_negative_number() =>
        Parser.Int("-10").Should().Equal((-10, ""), (-1, "0"));
}
