using static MonParsing.Core.Logger;

namespace MonParsing.Core;

public interface IResult<out T>
{
    T? Value { get; }

    string? Error { get; }

    IResult<U> Map<U>(Func<T, U> selector);

    IResult<U> AndThen<U>(Func<T, IResult<U>> selector);

    IResult<U> And<U>(IResult<U> other);

    ILogger<T> ToLogger();
}

internal class Result<T> : IResult<T>
{
    public T? Value { get; }
    public string? Error { get; }

    public Result(T value)
    {
        Value = value;
    }

    public Result(string? error)
    {
        Error = error;
    }

    public IResult<U> Map<U>(Func<T, U> selector)
    {
        if (Value != null)
        {
            return new Result<U>(selector(Value));
        }

        return new Result<U>(Error);
    }

    public IResult<U> AndThen<U>(Func<T, IResult<U>> selector)
    {
        if (Value != null)
        {
            return selector(Value);
        }

        return new Result<U>(Error);
    }

    public IResult<U> And<U>(IResult<U> other)
    {
        if (Value != null)
        {
            return other;
        }

        return new Result<U>(Error);
    }

    public ILogger<T> ToLogger()
    {
        if (Value != null)
        {
            return NoMsg(Value);
        }

        return Msg<T>(Error ?? string.Empty);
    }
}

public static class Result
{
    public static IResult<T> Ok<T>(T value) => new Result<T>(value);

    public static IResult<T> Error<T>(string error) => new Result<T>(error);
}
