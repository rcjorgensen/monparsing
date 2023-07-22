using static MonParsing.Core.Logger;

namespace MonParsing.Core;

public static class LogParsers
{
    public static ILogger<IResult<IParseResult<T>>> Zero<T>(string input) =>
        Annotate(
            $"Method: {nameof(Zero)}, Arguments: T={typeof(T).Name}, input={input}",
            Parsers.Zero<T>(input)
        );
}
