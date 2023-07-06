namespace MonParsing.Core;

public delegate (T, string)[] Parser<T>(string input);

public static class Parser
{
    private static (T, string)[] GetEmptyArray<T>() => Array.Empty<(T, string)>();

    public static Parser<T> Result<T>(T value) => (string input) => new[] { (value, input) };

    public static Parser<T> Zero<T>() => (string _) => Array.Empty<(T, string)>();

    public static Parser<char> Item = (string input) =>
        input.ToCharArray() switch
        {
            [] => GetEmptyArray<char>(),
            [var x, .. var xs] => new[] { (x, new string(xs)) }
        };

    public static Parser<U> Bind<T, U>(this Parser<T> parser, Func<T, Parser<U>> func) =>
        (string input) => parser(input).SelectMany(res => func(res.Item1)(res.Item2)).ToArray();

    public static Parser<(T, U)> Seq<T, U>(Parser<T> parser1, Parser<U> parser2) =>
        parser1.Bind(x => parser2.Bind(y => Result((x, y))));

    public static Parser<char> Sat(Predicate<char> predicate) =>
        Item.Bind(x => predicate(x) ? Result(x) : Zero<char>());

    public static Parser<char> Char(char x) => Sat(y => x == y);

    public static Parser<char> Digit = Sat(x => '0' <= x && x <= '9');
}
