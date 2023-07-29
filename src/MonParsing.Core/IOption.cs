namespace MonParsing.Core;

public interface IOption<out T>
{
    T? Value { get; }

    IOption<U> AndThen<U>(Func<T, IOption<U>> selector);

    IOption<U> Map<U>(Func<T, U> selector);
}

internal class Option<T> : IOption<T>
{
    public Option(T value)
    {
        Value = value;
    }

    public Option() { }

    public T? Value { get; }

    public IOption<U> AndThen<U>(Func<T, IOption<U>> selector)
    {
        return Value != null ? selector(Value) : Option.None<U>();
    }

    public IOption<U> Map<U>(Func<T, U> selector)
    {
        if (Value != null)
        {
            return Option.Some(selector(Value));
        }

        return Option.None<U>();
    }
}

public static class Option
{
    public static IOption<T> Some<T>(T value) => new Option<T>(value);

    public static IOption<T> None<T>() => new Option<T>();
}
