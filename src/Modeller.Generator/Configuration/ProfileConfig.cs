using YamlDotNet.Serialization;

namespace Modeller.Generator.Configuration;

/// <summary>
/// Model for .modeller/profiles/*.yaml - generation profile configuration
/// </summary>
public sealed class ProfileConfig
{
    /// <summary>
    /// Display name for the profile
    /// </summary>
    [YamlMember(Alias = "name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Description of what this profile generates
    /// </summary>
    [YamlMember(Alias = "description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Which template pack to use (e.g., "csharp/clean-architecture")
    /// </summary>
    [YamlMember(Alias = "pack")]
    public string Pack { get; set; } = string.Empty;

    /// <summary>
    /// Layers to generate
    /// </summary>
    [YamlMember(Alias = "layers")]
    public List<LayerConfig> Layers { get; set; } = [];

    /// <summary>
    /// What domain elements to include
    /// </summary>
    [YamlMember(Alias = "include")]
    public IncludeExcludeConfig Include { get; set; } = new();

    /// <summary>
    /// What domain elements to exclude
    /// </summary>
    [YamlMember(Alias = "exclude")]
    public IncludeExcludeConfig Exclude { get; set; } = new();

    /// <summary>
    /// Layer-specific variable overrides
    /// </summary>
    [YamlMember(Alias = "layer_variables")]
    public Dictionary<string, Dictionary<string, object>> LayerVariables { get; set; } = [];
}

/// <summary>
/// Configuration for a single layer in a profile
/// </summary>
public sealed class LayerConfig
{
    /// <summary>
    /// Display name for the layer
    /// </summary>
    [YamlMember(Alias = "name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Template to use for this layer (matches template folder name)
    /// </summary>
    [YamlMember(Alias = "template")]
    public string Template { get; set; } = string.Empty;

    /// <summary>
    /// Output folder pattern for this layer
    /// </summary>
    [YamlMember(Alias = "output")]
    public string Output { get; set; } = string.Empty;
}

/// <summary>
/// Configuration for including/excluding domain elements
/// </summary>
public sealed class IncludeExcludeConfig
{
    /// <summary>
    /// Entities to include/exclude: "all" or list of names
    /// </summary>
    [YamlMember(Alias = "entities")]
    public object Entities { get; set; } = "all";

    /// <summary>
    /// Enums to include/exclude: "all" or list of names
    /// </summary>
    [YamlMember(Alias = "enums")]
    public object Enums { get; set; } = "all";

    /// <summary>
    /// Commands to include/exclude: "all" or list of names
    /// </summary>
    [YamlMember(Alias = "commands")]
    public object Commands { get; set; } = "all";

    /// <summary>
    /// Queries to include/exclude: "all" or list of names
    /// </summary>
    [YamlMember(Alias = "queries")]
    public object Queries { get; set; } = "all";

    /// <summary>
    /// Projections to include/exclude: "all" or list of names
    /// </summary>
    [YamlMember(Alias = "projections")]
    public object Projections { get; set; } = "all";

    /// <summary>
    /// Check if a specific entity should be included
    /// </summary>
    public bool ShouldInclude(string category, string name)
    {
        var value = category.ToLowerInvariant() switch
        {
            "entities" => Entities,
            "enums" => Enums,
            "commands" => Commands,
            "queries" => Queries,
            "projections" => Projections,
            _ => "all"
        };

        if (value is string s && s.Equals("all", StringComparison.OrdinalIgnoreCase))
            return true;

        if (value is List<object> list)
            return list.Any(item => item?.ToString()?.Equals(name, StringComparison.OrdinalIgnoreCase) == true);

        return true;
    }
}

