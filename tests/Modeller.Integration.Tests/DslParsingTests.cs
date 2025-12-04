using Modeller.Parser;

namespace Modeller.Integration.Tests;

/// <summary>
/// Phase 1: Validates that all DSL files in samples/modeller/ parse correctly
/// </summary>
public class DslParsingTests
{
    private static readonly string SamplesPath = FindSamplesPath();
    
    private static string FindSamplesPath()
    {
        var dir = Directory.GetCurrentDirectory();
        while (dir != null)
        {
            var samples = Path.Combine(dir, "samples", "modeller");
            if (Directory.Exists(samples)) return samples;
            dir = Directory.GetParent(dir)?.FullName;
        }
        throw new DirectoryNotFoundException("Could not find samples/modeller directory");
    }

    [Fact]
    public void DomainFileExists()
    {
        var path = Path.Combine(SamplesPath, "domain.def");
        Assert.True(File.Exists(path), $"domain.def not found at {path}");
    }

    [Fact]
    public void ParsesDomainDefinition()
    {
        var path = Path.Combine(SamplesPath, "domain.def");
        var content = File.ReadAllText(path);
        var result = DslParser.ParseDomain(content);
        
        Assert.True(result.Success, $"Failed to parse domain.def: {result.Error}");
        Assert.Equal("Modeller", result.Value!.Name);
    }

    [Theory]
    [MemberData(nameof(GetEntityFiles))]
    public void ParsesAllEntityFiles(string relativePath)
    {
        var path = Path.Combine(SamplesPath, relativePath);
        var content = File.ReadAllText(path);
        var result = DslParser.ParseEntityFile(content);
        
        Assert.True(result.Success, $"Failed to parse {relativePath}: {result.Error}");
        Assert.NotNull(result.Value);
        Assert.False(string.IsNullOrEmpty(result.Value.Name), $"Entity name should not be empty in {relativePath}");
    }

    [Theory]
    [MemberData(nameof(GetEnumFiles))]
    public void ParsesAllEnumFiles(string relativePath)
    {
        var path = Path.Combine(SamplesPath, relativePath);
        var content = File.ReadAllText(path);
        var result = DslParser.ParseEnum(content);
        
        Assert.True(result.Success, $"Failed to parse {relativePath}: {result.Error}");
        Assert.NotNull(result.Value);
        Assert.False(string.IsNullOrEmpty(result.Value.Name), $"Enum name should not be empty in {relativePath}");
    }

    [Theory]
    [MemberData(nameof(GetServiceFiles))]
    public void ParsesAllServiceFiles(string relativePath)
    {
        var path = Path.Combine(SamplesPath, relativePath);
        var content = File.ReadAllText(path);
        var result = DslParser.ParseService(content);
        
        Assert.True(result.Success, $"Failed to parse {relativePath}: {result.Error}");
        Assert.NotNull(result.Value);
    }

    [Theory]
    [MemberData(nameof(GetCommandFiles))]
    public void ParsesAllCommandFiles(string relativePath)
    {
        var path = Path.Combine(SamplesPath, relativePath);
        var content = File.ReadAllText(path);
        var result = DslParser.ParseCommand(content);
        
        Assert.True(result.Success, $"Failed to parse {relativePath}: {result.Error}");
        Assert.NotNull(result.Value);
    }

    [Theory]
    [MemberData(nameof(GetQueryFiles))]
    public void ParsesAllQueryFiles(string relativePath)
    {
        var path = Path.Combine(SamplesPath, relativePath);
        var content = File.ReadAllText(path);
        var result = DslParser.ParseQuery(content);
        
        Assert.True(result.Success, $"Failed to parse {relativePath}: {result.Error}");
        Assert.NotNull(result.Value);
    }

    public static IEnumerable<object[]> GetEntityFiles() =>
        GetFilesWithExtension("entities", "*.entity");

    public static IEnumerable<object[]> GetEnumFiles() =>
        GetFilesWithExtension("enums", "*.enum");

    public static IEnumerable<object[]> GetServiceFiles() =>
        GetFilesWithExtension("services", "*.service");

    public static IEnumerable<object[]> GetCommandFiles() =>
        GetFilesWithExtension("behaviours", "*.command");

    public static IEnumerable<object[]> GetQueryFiles() =>
        GetFilesWithExtension("behaviours", "*.query");

    private static IEnumerable<object[]> GetFilesWithExtension(string folder, string pattern)
    {
        var dir = Path.Combine(SamplesPath, folder);
        if (!Directory.Exists(dir)) yield break;
        
        foreach (var file in Directory.GetFiles(dir, pattern))
        {
            yield return [Path.Combine(folder, Path.GetFileName(file))];
        }
    }
}

