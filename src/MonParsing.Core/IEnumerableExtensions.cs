namespace MonParsing.Core;

public static class IEnumerableExtensions
{
    public static T FoldL<T, U>(this IEnumerable<U> collection, T seed, Func<T, U, T> func) =>
        FoldL(func, seed, collection);

    public static T FoldL<T, U>(Func<T, U, T> func, T seed, IEnumerable<U> collection) =>
        collection.ToArray() switch
        {
            [] => seed,
            [var x, .. var xs] => FoldL(func, func(seed, x), xs)
        };

    public static U FoldR<T, U>(this IEnumerable<T> collection, U seed, Func<T, U, U> func) =>
        FoldR(func, seed, collection);

    public static U FoldR<T, U>(Func<T, U, U> func, U seed, IEnumerable<T> collection) =>
        collection.ToArray() switch
        {
            [] => seed,
            [var x, .. var xs] => func(x, FoldR(func, seed, xs))
        };
}
