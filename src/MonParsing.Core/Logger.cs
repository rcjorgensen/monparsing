namespace MonParsing.Core;

using static Unit;

public interface ILogger<out T>
{
    IEnumerable<string> Logs { get; }

    T Value { get; }

    ILogger<U> Map<U>(Func<T, U> selector);

    ILogger<U> AndThen<U>(Func<T, ILogger<U>> selector);

    ILogger<U> And<U>(ILogger<U> other);
}

internal class Logger<T> : ILogger<T>
{
    public IEnumerable<string> Logs { get; }

    public T Value { get; }

    public Logger(IEnumerable<string> logs, T value)
    {
        Logs = logs;
        Value = value;
    }

    public Logger(string log, T value)
        : this(new string[] { log }, value) { }

    public Logger(T value)
        : this(Enumerable.Empty<string>(), value) { }

    public ILogger<U> Map<U>(Func<T, U> selector) => new Logger<U>(Logs, selector(Value));

    public ILogger<U> AndThen<U>(Func<T, ILogger<U>> selector)
    {
        var next = selector(Value);
        return new Logger<U>(Logs.Concat(next.Logs), next.Value);
    }

    public ILogger<U> And<U>(ILogger<U> other) => new Logger<U>(Logs.Concat(Logs), other.Value);
}

public static class Logger
{
    public static ILogger<T> NoMsg<T>(T value) => new Logger<T>(value);

    public static ILogger<T> Annotate<T>(string log, T value) => new Logger<T>(log, value);

    public static ILogger<Unit> Msg(string log) => new Logger<Unit>(log, unit);
}
