using static MonParsing.Core.Option;
using static MonParsing.Core.Result;

namespace MonParsing.Core;

public delegate IResult<IStatePair<T>> Parser<out T>(string input);

public static class Parser
{
    public static Parser<T> Result<T>(T value) => (string input) => Ok(StatePair.Of(value, input));

    public static Parser<U> Map<T, U>(this Parser<T> parser, Func<T, U> selector) =>
        (string input) => from pr in parser(input) select (from r in pr select selector(r));

    public static Parser<U> AndThen<T, U>(this Parser<T> parser, Func<T, Parser<U>> selector) =>
        (string input) =>
            from pr1 in parser(input)
            from pr2 in selector(pr1.Result)(pr1.Input)
            select pr2;

    public static Parser<U> And<T, U>(this Parser<T> parser1, Parser<U> parser2) =>
        (string input) => from pr1 in parser1(input) from pr2 in parser2(pr1.Input) select pr2;

    public static Parser<T> If<T>(
        this Parser<T> parser,
        Predicate<T> predicate,
        string? expectation = null
    ) =>
        (string input) =>
        {
            var result = parser(input);

            return result.AndThen(
                statePair =>
                    predicate(statePair.Result)
                        ? Ok(statePair)
                        : Error<IStatePair<T>>(
                            $"Expectation: {expectation}, Actual: {statePair.Result}, Input before: {input}, Input after: {statePair.Input}"
                        )
            );
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

    public static Parser<IOption<T>> ZeroOrOne<T>(Parser<T> parser) =>
        parser.Map(Some).Or(Result(None<T>()));

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
        Parser<T> parser,
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
    ) => OneOrMoreSeparated(parser, separator).Or(Result(Enumerable.Empty<T>()));

    public static Parser<T> Or<T>(this Parser<T> parser1, Parser<T> parser2) =>
        (string input) =>
        {
            var result1 = parser1(input);

            if (result1.Value != null)
            {
                return result1;
            }

            var result2 = parser2(input);

            return result2.Value != null
                ? result2
                : Error<IStatePair<T>>($"{result1.Error} OR {result2.Error}");
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
                return Ok(StatePair.Of(c, input[1..]));
            }

            return Error<IStatePair<char>>("Input was empty");
        };

    public static Parser<char> If(Predicate<char> predicate, string? expectation = null) =>
        If(Item, predicate, expectation);

    public static Parser<char> Char(char x) => If(y => x == y, expectation: $"The character {x}");

    public static Parser<string> String(Parser<IEnumerable<char>> parser) =>
        from chs in parser
        select string.Concat(chs);

    public static Parser<string> String(Parser<char> parser) =>
        from ch in parser
        select ch.ToString();

    public static Parser<string> String(string str) => String(str.For(Char));

    //
    // Some useful everyday parsers
    //

    public static Parser<char> PositiveDigit = If(
        x => '1' <= x && x <= '9',
        expectation: "A digit between 1 and 9"
    );

    public static Parser<char> Digit = Char('0').Or(PositiveDigit);

    public static Parser<char> Lower = If(
        x => 'a' <= x && x <= 'z',
        expectation: "A lowercase character between a and z"
    );

    public static Parser<char> Upper = If(
        x => 'A' <= x && x <= 'Z',
        expectation: "An upper case character between A and Z"
    );

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

    //
    // LINQ support
    //

    public static Parser<U> Select<T, U>(this Parser<T> source, Func<T, U> selector) =>
        source.Map(selector);

    public static Parser<U> SelectMany<T, U>(this Parser<T> source, Func<T, Parser<U>> selector) =>
        source.AndThen(selector);

    public static Parser<U> SelectMany<T, U, V>(
        this Parser<T> monad,
        Func<T, Parser<V>> k,
        Func<T, V, U> s
    ) => monad.SelectMany(t => k(t).Select(v => s(t, v)));

    public static Parser<T> Where<T>(this Parser<T> parser, Predicate<T> predicate) =>
        parser.If(predicate);
}
