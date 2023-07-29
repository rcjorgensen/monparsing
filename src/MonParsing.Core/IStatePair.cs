namespace MonParsing.Core;

public interface IStatePair<out T>
{
    T Result { get; }
    string Input { get; }

    IStatePair<U> Map<U>(Func<T, U> selector);

    IStatePair<U> Select<U>(Func<T, U> selector) => Map(selector);
}

internal record StatePair<T> : IStatePair<T>
{
    public T Result { get; }
    public string Input { get; }

    public StatePair(T result, string input)
    {
        Result = result;
        Input = input;
    }

    public IStatePair<U> Map<U>(Func<T, U> selector) => new StatePair<U>(selector(Result), Input);
}

public static class StatePair
{
    public static IStatePair<T> Of<T>(T result, string input) => new StatePair<T>(result, input);
}
