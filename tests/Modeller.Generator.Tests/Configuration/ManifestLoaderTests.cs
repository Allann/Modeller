using Modeller.Generator.Configuration;

namespace Modeller.Generator.Tests.Configuration;

public class ManifestLoaderTests
{
    private readonly ManifestLoader _loader = new();

    [Fact]
    public void LoadPackManifest_ParsesBasicFields()
    {
        // Arrange
        var yaml = """
            name: clean-architecture
            version: 2.0.0
            description: Clean Architecture templates
            author: Test Author
            language: csharp
            """;

        // Act
        var manifest = _loader.LoadPackManifestFromString(yaml);

        // Assert
        Assert.Equal("clean-architecture", manifest.Name);
        Assert.Equal("2.0.0", manifest.Version);
        Assert.Equal("Clean Architecture templates", manifest.Description);
        Assert.Equal("Test Author", manifest.Author);
        Assert.Equal("csharp", manifest.Language);
    }

    [Fact]
    public void LoadPackManifest_ParsesTemplatesList()
    {
        // Arrange
        var yaml = """
            name: test-pack
            templates:
              - domain
              - infrastructure
              - api
            """;

        // Act
        var manifest = _loader.LoadPackManifestFromString(yaml);

        // Assert
        Assert.Equal(3, manifest.Templates.Count);
        Assert.Contains("domain", manifest.Templates);
        Assert.Contains("infrastructure", manifest.Templates);
        Assert.Contains("api", manifest.Templates);
    }

    [Fact]
    public void LoadPackManifest_ParsesVariables()
    {
        // Arrange
        var yaml = """
            name: test-pack
            variables:
              nullable: enable
              use_records: true
              max_length: 100
            """;

        // Act
        var manifest = _loader.LoadPackManifestFromString(yaml);

        // Assert
        Assert.Equal(3, manifest.Variables.Count);
        Assert.Equal("enable", manifest.Variables["nullable"]);
        Assert.Equal("true", manifest.Variables["use_records"].ToString());
        Assert.Equal("100", manifest.Variables["max_length"].ToString());
    }

    [Fact]
    public void LoadPackManifest_DefaultsVersion()
    {
        // Arrange
        var yaml = """
            name: minimal-pack
            """;

        // Act
        var manifest = _loader.LoadPackManifestFromString(yaml);

        // Assert
        Assert.Equal("1.0.0", manifest.Version);
    }

    [Fact]
    public void LoadTemplateManifest_ParsesBasicFields()
    {
        // Arrange
        var yaml = """
            name: Domain Layer
            description: Generates domain records
            """;

        // Act
        var manifest = _loader.LoadTemplateManifestFromString(yaml);

        // Assert
        Assert.Equal("Domain Layer", manifest.Name);
        Assert.Equal("Generates domain records", manifest.Description);
    }

    [Fact]
    public void LoadTemplateManifest_ParsesGenerationEntries()
    {
        // Arrange
        var yaml = """
            name: Domain Layer
            generates:
              - template: entity.scriban
                per: entity
                output: "Entities/{entity.name}.g.cs"
                description: Domain entity record
              - template: enum.scriban
                per: enum
                output: "Enums/{enum.name}.g.cs"
            """;

        // Act
        var manifest = _loader.LoadTemplateManifestFromString(yaml);

        // Assert
        Assert.Equal(2, manifest.Generates.Count);
        
        var entityEntry = manifest.Generates[0];
        Assert.Equal("entity.scriban", entityEntry.Template);
        Assert.Equal("entity", entityEntry.Per);
        Assert.Equal("Entities/{entity.name}.g.cs", entityEntry.Output);
        Assert.Equal("Domain entity record", entityEntry.Description);
        
        var enumEntry = manifest.Generates[1];
        Assert.Equal("enum.scriban", enumEntry.Template);
        Assert.Equal("enum", enumEntry.Per);
    }

    [Fact]
    public void LoadTemplateManifest_ParsesSnippets()
    {
        // Arrange
        var yaml = """
            name: Test Template
            snippets:
              - _snippets/csharp/header
              - _snippets/csharp/property
            """;

        // Act
        var manifest = _loader.LoadTemplateManifestFromString(yaml);

        // Assert
        Assert.Equal(2, manifest.Snippets.Count);
        Assert.Contains("_snippets/csharp/header", manifest.Snippets);
        Assert.Contains("_snippets/csharp/property", manifest.Snippets);
    }
}

