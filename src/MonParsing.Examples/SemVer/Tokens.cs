namespace MonParsing.Examples.SemVer;

public record SemVer
{
    public required VersionCore VersionCore { get; init; }

    public PreRelease? PreRelease { get; init; }

    public Build? Build { get; init; }
}

public record struct VersionCore
{
    public required int Major { get; init; }
    public required int Minor { get; init; }
    public required int Patch { get; init; }
}

public record PreRelease
{
    public required IEnumerable<string> Identifiers { get; init; }
}

public record Build
{
    public required IEnumerable<string> Identifiers { get; init; }
}
