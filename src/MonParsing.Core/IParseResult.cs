namespace MonParsing.Core;

public interface IParseResult<out T>
{
    T Result { get; }

    string Input { get; }

    IParseResult<U> Map<U>(Func<T, U> selector);

    IParseResult<U> AndThen<U>(Func<T, IParseResult<U>> selector);
}

internal record ParseResult<T> : IParseResult<T>
{
    public ParseResult(T result, string input)
    {
        Result = result;
        Input = input;
    }

    public T Result { get; }

    public string Input { get; }

    public IParseResult<U> AndThen<U>(Func<T, IParseResult<U>> selector) =>
        ParseResult.Of(selector(Result).Result, Input);

    public IParseResult<U> Map<U>(Func<T, U> selector) => ParseResult.Of(selector(Result), Input);
}

public static class ParseResult
{
    public static IParseResult<T> Of<T>(T result, string input) =>
        new ParseResult<T>(result, input);
}
