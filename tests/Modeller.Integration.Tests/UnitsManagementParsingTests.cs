using Modeller.Parser;

namespace Modeller.Integration.Tests;

/// <summary>
/// Validates that all DSL files in samples/modeller-units parse without errors.
/// </summary>
public class UnitsManagementParsingTests
{
    private static readonly string UnitsPath = FindUnitsPath();

    private static string FindUnitsPath()
    {
        var dir = Directory.GetCurrentDirectory();
        while (dir != null)
        {
            var candidate = Path.Combine(dir, "samples", "modeller-units");
            if (Directory.Exists(candidate)) return candidate;
            dir = Directory.GetParent(dir)?.FullName;
        }
        throw new DirectoryNotFoundException("Could not find samples/modeller-units directory");
    }

    [Fact]
    public void DomainDefFileExists()
    {
        Assert.True(File.Exists(Path.Combine(UnitsPath, "domain.def")));
    }

    [Fact]
    public void ParsesDomainDefinition()
    {
        var content = File.ReadAllText(Path.Combine(UnitsPath, "domain.def"));
        var result = DslParser.ParseDomain(content);
        Assert.True(result.Success, $"Failed to parse domain.def: {result.Error}");
        Assert.Equal("UnitsManagement", result.Value!.Name);
    }

    [Theory]
    [MemberData(nameof(GetEntityFiles))]
    public void ParsesEntityFile(string relativePath)
    {
        var content = File.ReadAllText(Path.Combine(UnitsPath, relativePath));
        var result = DslParser.ParseEntityFile(content);
        Assert.True(result.Success, $"Failed to parse {relativePath}: {result.Error}");
        Assert.False(string.IsNullOrWhiteSpace(result.Value?.Name));
    }

    [Theory]
    [MemberData(nameof(GetEnumFiles))]
    public void ParsesEnumFile(string relativePath)
    {
        var content = File.ReadAllText(Path.Combine(UnitsPath, relativePath));
        var result = DslParser.ParseEnum(content);
        Assert.True(result.Success, $"Failed to parse {relativePath}: {result.Error}");
        Assert.False(string.IsNullOrWhiteSpace(result.Value?.Name));
    }

    [Theory]
    [MemberData(nameof(GetCommandFiles))]
    public void ParsesCommandFile(string relativePath)
    {
        var content = File.ReadAllText(Path.Combine(UnitsPath, relativePath));
        var result = DslParser.ParseCommand(content);
        Assert.True(result.Success, $"Failed to parse {relativePath}: {result.Error}");
        Assert.NotNull(result.Value);
    }

    [Theory]
    [MemberData(nameof(GetQueryFiles))]
    public void ParsesQueryFile(string relativePath)
    {
        var content = File.ReadAllText(Path.Combine(UnitsPath, relativePath));
        var result = DslParser.ParseQuery(content);
        Assert.True(result.Success, $"Failed to parse {relativePath}: {result.Error}");
        Assert.NotNull(result.Value);
    }

    public static IEnumerable<object[]> GetEntityFiles() =>
        FilesWithExtension("entities", "*.entity");

    public static IEnumerable<object[]> GetEnumFiles() =>
        FilesWithExtension("enums", "*.enum");

    public static IEnumerable<object[]> GetCommandFiles() =>
        FilesWithExtension("behaviours", "*.command");

    public static IEnumerable<object[]> GetQueryFiles() =>
        FilesWithExtension("behaviours", "*.query");

    private static IEnumerable<object[]> FilesWithExtension(string folder, string pattern)
    {
        var dir = Path.Combine(UnitsPath, folder);
        if (!Directory.Exists(dir)) yield break;
        foreach (var file in Directory.GetFiles(dir, pattern))
            yield return [Path.Combine(folder, Path.GetFileName(file))];
    }
}
