using MonParsing.Core;
using static MonParsing.Core.Parser;

namespace MonParsing.Examples;

public static class Json
{
    private static Parser<Unit> Whitespace =
        from w in ZeroOrMore(Sat(c => c == ' ' || c == '\n' || c == '\r' || c == '\t'))
        select Unit.unit;

    private static Parser<T> Token<T>(Parser<T> parser) =>
        from p in parser
        from w in Whitespace
        select p;

    // Simplified JSON string where escapes are not allowed
    private static Parser<char> DoubleQuote = Char('"');
    private static Parser<string> StringContent =
        from cs in ZeroOrMore(Sat(c => c != '"' && c != '\\'))
        select string.Concat(cs);
    private static Parser<JString> JString = Token(
        from q1 in DoubleQuote
        from s in StringContent
        from q2 in DoubleQuote
        select new JString { Value = s }
    );

    // Simplified JSON number where only integers are allowed
    private static Parser<JNumber> JNumber = Token(
        from n in Integer
        select new JNumber { Value = n }
    );

    private static Parser<JBool> JTrue = Token(
        from b in String("true")
        select new JBool { Value = true }
    );

    private static Parser<JBool> JFalse = Token(
        from b in String("false")
        select new JBool { Value = false }
    );

    private static Parser<JNull> JNull = Token(from x in String("null") select new JNull());

    private static Parser<LeftBrace> LeftBrace = Token(from l in Char('{') select new LeftBrace());

    private static Parser<RightBrace> RightBrace = Token(
        from r in Char('}')
        select new RightBrace()
    );

    private static Parser<LeftBracket> LeftBracket = Token(
        from l in Char('[')
        select new LeftBracket()
    );

    private static Parser<RightBracket> RightBracket = Token(
        from r in Char(']')
        select new RightBracket()
    );

    private static Parser<Comma> Comma = Token(from c in Char(',') select new Comma());

    private static Parser<Colon> Colon = Token(from c in Char(':') select new Colon());

    private static Parser<JValue> JValue =>
        Token(Or<JValue>(JString, JNumber, JObject, JArray, JTrue, JFalse, JNull));

    private static Parser<KeyValuePair<string, JValue>> NameValue =>
        Token(
            from s in JString
            from c in Colon
            from v in JValue
            select KeyValuePair.Create(s.Value, v)
        );

    private static Parser<JObject> JObject =>
        Token(
            from l in LeftBrace
            from nvs in ZeroOrMoreSeparated(NameValue, Comma)
            from r in RightBrace
            select new JObject { Value = new Dictionary<string, JValue>(nvs) }
        );

    private static Parser<JArray> JArray =>
        Token(
            from l in LeftBracket
            from vs in ZeroOrMoreSeparated(JValue, Comma)
            from r in RightBracket
            select new JArray { Value = vs }
        );

    public static Parser<JObject> Parse = from w in Whitespace from json in JObject select json;
}
