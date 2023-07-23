using FluentAssertions;
using MonParsing.Examples.Json;

namespace MonParsing.Examples.Tests;

public class JsonTests
{
    [Fact]
    public void Parser_parses_empty_object() =>
        Parser
            .Parse("{}")
            .Should()
            .BeEquivalentTo(
                new
                {
                    Value = new
                    {
                        Result = new JObject { Value = new Dictionary<string, JValue>() },
                        Input = ""
                    }
                }
            );
}
