using Modeller.Parser;

namespace Modeller.Parser.Tests;

public class ServiceParserTests
{
    [Fact]
    public void ParsesServiceWithEntities()
    {
        var input = """
            service Definition
              "Manages domain definitions"
              
              entities
                Domain
                Service
                Entity
                Attribute
              end
            end
            """;

        var result = DslParser.ParseService(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal("Definition", result.Value!.Name);
        Assert.Equal("Manages domain definitions", result.Value.Description);
        Assert.Equal(4, result.Value.Entities?.Count);
    }

    [Fact]
    public void ParsesServiceWithEnums()
    {
        var input = """
            service Definition
              entities
                Entity
              end
              
              enums
                DataType
                RelationshipType
              end
            end
            """;

        var result = DslParser.ParseService(input);

        Assert.True(result.Success, result.Error);
        Assert.Single(result.Value!.Entities!);
        Assert.Equal(2, result.Value.Enums?.Count);
    }

    [Fact]
    public void ParsesServiceWithReferences()
    {
        var input = """
            service Generation
              "Code generation service"
              
              entities
                Template
                Generator
              end
              
              references
                Definition: [Domain, Entity, Enum]
              end
            end
            """;

        var result = DslParser.ParseService(input);

        Assert.True(result.Success, result.Error);
        Assert.Single(result.Value!.References!);
        Assert.Equal("Definition", result.Value.References![0].ServiceName);
        Assert.Equal(3, result.Value.References[0].EntityNames.Count);
    }
}

