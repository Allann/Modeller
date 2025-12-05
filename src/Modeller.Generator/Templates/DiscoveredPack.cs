using Modeller.Generator.Configuration;

namespace Modeller.Generator.Templates;

/// <summary>
/// Represents a discovered template pack with its manifest and templates.
/// </summary>
public sealed record DiscoveredPack
{
    /// <summary>
    /// Full path to the pack folder.
    /// </summary>
    public required string Path { get; init; }

    /// <summary>
    /// The language this pack targets (e.g., "csharp").
    /// </summary>
    public required string Language { get; init; }

    /// <summary>
    /// The pack manifest loaded from pack.yaml.
    /// </summary>
    public required PackManifest Manifest { get; init; }

    /// <summary>
    /// Templates discovered within this pack.
    /// </summary>
    public required IReadOnlyList<DiscoveredTemplate> Templates { get; init; }

    /// <summary>
    /// Gets the pack name from the manifest.
    /// </summary>
    public string Name => Manifest.Name;

    /// <summary>
    /// Gets the pack version from the manifest.
    /// </summary>
    public string Version => Manifest.Version;
}

/// <summary>
/// Represents a discovered template within a pack.
/// </summary>
public sealed record DiscoveredTemplate
{
    /// <summary>
    /// Full path to the template folder.
    /// </summary>
    public required string Path { get; init; }

    /// <summary>
    /// The template folder name (e.g., "infrastructure").
    /// </summary>
    public required string Name { get; init; }

    /// <summary>
    /// The template manifest loaded from template.yaml.
    /// </summary>
    public required TemplateManifest Manifest { get; init; }

    /// <summary>
    /// Gets the display name from the manifest.
    /// </summary>
    public string DisplayName => Manifest.Name;

    /// <summary>
    /// Gets the description from the manifest.
    /// </summary>
    public string Description => Manifest.Description;

    /// <summary>
    /// Gets the generation entries from the manifest.
    /// </summary>
    public IReadOnlyList<GenerationEntry> Generates => Manifest.Generates;
}

