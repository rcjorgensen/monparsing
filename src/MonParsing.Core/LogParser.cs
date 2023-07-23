namespace MonParsing.Core;

public delegate ILogger<IParseResult<T>> LogParser<out T>(string input);

public static class LogParser
{
    public static LogParser<U> Map<T, U>(this LogParser<T> logParser, Func<T, U> selector) =>
        (string input) => logParser(input).Map(pr => pr.Map(selector));

    public static LogParser<U> AndThen<T, U>(
        this LogParser<T> logParser,
        Func<T, LogParser<U>> selector
    ) =>
        (string input) =>
            from pr1 in logParser(input)
            from pr2 in selector(pr1.Result)(pr1.Input)
            select pr2;

    public static LogParser<T> NoMsg<T>(this Parser<T> parser) =>
        (string input) => parser(input).ToLogger();

    public static LogParser<T> Msg<T>(this Parser<T> parser, string log) =>
        (string input) => Logger.Msg<IParseResult<T>>(log).Then(parser(input).ToLogger());

    public static ILogger<IParseResult<T>> Zero<T>(string input) =>
        Parser.Zero<T>(input).ToLogger();

    public static LogParser<T> Result<T>(T value) =>
        Parser
            .Result(value)
            .Msg(
                $"Method: {nameof(Result)}, Arguments: {nameof(T)}={typeof(T).Name}, {nameof(value)}={value}"
            );
}
