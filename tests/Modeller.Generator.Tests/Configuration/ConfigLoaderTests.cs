using Modeller.Generator.Configuration;

namespace Modeller.Generator.Tests.Configuration;

public class ConfigLoaderTests
{
    private readonly ConfigLoader _loader = new();

    [Fact]
    public void LoadProjectConfigFromString_ParsesBasicFields()
    {
        // Arrange
        var yaml = """
            version: 1
            domain: ./samples/units
            template_source: file://C:/templates
            default_profile: custom
            """;

        // Act
        var config = _loader.LoadProjectConfigFromString(yaml);

        // Assert
        Assert.Equal(1, config.Version);
        Assert.Equal("./samples/units", config.Domain);
        Assert.Equal("file://C:/templates", config.TemplateSource);
        Assert.Equal("custom", config.DefaultProfile);
    }

    [Fact]
    public void LoadProjectConfigFromString_ParsesVariables()
    {
        // Arrange
        var yaml = """
            version: 1
            variables:
              company: JJs
              product: UnitsManagement
              copyright: "© 2024 JJs"
              root_namespace: JJs.UnitsManagement
            """;

        // Act
        var config = _loader.LoadProjectConfigFromString(yaml);

        // Assert
        Assert.Equal("JJs", config.Variables["company"]);
        Assert.Equal("UnitsManagement", config.Variables["product"]);
        Assert.Equal("© 2024 JJs", config.Variables["copyright"]);
        Assert.Equal("JJs.UnitsManagement", config.Variables["root_namespace"]);
    }

    [Fact]
    public void LoadProjectConfigFromString_ParsesOutputConfig()
    {
        // Arrange
        var yaml = """
            version: 1
            output:
              root: ./src
              project_pattern: "{company}.{product}.{layer}"
            """;

        // Act
        var config = _loader.LoadProjectConfigFromString(yaml);

        // Assert
        Assert.Equal("./src", config.Output.Root);
        Assert.Equal("{company}.{product}.{layer}", config.Output.ProjectPattern);
    }

    [Fact]
    public void LoadProjectConfigFromString_ParsesFileConfig()
    {
        // Arrange
        var yaml = """
            version: 1
            files:
              generated_suffix: ".g"
              line_ending: lf
              encoding: utf-8
            """;

        // Act
        var config = _loader.LoadProjectConfigFromString(yaml);

        // Assert
        Assert.Equal(".g", config.Files.GeneratedSuffix);
        Assert.Equal("lf", config.Files.LineEnding);
        Assert.Equal("utf-8", config.Files.Encoding);
    }

    [Fact]
    public void LoadProfileFromString_ParsesBasicFields()
    {
        // Arrange
        var yaml = """
            name: Full Stack
            description: Generates all layers
            pack: csharp/clean-architecture
            """;

        // Act
        var profile = _loader.LoadProfileFromString(yaml);

        // Assert
        Assert.Equal("Full Stack", profile.Name);
        Assert.Equal("Generates all layers", profile.Description);
        Assert.Equal("csharp/clean-architecture", profile.Pack);
    }

    [Fact]
    public void LoadProfileFromString_ParsesLayers()
    {
        // Arrange
        var yaml = """
            name: Test
            layers:
              - name: Infrastructure
                template: infrastructure
                output: "{company}.{product}.Infrastructure"
              - name: Api
                template: api
                output: "{company}.{product}.Api"
            """;

        // Act
        var profile = _loader.LoadProfileFromString(yaml);

        // Assert
        Assert.Equal(2, profile.Layers.Count);
        Assert.Equal("Infrastructure", profile.Layers[0].Name);
        Assert.Equal("infrastructure", profile.Layers[0].Template);
        Assert.Equal("Api", profile.Layers[1].Name);
    }

    [Fact]
    public void LoadProfileFromString_ParsesLayerVariables()
    {
        // Arrange
        var yaml = """
            name: Test
            layer_variables:
              Infrastructure:
                use_audit_fields: true
                generate_configurations: true
              Sdk:
                include_validation: true
            """;

        // Act
        var profile = _loader.LoadProfileFromString(yaml);

        // Assert
        Assert.Equal(2, profile.LayerVariables.Count);
        Assert.True(profile.LayerVariables.ContainsKey("Infrastructure"));
        Assert.Equal("true", profile.LayerVariables["Infrastructure"]["use_audit_fields"].ToString());
    }
}

