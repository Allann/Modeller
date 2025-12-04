namespace Modeller.Generator;

/// <summary>
/// Interface for code generators
/// </summary>
public interface ICodeGenerator
{
    /// <summary>
    /// The name of this generator
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Generates code for the given domain
    /// </summary>
    IEnumerable<GeneratedFile> Generate(Domain.Domain domain);
}

/// <summary>
/// Represents a generated file
/// </summary>
public sealed class GeneratedFile
{
    public required string Path { get; init; }
    public required string Content { get; init; }
}

