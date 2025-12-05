using YamlDotNet.Serialization;

namespace Modeller.Generator.Configuration;

/// <summary>
/// Represents a template manifest (template.yaml).
/// Describes what files a template generates and how.
/// </summary>
public sealed record TemplateManifest
{
    /// <summary>
    /// Template name (e.g., "Infrastructure Layer")
    /// </summary>
    [YamlMember(Alias = "name")]
    public string Name { get; init; } = string.Empty;

    /// <summary>
    /// Template description
    /// </summary>
    [YamlMember(Alias = "description")]
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// List of files this template generates
    /// </summary>
    [YamlMember(Alias = "generates")]
    public List<GenerationEntry> Generates { get; init; } = [];

    /// <summary>
    /// List of snippets this template requires
    /// </summary>
    [YamlMember(Alias = "snippets")]
    public List<string> Snippets { get; init; } = [];
}

/// <summary>
/// Represents a single file generation entry within a template.
/// </summary>
public sealed record GenerationEntry
{
    /// <summary>
    /// The Scriban template file to use (e.g., "entity.scriban")
    /// </summary>
    [YamlMember(Alias = "template")]
    public string Template { get; init; } = string.Empty;

    /// <summary>
    /// What to iterate over: "entity", "enum", "command", "query", "projection", or "domain"
    /// </summary>
    [YamlMember(Alias = "per")]
    public string Per { get; init; } = "domain";

    /// <summary>
    /// Output file path pattern with template variables.
    /// E.g., "Entities/{entity.name | pascal_case}.g.cs"
    /// </summary>
    [YamlMember(Alias = "output")]
    public string Output { get; init; } = string.Empty;

    /// <summary>
    /// Description of what this generates
    /// </summary>
    [YamlMember(Alias = "description")]
    public string Description { get; init; } = string.Empty;

    /// <summary>
    /// Optional condition for when to generate this file
    /// </summary>
    [YamlMember(Alias = "condition")]
    public string? Condition { get; init; }
}

