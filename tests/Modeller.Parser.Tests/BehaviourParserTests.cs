using Modeller.Parser;

namespace Modeller.Parser.Tests;

public class BehaviourParserTests
{
    [Fact]
    public void ParsesCommand()
    {
        var input = """
            command ParseDomain
              "Parses DSL files into domain model"
              
              input
                SourcePath: text(500) "Path to files"
                Recursive: boolean, default(true)
              end
              
              output Domain
              
              errors
                FileNotFound "Source path does not exist"
                ParseError "Syntax error in file"
              end
              
              publishes
                DomainParsed
              end
            end
            """;

        var result = DslParser.ParseCommand(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal("ParseDomain", result.Value!.Name);
        Assert.Equal("Parses DSL files into domain model", result.Value.Description);
        Assert.Equal(2, result.Value.Inputs?.Count);
        Assert.Equal("Domain", result.Value.Output);
        Assert.Equal(2, result.Value.Errors?.Count);
        Assert.Single(result.Value.Events!);
    }

    [Fact]
    public void ParsesMinimalCommand()
    {
        var input = """
            command DoSomething
            end
            """;

        var result = DslParser.ParseCommand(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal("DoSomething", result.Value!.Name);
        Assert.Null(result.Value.Inputs);
        Assert.Null(result.Value.Output);
    }

    [Fact]
    public void ParsesQuery()
    {
        var input = """
            query GetDomain
              "Retrieves a domain by name"
              
              input
                Name: name "The domain name"
              end
              
              returns Domain
            end
            """;

        var result = DslParser.ParseQuery(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal("GetDomain", result.Value!.Name);
        Assert.Single(result.Value.Inputs!);
        Assert.Equal("Domain", result.Value.Returns);
        Assert.False(result.Value.ReturnsMany);
    }

    [Fact]
    public void ParsesQueryReturningMany()
    {
        var input = """
            query ListGenerators
              "Lists all generators"
              
              returns many Generator
            end
            """;

        var result = DslParser.ParseQuery(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal("Generator", result.Value!.Returns);
        Assert.True(result.Value.ReturnsMany);
    }
}

