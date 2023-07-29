namespace MonParsing.Core;

public interface IResult<out T>
{
    T? Value { get; }

    string? Error { get; }

    IResult<U> AndThen<U>(Func<T, IResult<U>> selector);

    IResult<U> Map<U>(Func<T, U> selector);

    IResult<U> Select<U>(Func<T, U> selector) => Map(selector);

    IResult<U> SelectMany<U>(Func<T, IResult<U>> selector) => AndThen(selector);

    IResult<U> SelectMany<U, V>(Func<T, IResult<V>> k, Func<T, V, U> s) =>
        SelectMany(t => k(t).Select(v => s(t, v)));
}

internal class Result<T> : IResult<T>
{
    public Result(T value)
    {
        Value = value;
    }

    public Result(string? error)
    {
        Error = error;
    }

    public T? Value { get; }

    public string? Error { get; }

    public IResult<U> AndThen<U>(Func<T, IResult<U>> selector)
    {
        return Value != null ? selector(Value) : new Result<U>(Error);
    }

    public IResult<U> Map<U>(Func<T, U> selector)
    {
        return Value != null ? new Result<U>(selector(Value)) : (IResult<U>)new Result<U>(Error);
    }
}

public static class Result
{
    public static IResult<T> Ok<T>(T value) => new Result<T>(value);

    public static IResult<T> Error<T>(string error) => new Result<T>(error);
}
