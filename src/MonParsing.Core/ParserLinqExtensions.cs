namespace MonParsing.Core;

public static class ParserLinqExtensions
{
    public static Parser<U> Select<T, U>(this Parser<T> parser, Func<T, U> selector) =>
        parser.Map(selector);

    public static Parser<U> SelectMany<T, U>(this Parser<T> parser, Func<T, Parser<U>> selector) =>
        parser.Bind(selector);

    public static Parser<U> SelectMany<T, U, V>(
        this Parser<T> parser,
        Func<T, Parser<V>> k,
        Func<T, V, U> s
    ) => parser.SelectMany(t => k(t).Select(v => s(t, v)));

    public static Parser<T> Where<T>(this Parser<T> parser, Predicate<T> predicate) =>
        parser.Sat(predicate);
}
