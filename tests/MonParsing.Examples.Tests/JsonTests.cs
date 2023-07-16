using FluentAssertions;
using MonParsing.Core;

namespace MonParsing.Examples.Tests;

public class JsonTests
{
    [Fact]
    public void Parser_parses_empty_object() =>
        Json.Parse("{}")
            .First()
            .Should()
            .BeEquivalentTo(
                ParseResult.Default(new JObject { Value = new Dictionary<string, JValue>() }, "")
            );
}
