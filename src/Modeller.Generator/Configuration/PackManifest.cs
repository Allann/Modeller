using YamlDotNet.Serialization;

namespace Modeller.Generator.Configuration;

/// <summary>
/// Represents a template pack manifest (pack.yaml).
/// A pack is a collection of related templates for a specific architecture pattern.
/// </summary>
public sealed record PackManifest
{
    /// <summary>
    /// Pack name (e.g., "clean-architecture")
    /// </summary>
    [YamlMember(Alias = "name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Pack version (e.g., "1.0.0")
    /// </summary>
    [YamlMember(Alias = "version")]
    public string Version { get; init; } = "1.0.0";

    /// <summary>
    /// Pack description
    /// </summary>
    [YamlMember(Alias = "description")]
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Pack author
    /// </summary>
    [YamlMember(Alias = "author")]
    public string Author { get; init; } = string.Empty;

    /// <summary>
    /// Target language (e.g., "csharp")
    /// </summary>
    [YamlMember(Alias = "language")]
    public string Language { get; init; } = "csharp";

    /// <summary>
    /// List of template names included in this pack
    /// </summary>
    [YamlMember(Alias = "templates")]
    public List<string> Templates { get; init; } = [];

    /// <summary>
    /// Default variables for this pack
    /// </summary>
    [YamlMember(Alias = "variables")]
    public Dictionary<string, object> Variables { get; init; } = [];
}

