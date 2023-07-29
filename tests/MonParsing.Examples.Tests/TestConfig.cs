using Argon;
using System.Runtime.CompilerServices;

namespace MonParsing.Examples.Tests;

public static class TestConfig
{
    [ModuleInitializer]
    public static void Initialize()
    {
        VerifierSettings.UseStrictJson();
        VerifierSettings.AddExtraSettings(
            settings => settings.DefaultValueHandling = DefaultValueHandling.Include
        );
        UseProjectRelativeDirectory("Expectations");
    }
}
