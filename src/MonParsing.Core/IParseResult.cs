namespace MonParsing.Core;

public interface IParseResult<out T>
{
    T Result { get; }

    string Input { get; }
}

public record ParseResult<T> : IParseResult<T>
{
    public ParseResult(T result, string input)
    {
        Result = result;
        Input = input;
    }

    public T Result { get; }

    public string Input { get; }
}

public static class ParseResult
{
    public static IParseResult<T> Default<T>(T result, string input) =>
        new ParseResult<T>(result, input);

    public static IParseResult<U> Map<T, U>(this IParseResult<T> parseResult, Func<T, U> func) =>
        Default(func(parseResult.Result), parseResult.Input);
}
