using FluentAssertions;
using MonParsing.Core;
using static MonParsing.Core.Result;

namespace MonParsing.Examples.Tests;

public class JsonTests
{
    [Fact]
    public void Parser_parses_empty_object() =>
        Json.Parse("{}")
            .Should()
            .BeEquivalentTo(
                Ok(ParseResult.Of(new JObject { Value = new Dictionary<string, JValue>() }, ""))
            );
}
