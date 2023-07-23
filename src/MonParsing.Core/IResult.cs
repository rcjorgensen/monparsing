namespace MonParsing.Core;

public interface IResult<out T>
{
    T? Value { get; }

    string? Error { get; }

    IResult<U> AndThen<U>(Func<T, IResult<U>> selector);

    IResult<U> Map<U>(Func<T, U> selector);
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
        if (Value != null)
        {
            return selector(Value);
        }

        return new Result<U>(Error);
    }

    public IResult<U> Map<U>(Func<T, U> selector)
    {
        if (Value != null)
        {
            return new Result<U>(selector(Value));
        }

        return new Result<U>(Error);
    }
}

public static class Result
{
    public static IResult<T> Ok<T>(T value) => new Result<T>(value);

    public static IResult<T> Error<T>(string error) => new Result<T>(error);
}
