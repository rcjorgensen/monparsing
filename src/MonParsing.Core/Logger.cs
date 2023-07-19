namespace MonParsing.Core;

using static Unit;

public class Logger<T>
{
    public IEnumerable<string> Logs { get; }

    public T Value { get; }

    public Logger(IEnumerable<string> logs, T value)
    {
        Logs = logs;
        Value = value;
    }
}

public static class Logger
{
    public static Logger<T> NoMsg<T>(T value) => new Logger<T>(Enumerable.Empty<string>(), value);

    public static Logger<T> Annotate<T>(string log, T value) =>
        new Logger<T>(new string[] { log }, value);

    public static Logger<Unit> Msg(string log) => new Logger<Unit>(new string[] { log }, unit);

    public static Logger<U> AndThen<T, U>(this Logger<T> logger, Func<T, Logger<U>> func)
    {
        var next = func(logger.Value);
        return new Logger<U>(logger.Logs.Concat(next.Logs), next.Value);
    }

    public static Logger<U> And<T, U>(this Logger<T> logger1, Logger<U> logger2) =>
        new Logger<U>(logger1.Logs.Concat(logger2.Logs), logger2.Value);
}
