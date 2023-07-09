namespace MonParsing.Core;

public delegate IEnumerable<(T, string)> Parser<T>(string input);

public static class Parsers
{
    public static IEnumerable<(T, string)> Zero<T>(string input)
    {
        yield break;
    }

    private static IEnumerable<(T, string)> Result<T>(T value, string input)
    {
        yield return (value, input);
    }

    public static Parser<T> Result<T>(T value) => (string input) => Result(value, input);

    private static IEnumerable<(char, string)> ItemM(string input)
    {
        foreach (var c in input)
        {
            yield return (c, input.Substring(1));
            yield break;
        }
    }

    public static Parser<char> Item = ItemM;

    public static Parser<U> Map<T, U>(this Parser<T> parser, Func<T, U> func) =>
        (string input) => parser(input).Select(res => (func(res.Item1), res.Item2));

    public static Parser<U> Bind<T, U>(this Parser<T> parser, Func<T, Parser<U>> func) =>
        (string input) => parser(input).SelectMany(res => func(res.Item1)(res.Item2));

    public static Parser<(T, U)> Seq<T, U>(this Parser<T> parser1, Parser<U> parser2) =>
        Bind(parser1, x => Bind(parser2, y => Result((x, y))));

    public static Parser<T> Sat<T>(this Parser<T> parser, Predicate<T> predicate) =>
        Bind(parser, x => predicate(x) ? Result(x) : Zero<T>);

    public static Parser<char> Sat(Predicate<char> predicate) => Sat(ItemM, predicate);

    public static Parser<char> Char(char x) => Sat(y => x == y);

    public static Parser<char> Digit = Sat(x => '0' <= x && x <= '9');

    public static Parser<char> Lower = Sat(x => 'a' <= x && x <= 'z');

    public static Parser<char> Upper = Sat(x => 'A' <= x && x <= 'Z');

    public static IEnumerable<(T, string)> Plus<T>(
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

    public static Parser<char> Letter = Lower.Plus(Upper);

    public static Parser<char> Alphanum = Letter.Plus(Digit);

    public static Parser<string> Word = from w in Many(Letter) select new string(w.ToArray());

    public static Parser<string> String(string s) =>
        s.ToCharArray() switch
        {
            [] => Result(string.Empty),
            [var x, .. var xs]
                => from _x in Char(x)
                from _xs in String(new string(xs))
                select new string(xs.Prepend(x).ToArray())
        };

    public static Parser<IEnumerable<T>> Many1<T>(this Parser<T> parser) =>
        from x in parser
        from xs in parser.Many()
        select xs.Prepend(x);

    public static Parser<int> Nat = from xs in Digit.Many1() select int.Parse(xs.ToArray());

    public static Parser<int> Int = (from _ in Char('-') from n in Nat select -n).Plus(Nat);

    public static Parser<IEnumerable<T>> SepBy1<T, U>(this Parser<T> parser, Parser<U> separator) =>
        from x in parser
        from xs in Many(from s in separator from y in parser select y)
        select xs.Prepend(x);

    public static Parser<U> Bracket<T, U, V>(Parser<T> open, Parser<U> parser, Parser<V> close) =>
        from o in open
        from x in parser
        from c in close
        select x;

    public static Parser<IEnumerable<T>> SepBy<T, U>(this Parser<T> parser, Parser<U> separator) =>
        parser.SepBy1(separator).Plus(Result(Enumerable.Empty<T>()));

    public static Parser<IEnumerable<T>> Many<T>(this Parser<T> parser) =>
        (from x in parser from xs in parser.Many() select xs.Prepend(x)).Plus(
            Result(Enumerable.Empty<T>())
        );

    private static IEnumerable<(T, string)> First<T>(Parser<T> parser, string input)
    {
        foreach (var item in parser(input))
        {
            yield return item;
            yield break;
        }
    }

    public static Parser<T> First<T>(this Parser<T> parser) =>
        (string input) => First(parser, input);

    public static Parser<T> PlusD<T>(this Parser<T> parser1, Parser<T> parser2) =>
        parser1.Plus(parser2).First();
}
