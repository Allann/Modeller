using Modeller.Parser;

namespace Modeller.Parser.Tests;

public class ProjectionParserTests
{
    [Fact]
    public void ParsesSimpleProjection()
    {
        var input = """
            projection EntitySummary
              "A summary view of entities"
              
              EntityId: guid "The entity identifier"
              Name: name "Entity name"
              AttributeCount: integer "Number of attributes"
            end
            """;

        var result = DslParser.ParseProjection(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal("EntitySummary", result.Value!.Name);
        Assert.Equal("A summary view of entities", result.Value.Description);
        Assert.Equal(3, result.Value.Attributes?.Count);
        
        Assert.Equal("EntityId", result.Value.Attributes![0].Name);
        Assert.Equal("guid", result.Value.Attributes![0].DataType);
    }

    [Fact]
    public void ParsesProjectionWithManyFields()
    {
        var input = """
            projection DomainOverview
              DomainId: guid
              Name: name
              Description: text(500), optional
              ServiceCount: integer
              EntityCount: integer
              Version: text(20), optional
            end
            """;

        var result = DslParser.ParseProjection(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal("DomainOverview", result.Value!.Name);
        Assert.Equal(6, result.Value.Attributes?.Count);
    }

    [Fact]
    public void ParsesProjectionWithDefaultValues()
    {
        var input = """
            projection StatusDisplay
              Id: guid
              Status: text(20), default("Active")
              IsVisible: boolean, default(true)
            end
            """;

        var result = DslParser.ParseProjection(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal("Active", result.Value!.Attributes![1].DefaultValue);
        Assert.Equal("true", result.Value.Attributes![2].DefaultValue);
    }
}

