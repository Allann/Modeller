using Modeller.Parser;

namespace Modeller.Parser.Tests;

public class EventParserTests
{
    [Fact]
    public void ParsesSimpleEvent()
    {
        var input = """
            event EntityCreated
              "Raised when a new entity is defined"
              
              EntityId: guid "The entity identifier"
              Name: name "The entity name"
              Timestamp: datetime "When created"
            end
            """;

        var result = DslParser.ParseEvent(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal("EntityCreated", result.Value!.Name);
        Assert.Equal("Raised when a new entity is defined", result.Value.Description);
        Assert.Equal(3, result.Value.Attributes?.Count);
        
        Assert.Equal("EntityId", result.Value.Attributes![0].Name);
        Assert.Equal("guid", result.Value.Attributes![0].DataType);
    }

    [Fact]
    public void ParsesEventWithMultipleFields()
    {
        var input = """
            event GenerationCompleted
              GeneratorId: guid
              DomainId: guid
              OutputPath: text(500)
              FileCount: integer
              CompletedAt: datetime
            end
            """;

        var result = DslParser.ParseEvent(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal("GenerationCompleted", result.Value!.Name);
        Assert.Equal(5, result.Value.Attributes?.Count);
    }

    [Fact]
    public void ParsesEventWithOptionalFields()
    {
        var input = """
            event UserUpdated
              UserId: guid
              OldEmail: email, optional
              NewEmail: email
            end
            """;

        var result = DslParser.ParseEvent(input);

        Assert.True(result.Success, result.Error);
        Assert.False(result.Value!.Attributes![0].IsOptional);
        Assert.True(result.Value.Attributes![1].IsOptional);
        Assert.False(result.Value.Attributes![2].IsOptional);
    }
}

