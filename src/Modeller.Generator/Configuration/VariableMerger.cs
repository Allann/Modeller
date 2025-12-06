namespace Modeller.Generator.Configuration;

/// <summary>
/// Merges variables from multiple sources with proper precedence:
/// config.yaml → profile → layer_variables → CLI overrides
/// </summary>
public sealed class VariableMerger
{
    /// <summary>
    /// Merge variables from all sources for a specific layer
    /// </summary>
    /// <param name="projectConfig">Base variables from config.yaml</param>
    /// <param name="profile">Profile configuration</param>
    /// <param name="layerName">Current layer name (for layer_variables lookup)</param>
    /// <param name="cliOverrides">Command-line variable overrides</param>
    /// <returns>Merged dictionary with all variables</returns>
    public Dictionary<string, object> MergeVariables(
        ProjectConfig projectConfig,
        ProfileConfig? profile = null,
        string? layerName = null,
        Dictionary<string, object>? cliOverrides = null)
    {
        var result = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase);

        // 1. Start with project config variables
        MergeDictionary(result, projectConfig.Variables);

        // 2. Add standard variables from config
        result["company"] = result.GetValueOrDefault("company", "");
        result["product"] = result.GetValueOrDefault("product", "");
        result["root_namespace"] = result.GetValueOrDefault("root_namespace", "");
        result["copyright"] = result.GetValueOrDefault("copyright", "");

        // 3. Apply profile layer variables if applicable
        if (profile != null && !string.IsNullOrEmpty(layerName))
        {
            if (profile.LayerVariables.TryGetValue(layerName, out var layerVars))
            {
                MergeDictionary(result, layerVars);
            }
        }

        // 4. Apply CLI overrides (highest precedence)
        if (cliOverrides != null)
        {
            MergeDictionary(result, cliOverrides);
        }

        // 5. Add layer name if available
        if (!string.IsNullOrEmpty(layerName))
        {
            result["layer"] = layerName;
        }

        return result;
    }

    /// <summary>
    /// Create a template context dictionary with variables and domain info
    /// </summary>
    public Dictionary<string, object> CreateTemplateContext(
        Dictionary<string, object> variables,
        object? entity = null,
        object? enumeration = null,
        object? command = null,
        object? query = null,
        object? projection = null,
        object? domain = null)
    {
        var context = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
        {
            ["variables"] = variables
        };

        // Add namespace from variables if present
        if (variables.TryGetValue("root_namespace", out var ns))
        {
            context["namespace"] = ns;
        }

        // Add domain elements
        if (entity != null) context["entity"] = entity;
        if (enumeration != null) context["enumeration"] = enumeration;
        if (command != null) context["command"] = command;
        if (query != null) context["query"] = query;
        if (projection != null) context["projection"] = projection;
        if (domain != null) context["domain"] = domain;

        return context;
    }

    /// <summary>
    /// Merge source dictionary into target, overwriting existing keys
    /// </summary>
    private static void MergeDictionary(
        Dictionary<string, object> target,
        Dictionary<string, object> source)
    {
        foreach (var kvp in source)
        {
            target[kvp.Key] = kvp.Value;
        }
    }

    /// <summary>
    /// Resolve a variable pattern like "{variables.company}.{variables.product}.{layer}"
    /// </summary>
    public string ResolvePattern(string pattern, Dictionary<string, object> variables)
    {
        if (string.IsNullOrEmpty(pattern))
            return pattern;

        var result = pattern;

        // Replace {variables.xxx} patterns
        foreach (var kvp in variables)
        {
            result = result.Replace($"{{variables.{kvp.Key}}}", kvp.Value?.ToString() ?? "");
            result = result.Replace($"{{{kvp.Key}}}", kvp.Value?.ToString() ?? "");
        }

        // Replace {layer} if present
        if (variables.TryGetValue("layer", out var layer))
        {
            result = result.Replace("{layer}", layer?.ToString() ?? "");
            result = result.Replace("{layer | pascal_case}", ToPascalCase(layer?.ToString() ?? ""));
        }

        return result;
    }

    private static string ToPascalCase(string value)
    {
        if (string.IsNullOrEmpty(value))
            return value;

        return char.ToUpperInvariant(value[0]) + value[1..];
    }
}

