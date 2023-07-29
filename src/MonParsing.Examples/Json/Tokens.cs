namespace MonParsing.Examples.Json;

public abstract record JToken { }

public record LeftBrace : JToken
{
    public override string ToString() => "{";
}

public record RightBrace : JToken
{
    public override string ToString() => "}";
}

public record LeftBracket : JToken
{
    public override string ToString() => "[";
}

public record RightBracket : JToken
{
    public override string ToString() => "]";
}

public record Comma : JToken
{
    public override string ToString() => ",";
}

public record Colon : JToken
{
    public override string ToString() => ":";
}

public abstract record JValue : JToken { }

public abstract record JValue<T> : JValue
{
    public virtual required T Value { get; init; }

    public override string ToString()
    {
        return $"JValue<{typeof(T)}>({Value})";
    }
}

public record JString : JValue<string> { }

public record JNumber : JValue<int> { }

public record JBool : JValue<bool> { }

public record JNull : JValue { }

public record JArray : JValue<IEnumerable<JValue>> { }

public record JObject : JValue<Dictionary<string, JValue>> { }
