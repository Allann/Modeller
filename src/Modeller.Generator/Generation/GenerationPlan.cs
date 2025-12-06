namespace Modeller.Generator.Generation;

/// <summary>
/// Represents a plan for generating files from templates
/// </summary>
public sealed record GenerationPlan
{
    /// <summary>
    /// Domain being generated from
    /// </summary>
    public required Domain.Domain Domain { get; init; }

    /// <summary>
    /// All files to be generated
    /// </summary>
    public required IReadOnlyList<PlannedFile> Files { get; init; }

    /// <summary>
    /// Layer name being generated
    /// </summary>
    public required string LayerName { get; init; }

    /// <summary>
    /// Output root directory
    /// </summary>
    public required string OutputRoot { get; init; }
}

/// <summary>
/// Represents a single file to be generated
/// </summary>
public sealed record PlannedFile
{
    /// <summary>
    /// Full output path for the file
    /// </summary>
    public required string OutputPath { get; init; }

    /// <summary>
    /// Relative path within the layer output folder
    /// </summary>
    public required string RelativePath { get; init; }

    /// <summary>
    /// Path to the Scriban template file
    /// </summary>
    public required string TemplatePath { get; init; }

    /// <summary>
    /// Template context with variables and domain element
    /// </summary>
    public required Dictionary<string, object> Context { get; init; }

    /// <summary>
    /// Description of what is being generated
    /// </summary>
    public required string Description { get; init; }

    /// <summary>
    /// Whether this file can be overwritten (has .g. in name)
    /// </summary>
    public bool IsGenerated => RelativePath.Contains(".g.");

    /// <summary>
    /// Type of domain element being generated (entity, enum, etc.)
    /// </summary>
    public required string ElementType { get; init; }

    /// <summary>
    /// Name of the domain element (for display)
    /// </summary>
    public string? ElementName { get; init; }
}

/// <summary>
/// Result of generating a single file
/// </summary>
public sealed record GenerationResult
{
    public required PlannedFile File { get; init; }
    public required bool Success { get; init; }
    public required GenerationAction Action { get; init; }
    public string? Error { get; init; }
}

/// <summary>
/// What action was taken for a file
/// </summary>
public enum GenerationAction
{
    /// <summary>File was created (didn't exist)</summary>
    Created,
    /// <summary>File was overwritten (had .g. suffix)</summary>
    Overwritten,
    /// <summary>File was skipped (exists, no .g. suffix)</summary>
    Skipped,
    /// <summary>File generation failed</summary>
    Failed,
    /// <summary>Dry run - no action taken</summary>
    DryRun
}

