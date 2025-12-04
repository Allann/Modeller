using Modeller.Parser;

namespace Modeller.Parser.Tests;

/// <summary>
/// Tests that parse the actual sample definition files
/// </summary>
public class SampleFileTests
{
    private static string SamplesPath => Path.Combine(
        Directory.GetCurrentDirectory(), 
        "..", "..", "..", "..", "..", 
        "samples", "modeller");

    [Fact]
    public void ParsesDomainFile()
    {
        var path = Path.Combine(SamplesPath, "domain.def");
        if (!File.Exists(path))
        {
            // Skip if samples not available in test run
            return;
        }

        var input = File.ReadAllText(path);
        var result = DslParser.ParseDomain(input);

        Assert.True(result.Success, $"Failed to parse domain.def: {result.Error}");
        Assert.Equal("Modeller", result.Value!.Name);
    }

    [Theory]
    [InlineData("entities/domain.entity")]
    [InlineData("entities/service.entity")]
    [InlineData("entities/entity.entity")]
    [InlineData("entities/attribute.entity")]
    public void ParsesEntityFiles(string relativePath)
    {
        var path = Path.Combine(SamplesPath, relativePath);
        if (!File.Exists(path))
        {
            return;
        }

        var input = File.ReadAllText(path);
        var result = DslParser.ParseEntityFile(input);

        Assert.True(result.Success, $"Failed to parse {relativePath}: {result.Error}");
    }

    [Theory]
    [InlineData("enums/data-type.enum")]
    [InlineData("enums/relationship-type.enum")]
    [InlineData("enums/output-type.enum")]
    public void ParsesEnumFiles(string relativePath)
    {
        var path = Path.Combine(SamplesPath, relativePath);
        if (!File.Exists(path))
        {
            return;
        }

        var input = File.ReadAllText(path);
        var result = DslParser.ParseEnum(input);

        Assert.True(result.Success, $"Failed to parse {relativePath}: {result.Error}");
    }

    [Theory]
    [InlineData("services/definition.service")]
    [InlineData("services/generation.service")]
    public void ParsesServiceFiles(string relativePath)
    {
        var path = Path.Combine(SamplesPath, relativePath);
        if (!File.Exists(path))
        {
            return;
        }

        var input = File.ReadAllText(path);
        var result = DslParser.ParseService(input);

        Assert.True(result.Success, $"Failed to parse {relativePath}: {result.Error}");
    }

    [Theory]
    [InlineData("behaviours/parse-domain.command")]
    [InlineData("behaviours/generate-output.command")]
    public void ParsesCommandFiles(string relativePath)
    {
        var path = Path.Combine(SamplesPath, relativePath);
        if (!File.Exists(path))
        {
            return;
        }

        var input = File.ReadAllText(path);
        var result = DslParser.ParseCommand(input);

        Assert.True(result.Success, $"Failed to parse {relativePath}: {result.Error}");
    }

    [Theory]
    [InlineData("behaviours/get-domain.query")]
    [InlineData("behaviours/list-generators.query")]
    public void ParsesQueryFiles(string relativePath)
    {
        var path = Path.Combine(SamplesPath, relativePath);
        if (!File.Exists(path))
        {
            return;
        }

        var input = File.ReadAllText(path);
        var result = DslParser.ParseQuery(input);

        Assert.True(result.Success, $"Failed to parse {relativePath}: {result.Error}");
    }
}

