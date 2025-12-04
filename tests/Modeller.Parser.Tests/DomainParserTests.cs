using Modeller.Parser;

namespace Modeller.Parser.Tests;

public class DomainParserTests
{
    [Fact]
    public void ParsesSimpleDomain()
    {
        var input = """
            domain Modeller
              "Code generation tool"
              
              company "Catalyst"
              version "2.0.0"
              
              services
                Definition
                Generation
              end
            end
            """;

        var result = DslParser.ParseDomain(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal("Modeller", result.Value!.Name);
        Assert.Equal("Code generation tool", result.Value.Description);
        Assert.Equal("Catalyst", result.Value.Company);
        Assert.Equal("2.0.0", result.Value.Version);
        Assert.Equal(2, result.Value.Services?.Count);
        Assert.Contains("Definition", result.Value.Services!);
        Assert.Contains("Generation", result.Value.Services!);
    }

    [Fact]
    public void ParsesMinimalDomain()
    {
        var input = """
            domain Simple
            end
            """;

        var result = DslParser.ParseDomain(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal("Simple", result.Value!.Name);
        Assert.Null(result.Value.Description);
        Assert.Null(result.Value.Company);
        Assert.Null(result.Value.Services);
    }
}

