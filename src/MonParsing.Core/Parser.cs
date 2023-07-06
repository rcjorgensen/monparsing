namespace MonParsing.Core;

public delegate IEnumerable<(T, string)> Parser<T>(string input);

public static class Parser
{
    private static IEnumerable<(T, string)> Empty<T>() => Enumerable.Empty<(T, string)>();

    public static Parser<T> Result<T>(T value) =>
        (string input) => new List<(T, string)> { (value, input) };

    public static Parser<T> Zero<T>() => (string _) => Empty<T>();

    public static Parser<char> Item = (string input) =>
        input.ToCharArray() switch
        {
            [] => Empty<char>(),
            [var x, .. var xs] => new List<(char, string)> { (x, new string(xs)) }
        };

    public static Parser<U> Map<T, U>(this Parser<T> parser, Func<T, U> func) =>
        (string input) => parser(input).Select(res => (func(res.Item1), res.Item2));

    public static Parser<U> Bind<T, U>(this Parser<T> parser, Func<T, Parser<U>> func) =>
        (string input) => parser(input).SelectMany(res => func(res.Item1)(res.Item2));

    public static Parser<(T, U)> Seq<T, U>(this Parser<T> parser1, Parser<U> parser2) =>
        parser1.Bind(x => parser2.Bind(y => Result((x, y))));

    public static Parser<T> Sat<T>(this Parser<T> parser, Predicate<T> predicate) =>
        parser.Bind(x => predicate(x) ? Result(x) : Zero<T>());

    public static Parser<char> Sat(Predicate<char> predicate) =>
        from x in Item
        where predicate(x)
        select x;

    public static Parser<char> Char(char x) => Sat(y => x == y);

    public static Parser<char> Digit = Sat(x => '0' <= x && x <= '9');

    public static Parser<char> Lower = Sat(x => 'a' <= x && x <= 'z');

    public static Parser<char> Upper = Sat(x => 'A' <= x && x <= 'Z');

    public static Parser<T> Plus<T>(this Parser<T> parser1, Parser<T> parser2) =>
        (string input) => parser1(input).Concat(parser2(input));

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

    public static Parser<IEnumerable<T>> Many<T>(this Parser<T> parser) =>
        (from x in parser from xs in parser.Many() select xs.Prepend(x)).Plus(
            Result(Enumerable.Empty<T>())
        );

    public static Parser<IEnumerable<T>> Many1<T>(this Parser<T> parser) =>
        from x in parser
        from xs in parser.Many()
        select xs.Prepend(x);

    public static Parser<int> Nat = from xs in Digit.Many1() select int.Parse(xs.ToArray());

    public static Parser<int> Int = (from _ in Char('-') from n in Nat select -n).Plus(Nat);
}
