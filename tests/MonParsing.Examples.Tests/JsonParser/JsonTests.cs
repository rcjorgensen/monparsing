using FluentAssertions;
using MonParsing.Core;
using MonParsing.Examples.JsonParser;

namespace MonParsing.Examples.Tests.JsonParser;

public class JsonTests
{
    [Fact]
    public void Parser_parses_empty_object() =>
        Json.Parser("{}")
            .First()
            .Should()
            .BeEquivalentTo(
                ParseResult.Default(new JObject { Value = new Dictionary<string, JValue>() }, "")
            );
}
