namespace MonParsing.Examples.Tests;

[UsesVerify]
public class SemVerTests
{
    [Fact]
    public Task SemVer_parser_parses_valid_input() =>
        Verify(SemVer.Parser("1.0.0-alpha.beta+exp.sha.5114f85"));

    [Fact]
    public Task SemVer_parser_does_not_parse_invalid_input() => Verify(SemVer.Parser("01.0.0"));
}
