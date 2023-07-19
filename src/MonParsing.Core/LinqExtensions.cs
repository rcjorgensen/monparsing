using static MonParsing.Core.Result;

namespace MonParsing.Core;

public static class LinqExtensions
{
    public static IResult<U> Select<T, U>(this IResult<T> source, Func<T, U> selector) =>
        source.Map(selector);

    public static IResult<U> SelectMany<T, U>(
        this IResult<T> source,
        Func<T, IResult<U>> selector
    ) => source.AndThen(selector);

    public static IResult<U> SelectMany<T, U, V>(
        this IResult<T> monad,
        Func<T, IResult<V>> k,
        Func<T, V, U> s
    ) => monad.SelectMany(t => k(t).Select(v => s(t, v)));

    public static IParseResult<U> Select<T, U>(this IParseResult<T> source, Func<T, U> selector) =>
        source.Map(selector);

    public static IParseResult<U> SelectMany<T, U>(
        this IParseResult<T> source,
        Func<T, IParseResult<U>> selector
    ) => source.AndThen(selector);

    public static IParseResult<U> SelectMany<T, U, V>(
        this IParseResult<T> monad,
        Func<T, IParseResult<V>> k,
        Func<T, V, U> s
    ) => monad.SelectMany(t => k(t).Select(v => s(t, v)));

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
        parser.Sat(predicate);
}
