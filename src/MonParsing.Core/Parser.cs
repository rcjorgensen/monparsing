namespace MonParsing.Core;

public delegate IResult<IParseResult<T>> Parser<out T>(string input);
