using Modeller.Parser;
using Modeller.Parser.Ast;

namespace Modeller.Parser.Tests;

public class EntityParserTests
{
    [Fact]
    public void ParsesSimpleEntity()
    {
        var input = """
            entity Domain
              "The root container for a domain model"
              
              Name: name "The domain name"
              Description: text(500) "Description of the domain"
              
              has_many Service
            end
            """;

        var result = DslParser.ParseEntity(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal("Domain", result.Value!.Name);
        Assert.Equal("The root container for a domain model", result.Value.Description);
        Assert.Equal(2, result.Value.Attributes?.Count);
        Assert.Single(result.Value.Relationships!);
    }

    [Fact]
    public void ParsesEntityWithAllAttributeTypes()
    {
        var input = """
            entity Attribute
              "A property on an entity"
              
              Name: name "The attribute name"
              MaxLength: integer, optional "Max length"
              IsOptional: boolean, default(false) "Can be null"
              
              belongs_to Entity
            end
            """;

        var result = DslParser.ParseEntity(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal("Attribute", result.Value!.Name);
        Assert.Equal(3, result.Value.Attributes?.Count);

        var nameAttr = result.Value.Attributes![0];
        Assert.Equal("Name", nameAttr.Name);
        Assert.Equal("name", nameAttr.DataType);

        var maxLengthAttr = result.Value.Attributes[1];
        Assert.True(maxLengthAttr.IsOptional);

        var isOptionalAttr = result.Value.Attributes[2];
        Assert.Equal("false", isOptionalAttr.DefaultValue);
    }

    [Fact]
    public void ParsesEntityWithMultipleRelationships()
    {
        var input = """
            entity Service
              "A bounded context"
              
              Name: name
              
              belongs_to Domain
              has_many Entity
              has_many Command
              has_many Query
            end
            """;

        var result = DslParser.ParseEntity(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal(4, result.Value!.Relationships?.Count);
        Assert.Equal(RelationshipType.BelongsTo, result.Value.Relationships![0].Type);
        Assert.Equal(RelationshipType.HasMany, result.Value.Relationships[1].Type);
    }

    [Fact]
    public void ParsesEntityWithRelationshipAlias()
    {
        var input = """
            entity Command
              "A write operation"
              
              Name: name
              
              has_many Attribute as Parameters
            end
            """;

        var result = DslParser.ParseEntity(input);

        Assert.True(result.Success, result.Error);
        Assert.Single(result.Value!.Relationships!);
        Assert.Equal("Attribute", result.Value.Relationships![0].TargetEntity);
        Assert.Equal("Parameters", result.Value.Relationships[0].Alias);
    }

    [Fact]
    public void ParsesEntityWithComments()
    {
        var input = """
            # This is a comment
            entity Domain
              "Root container"
              
              # Another comment
              Name: name
            end
            """;

        var result = DslParser.ParseEntity(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal("Domain", result.Value!.Name);
    }
}

