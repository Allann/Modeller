using Modeller.Generator.Configuration;

namespace Modeller.Generator.Tests.Configuration;

public class VariableMergerTests
{
    private readonly VariableMerger _merger = new();

    [Fact]
    public void MergeVariables_IncludesProjectConfigVariables()
    {
        // Arrange
        var projectConfig = new ProjectConfig
        {
            Variables = new Dictionary<string, object>
            {
                ["company"] = "JJs",
                ["product"] = "Units"
            }
        };

        // Act
        var result = _merger.MergeVariables(projectConfig);

        // Assert
        Assert.Equal("JJs", result["company"]);
        Assert.Equal("Units", result["product"]);
    }

    [Fact]
    public void MergeVariables_AppliesLayerVariables()
    {
        // Arrange
        var projectConfig = new ProjectConfig
        {
            Variables = new Dictionary<string, object>
            {
                ["use_audit"] = "false"
            }
        };

        var profile = new ProfileConfig
        {
            LayerVariables = new Dictionary<string, Dictionary<string, object>>
            {
                ["Infrastructure"] = new Dictionary<string, object>
                {
                    ["use_audit"] = "true"
                }
            }
        };

        // Act
        var result = _merger.MergeVariables(projectConfig, profile, "Infrastructure");

        // Assert
        Assert.Equal("true", result["use_audit"]);
    }

    [Fact]
    public void MergeVariables_CliOverridesTakePrecedence()
    {
        // Arrange
        var projectConfig = new ProjectConfig
        {
            Variables = new Dictionary<string, object>
            {
                ["company"] = "JJs",
                ["product"] = "Units"
            }
        };

        var cliOverrides = new Dictionary<string, object>
        {
            ["company"] = "Acme"
        };

        // Act
        var result = _merger.MergeVariables(projectConfig, cliOverrides: cliOverrides);

        // Assert
        Assert.Equal("Acme", result["company"]);
        Assert.Equal("Units", result["product"]);
    }

    [Fact]
    public void MergeVariables_AddsLayerName()
    {
        // Arrange
        var projectConfig = new ProjectConfig();
        var profile = new ProfileConfig();

        // Act
        var result = _merger.MergeVariables(projectConfig, profile, "Infrastructure");

        // Assert
        Assert.Equal("Infrastructure", result["layer"]);
    }

    [Fact]
    public void ResolvePattern_ReplacesVariables()
    {
        // Arrange
        var variables = new Dictionary<string, object>
        {
            ["company"] = "JJs",
            ["product"] = "Units",
            ["layer"] = "Infrastructure"
        };

        // Act
        var result = _merger.ResolvePattern("{variables.company}.{variables.product}.{layer}", variables);

        // Assert
        Assert.Equal("JJs.Units.Infrastructure", result);
    }

    [Fact]
    public void ResolvePattern_ReplacesShortVariables()
    {
        // Arrange
        var variables = new Dictionary<string, object>
        {
            ["company"] = "JJs",
            ["product"] = "Units"
        };

        // Act
        var result = _merger.ResolvePattern("{company}.{product}", variables);

        // Assert
        Assert.Equal("JJs.Units", result);
    }

    [Fact]
    public void CreateTemplateContext_IncludesVariablesAndEntity()
    {
        // Arrange
        var variables = new Dictionary<string, object>
        {
            ["root_namespace"] = "JJs.Units"
        };
        var entity = new { Name = "Unit", Description = "A unit" };

        // Act
        var context = _merger.CreateTemplateContext(variables, entity: entity);

        // Assert
        Assert.Equal("JJs.Units", context["namespace"]);
        Assert.Equal(entity, context["entity"]);
        Assert.Equal(variables, context["variables"]);
    }
}

