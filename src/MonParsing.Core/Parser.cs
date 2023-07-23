using static MonParsing.Core.Result;

namespace MonParsing.Core;

public delegate IResult<IParseResult<T>> Parser<out T>(string input);

public static class Parser
{
    public static IResult<IParseResult<T>> Zero<T>(string input)
    {
        var maxLength = 10;
        var inputTruncated =
            input.Length > maxLength ? input.Substring(0, maxLength) + "..." : input;

        var error = $"Invalid input: {inputTruncated}";

        return Error<IParseResult<T>>(error);
    }

    public static Parser<T> Result<T>(T value) =>
        (string input) =>
        {
            return Ok(ParseResult.Of(value, input));
        };

    public static Parser<U> Map<T, U>(this Parser<T> parser, Func<T, U> selector) =>
        (string input) => from pr in parser(input) select (from r in pr select selector(r));

    public static Parser<U> AndThen<T, U>(this Parser<T> parser, Func<T, Parser<U>> selector) =>
        (string input) =>
            from pr1 in parser(input)
            from pr2 in selector(pr1.Result)(pr1.Input)
            select pr2;

    public static Parser<U> And<T, U>(this Parser<T> parser1, Parser<U> parser2) =>
        (string input) => from pr1 in parser1(input) from pr2 in parser2(pr1.Input) select pr2;

    public static Parser<T> Sat<T>(this Parser<T> parser, Predicate<T> predicate) =>
        (string input) =>
        {
            var result = parser(input);
            if (result.Value != null && predicate(result.Value.Result))
            {
                return Ok(ParseResult.Of(result.Value.Result, result.Value.Input));
            }

            return Zero<T>(input);
        };

    public static Parser<IEnumerable<U>> For<T, U>(
        this IEnumerable<T> collection,
        Func<T, Parser<U>> func
    )
    {
        var result = Result(Enumerable.Empty<U>());
        foreach (var item in collection)
        {
            result = AndThen(result, xs => AndThen(func(item), x => Result(xs.Append(x))));
        }
        return result;
    }

    // TODO: Rewrite without recursion
    public static Parser<IEnumerable<T>> ZeroOrMore<T>(Parser<T> parser) =>
        (from x in parser from xs in ZeroOrMore(parser) select xs.Prepend(x)).Or(
            Result(Enumerable.Empty<T>())
        );

    public static Parser<IEnumerable<T>> OneOrMore<T>(Parser<T> parser) =>
        from x in parser
        from xs in ZeroOrMore(parser)
        select xs.Prepend(x);

    public static Parser<IEnumerable<T>> OneOrMoreSeparated<T, U>(
        this Parser<T> parser,
        Parser<U> separator
    ) =>
        from x in parser
        from xs in ZeroOrMore(from s in separator from y in parser select y)
        select xs.Prepend(x);

    public static Parser<U> Bracket<T, U, V>(Parser<T> open, Parser<U> parser, Parser<V> close) =>
        from o in open
        from x in parser
        from c in close
        select x;

    public static Parser<IEnumerable<T>> ZeroOrMoreSeparated<T, U>(
        Parser<T> parser,
        Parser<U> separator
    ) => parser.OneOrMoreSeparated(separator).Or(Result(Enumerable.Empty<T>()));

    public static Parser<T> Or<T>(this Parser<T> parser1, Parser<T> parser2) =>
        (string input) =>
        {
            var result1 = parser1(input);

            if (result1.Value != null)
            {
                return result1;
            }

            return parser2(input);
        };

    public static Parser<T> Or<T>(Parser<T> parser1, params Parser<T>[] parsers)
    {
        var result = parser1;
        foreach (var parser in parsers)
        {
            result = Or(result, parser);
        }
        return result;
    }

    //
    // Useful `char` and `string` parsers
    //

    public static Parser<char> Item =>
        (string input) =>
        {
            foreach (var c in input)
            {
                return Ok(ParseResult.Of(c, input.Substring(1)));
            }

            return Error<IParseResult<char>>("Empty input");
        };

    public static Parser<char> Sat(Predicate<char> predicate) => Sat(Item, predicate);

    public static Parser<char> Char(char x) => Sat(y => x == y);

    public static Parser<string> Convert(this Parser<IEnumerable<char>> parser) =>
        from chs in parser
        select string.Concat(chs);

    public static Parser<string> Convert(this Parser<char> parser) =>
        from ch in parser
        select ch.ToString();

    public static Parser<string> String(string str) => str.For(ch => Char(ch)).Convert();

    //
    // Some useful everyday parsers
    //

    public static Parser<char> PositiveDigit = Sat(x => '1' <= x && x <= '9');

    public static Parser<char> Digit = Char('0').Or(PositiveDigit);

    public static Parser<char> Lower = Sat(x => 'a' <= x && x <= 'z');

    public static Parser<char> Upper = Sat(x => 'A' <= x && x <= 'Z');

    public static Parser<char> Letter = Lower.Or(Upper);

    public static Parser<char> Alphanumeric = Letter.Or(Digit);

    public static Parser<int> PositiveInteger =
        from ds in OneOrMore(Digit)
        select int.Parse(string.Concat(ds));

    public static Parser<int> NegativeInteger =
        from m in Char('-')
        from n in PositiveInteger
        select -n;

    public static Parser<int> Integer = PositiveInteger.Or(NegativeInteger);
}
