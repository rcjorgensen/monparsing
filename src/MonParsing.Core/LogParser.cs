namespace MonParsing.Core;

public delegate ILogger<IResult<IParseResult<T>>> LogParser<out T>(string input);
