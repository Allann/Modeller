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
                DomainForGeneration
                EntityForGeneration
                EnumForGeneration
              end
            end
            """;

        var result = DslParser.ParseService(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal(3, result.Value!.References!.Count);
        Assert.Contains("DomainForGeneration", result.Value.References);
        Assert.Contains("EntityForGeneration", result.Value.References);
    }

    [Fact]
    public void ParsesServiceWithCallsAndImplements()
    {
        var input = """
            service Scheduling
              "Manages bookings"

              entities
                Booking
              end

              calls
                FileStorageRPC
                CreateUserNotification
              end

              implements
                GovSubmitSessionReport
              end
            end
            """;

        var result = DslParser.ParseService(input);

        Assert.True(result.Success, result.Error);
        Assert.Equal(2, result.Value!.Calls!.Count);
        Assert.Contains("FileStorageRPC", result.Value.Calls);
        Assert.Single(result.Value.Implements!);
        Assert.Equal("GovSubmitSessionReport", result.Value.Implements![0]);
    }
}

