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

    [Fact]
    public void ParsesEntityWithReferencesRelationship()
    {
        var input = """
            entity Booking
              "A booking"

              Date: date "The booking date"

              belongs_to Child
              references CentreForScheduling
              references SessionForScheduling
            end
            """;

        var result = DslParser.ParseEntity(input);

        Assert.True(result.Success, result.Error);
        var rels = result.Value!.Relationships!;
        Assert.Equal(3, rels.Count);
        Assert.Equal(RelationshipType.BelongsTo, rels[0].Type);
        Assert.Equal(RelationshipType.References, rels[1].Type);
        Assert.Equal("CentreForScheduling", rels[1].TargetEntity);
        Assert.Equal(RelationshipType.References, rels[2].Type);
    }

    [Fact]
    public void ParsesEntityRelationshipsWithDescriptionsAliasAndOptionalTarget()
    {
        var input = """
            entity Booking
              "A booking"

              has_one Child? "Optional child relationship"
              has_many Children: Child "Aliased collection relationship"
              references CentreForScheduling "Cross-service relationship"
              has_many Attendance as Attendances "Legacy alias form"
            end
            """;

        var result = DslParser.ParseEntity(input);

        Assert.True(result.Success, result.Error);
        var rels = result.Value!.Relationships!;
        Assert.Equal(4, rels.Count);

        Assert.Equal("Child", rels[0].TargetEntity);
        Assert.Equal("Optional child relationship", rels[0].Description);

        Assert.Equal("Child", rels[1].TargetEntity);
        Assert.Equal("Children", rels[1].Alias);
        Assert.Equal("Aliased collection relationship", rels[1].Description);

        Assert.Equal(RelationshipType.References, rels[2].Type);
        Assert.Equal("Cross-service relationship", rels[2].Description);

        Assert.Equal("Attendance", rels[3].TargetEntity);
        Assert.Equal("Attendances", rels[3].Alias);
        Assert.Equal("Legacy alias form", rels[3].Description);
    }

    [Fact]
    public void ParsesEntityWithStandardLibraryTypes()
    {
        var input = """
            entity RoomSessionFee
              "Fee schedule for a room session"

              Amount: money "The scheduled fee"
              CCSRate: percentage "The CCS subsidy rate"
              Photo: image, optional "Room photo"
              Contract: document, optional "Fee contract"
            end
            """;

        var result = DslParser.ParseEntity(input);

        Assert.True(result.Success, result.Error);
        var attrs = result.Value!.Attributes!;
        Assert.Equal(4, attrs.Count);
        Assert.Equal("money", attrs[0].DataType);
        Assert.Equal("percentage", attrs[1].DataType);
        Assert.Equal("image", attrs[2].DataType);
        Assert.True(attrs[2].IsOptional);
        Assert.Equal("document", attrs[3].DataType);
    }

    [Fact]
    public void ParsesEntityWithGeospatialType()
    {
        var input = """
            entity Centre
              "A childcare centre"

              Name: name "Centre name"
              Location: geospatial, optional "Geographic coordinates"
            end
            """;

        var result = DslParser.ParseEntity(input);

        Assert.True(result.Success, result.Error);
        var locationAttr = result.Value!.Attributes![1];
        Assert.Equal("geospatial", locationAttr.DataType);
        Assert.True(locationAttr.IsOptional);
    }
}

