using FluentAssertions;
using MonParsing.Examples.SemVer;

namespace MonParsing.Examples.Tests;

public class SemVerTests
{
    [Fact]
    public void SemVer_parser_parses_valid_SemVer() =>
        Parser
            .Parse("1.0.0-alpha.beta+exp.sha.5114f85")
            .Should()
            .BeEquivalentTo(
                new
                {
                    Value = new
                    {
                        Result = new SemVer.SemVer
                        {
                            VersionCore = new VersionCore
                            {
                                Major = 1,
                                Minor = 0,
                                Patch = 0
                            },
                            PreRelease = new PreRelease
                            {
                                Identifiers = new string[] { "alpha", "beta" }
                            },
                            Build = new Build
                            {
                                Identifiers = new string[] { "exp", "sha", "5114f85" }
                            }
                        },
                        Input = ""
                    }
                }
            );
}
