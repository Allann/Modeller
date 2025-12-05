using Modeller.Generator.Templates;

namespace Modeller.Generator.Tests.TemplatesTests;

public class TemplateDiscoveryTests : IDisposable
{
    private readonly string _tempDir;

    public TemplateDiscoveryTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), $"TemplateDiscoveryTests_{Guid.NewGuid():N}");
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDir))
        {
            Directory.Delete(_tempDir, recursive: true);
        }
    }

    [Fact]
    public void DiscoverPacks_ReturnsEmptyForNonExistentFolder()
    {
        // Arrange
        var discovery = new TemplateDiscovery(Path.Combine(_tempDir, "nonexistent"));

        // Act
        var packs = discovery.DiscoverPacks().ToList();

        // Assert
        Assert.Empty(packs);
    }

    [Fact]
    public void DiscoverPacks_FindsPackWithManifest()
    {
        // Arrange
        var packDir = Path.Combine(_tempDir, "csharp", "clean-architecture");
        Directory.CreateDirectory(packDir);
        File.WriteAllText(Path.Combine(packDir, "pack.yaml"), """
            name: clean-architecture
            version: 1.0.0
            description: Test pack
            """);

        var discovery = new TemplateDiscovery(_tempDir);

        // Act
        var packs = discovery.DiscoverPacks().ToList();

        // Assert
        Assert.Single(packs);
        Assert.Equal("clean-architecture", packs[0].Name);
        Assert.Equal("csharp", packs[0].Language);
    }

    [Fact]
    public void DiscoverPacks_SkipsSnippetsFolder()
    {
        // Arrange
        var snippetsDir = Path.Combine(_tempDir, "_snippets", "csharp");
        Directory.CreateDirectory(snippetsDir);
        File.WriteAllText(Path.Combine(snippetsDir, "pack.yaml"), """
            name: should-be-ignored
            """);

        var discovery = new TemplateDiscovery(_tempDir);

        // Act
        var packs = discovery.DiscoverPacks().ToList();

        // Assert
        Assert.Empty(packs);
    }

    [Fact]
    public void DiscoverPacks_FindsTemplatesWithinPack()
    {
        // Arrange
        var packDir = Path.Combine(_tempDir, "csharp", "test-pack");
        var domainDir = Path.Combine(packDir, "domain");
        var infraDir = Path.Combine(packDir, "infrastructure");
        
        Directory.CreateDirectory(domainDir);
        Directory.CreateDirectory(infraDir);
        
        File.WriteAllText(Path.Combine(packDir, "pack.yaml"), """
            name: test-pack
            """);
        File.WriteAllText(Path.Combine(domainDir, "template.yaml"), """
            name: Domain Layer
            """);
        File.WriteAllText(Path.Combine(infraDir, "template.yaml"), """
            name: Infrastructure Layer
            """);

        var discovery = new TemplateDiscovery(_tempDir);

        // Act
        var packs = discovery.DiscoverPacks().ToList();

        // Assert
        Assert.Single(packs);
        Assert.Equal(2, packs[0].Templates.Count);
        Assert.Contains(packs[0].Templates, t => t.Name == "domain");
        Assert.Contains(packs[0].Templates, t => t.Name == "infrastructure");
    }

    [Fact]
    public void GetPack_ReturnsNullForNonExistent()
    {
        // Arrange
        var discovery = new TemplateDiscovery(_tempDir);

        // Act
        var pack = discovery.GetPack("csharp", "nonexistent");

        // Assert
        Assert.Null(pack);
    }

    [Fact]
    public void GetPack_ReturnsPackByLanguageAndName()
    {
        // Arrange
        var packDir = Path.Combine(_tempDir, "csharp", "my-pack");
        Directory.CreateDirectory(packDir);
        File.WriteAllText(Path.Combine(packDir, "pack.yaml"), """
            name: my-pack
            version: 2.0.0
            """);

        var discovery = new TemplateDiscovery(_tempDir);

        // Act
        var pack = discovery.GetPack("csharp", "my-pack");

        // Assert
        Assert.NotNull(pack);
        Assert.Equal("my-pack", pack.Name);
        Assert.Equal("2.0.0", pack.Version);
    }

    [Fact]
    public void GetTemplate_ReturnsTemplateByPath()
    {
        // Arrange
        var templateDir = Path.Combine(_tempDir, "csharp", "pack", "domain");
        Directory.CreateDirectory(templateDir);
        File.WriteAllText(Path.Combine(templateDir, "template.yaml"), """
            name: Domain Layer
            description: Generates domain entities
            """);

        var discovery = new TemplateDiscovery(_tempDir);

        // Act
        var template = discovery.GetTemplate("csharp", "pack", "domain");

        // Assert
        Assert.NotNull(template);
        Assert.Equal("domain", template.Name);
        Assert.Equal("Domain Layer", template.DisplayName);
    }
}

