namespace MonParsing.Core;

using static Unit;

public interface ILogger<out T>
{
    T? Value { get; }

    IEnumerable<string> Logs { get; }

    ILogger<U> Map<U>(Func<T, U> selector);

    ILogger<U> Then<U>(ILogger<U> other);

    ILogger<U> And<U>(ILogger<U> other);

    ILogger<U> AndThen<U>(Func<T, ILogger<U>> selector);
}

internal class Logger<T> : ILogger<T>
{
    public IEnumerable<string> Logs { get; }

    public T? Value { get; }

    public Logger(IEnumerable<string> logs, T value)
    {
        Logs = logs;
        Value = value;
    }

    public Logger(T value)
        : this(Enumerable.Empty<string>(), value) { }

    public Logger(IEnumerable<string> logs)
    {
        Logs = logs;
    }

    public ILogger<U> Map<U>(Func<T, U> selector)
    {
        if (Value != null)
        {
            return new Logger<U>(Logs, selector(Value));
        }

        return new Logger<U>(Logs);
    }

    public ILogger<U> Then<U>(ILogger<U> other)
    {
        if (other.Value == null)
        {
            return new Logger<U>(Logs.Concat(other.Logs));
        }

        return new Logger<U>(Logs.Concat(other.Logs), other.Value);
    }

    public ILogger<U> And<U>(ILogger<U> other)
    {
        if (Value == null)
        {
            return new Logger<U>(Logs);
        }

        return Then(other);
    }

    public ILogger<U> AndThen<U>(Func<T, ILogger<U>> selector)
    {
        if (Value == null)
        {
            return new Logger<U>(Logs);
        }

        return Then(selector(Value));
    }
}

public static class Logger
{
    public static ILogger<T> NoMsg<T>(T value) => new Logger<T>(value);

    public static ILogger<T> Annotate<T>(string log, T value) =>
        new Logger<T>(new string[] { log }, value);

    public static ILogger<T> Msg<T>(string log) => new Logger<T>(new string[] { log });
}
