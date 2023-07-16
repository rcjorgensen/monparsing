namespace MonParsing.Core;

public delegate IEnumerable<IParseResult<T>> Parser<out T>(string input);

public static class Parsers
{
    public static IEnumerable<IParseResult<T>> Zero<T>(string input)
    {
        yield break;
    }

    private static IEnumerable<IParseResult<T>> Result<T>(T value, string input)
    {
        yield return ParseResult.Default(value, input);
    }

    public static Parser<T> Result<T>(T value) => (string input) => Result(value, input);

    public static Parser<U> Map<T, U>(this Parser<T> parser, Func<T, U> func) =>
        (string input) => parser(input).Select(res => res.Map(func));

    public static Parser<U> Bind<T, U>(this Parser<T> parser, Func<T, Parser<U>> func) =>
        (string input) => parser(input).SelectMany(res => func(res.Result)(res.Input));

    public static Parser<T> Sat<T>(this Parser<T> parser, Predicate<T> predicate) =>
        Bind(parser, x => predicate(x) ? Result(x) : Zero<T>);

    public static Parser<IEnumerable<U>> For<T, U>(
        this IEnumerable<T> collection,
        Func<T, Parser<U>> func
    )
    {
        var result = Result(Enumerable.Empty<U>());
        foreach (var item in collection)
        {
            result = Bind(result, xs => Bind(func(item), x => Result(xs.Append(x))));
        }
        return result;
    }

    private static IEnumerable<IParseResult<T>> Plus<T>(
        Parser<T> parser1,
        Parser<T> parser2,
        string input
    )
    {
        foreach (var item in parser1(input))
        {
            yield return item;
        }

        foreach (var item in parser2(input))
        {
            yield return item;
        }
    }

    public static Parser<T> Plus<T>(this Parser<T> parser1, Parser<T> parser2) =>
        (string input) => Plus(parser1, parser2, input);

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

    private static IEnumerable<IParseResult<T>> First<T>(Parser<T> parser, string input)
    {
        foreach (var item in parser(input))
        {
            yield return item;
            yield break;
        }
    }

    public static Parser<T> First<T>(this Parser<T> parser) =>
        (string input) => First(parser, input);

    public static Parser<T> Or<T>(this Parser<T> parser1, Parser<T> parser2) =>
        parser1.Plus(parser2).First();

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

    public static IEnumerable<IParseResult<char>> Item(string input)
    {
        foreach (var c in input)
        {
            yield return ParseResult.Default(c, input.Substring(1));
            yield break;
        }
    }

    public static Parser<char> Sat(Predicate<char> predicate) => Sat(Item, predicate);

    public static Parser<char> Char(char x) => Sat(y => x == y);

    public static Parser<string> Convert(this Parser<IEnumerable<char>> parser) =>
        from chs in parser
        select string.Concat(chs);

    public static Parser<string> String(string str) => str.For(ch => Char(ch)).Convert();

    //
    // Some useful everyday parsers
    //

    public static Parser<char> Digit = Sat(x => '0' <= x && x <= '9');

    public static Parser<char> Lower = Sat(x => 'a' <= x && x <= 'z');

    public static Parser<char> Upper = Sat(x => 'A' <= x && x <= 'Z');

    public static Parser<char> Letter = Lower.Plus(Upper);

    public static Parser<char> Alphanumeric = Letter.Plus(Digit);

    public static Parser<int> PositiveInteger =
        from ds in OneOrMore(Digit)
        select int.Parse(string.Concat(ds));

    public static Parser<int> NegativeInteger =
        from m in Char('-')
        from n in PositiveInteger
        select -n;

    public static Parser<int> Integer = PositiveInteger.Or(NegativeInteger);
}
