namespace Modeller.Domain;

/// <summary>
/// Represents a command (write operation) in the domain
/// </summary>
/// <param name="Name">The command name</param>
/// <param name="Description">Optional description</param>
/// <param name="Inputs">Input parameters for the command</param>
/// <param name="OutputTypeName">The output type name (if any)</param>
/// <param name="OutputEntity">Resolved output entity</param>
/// <param name="Errors">Error types this command can return</param>
/// <param name="Events">Events this command publishes</param>
public sealed record Command(
    string Name,
    string? Description = null,
    IReadOnlyList<Attribute>? Inputs = null,
    string? OutputTypeName = null,
    Entity? OutputEntity = null,
    IReadOnlyList<ErrorType>? Errors = null,
    IReadOnlyList<string>? Events = null)
{
    public IReadOnlyList<Attribute> Inputs { get; init; } = Inputs ?? [];
    public IReadOnlyList<ErrorType> Errors { get; init; } = Errors ?? [];
    public IReadOnlyList<string> Events { get; init; } = Events ?? [];
}

/// <summary>
/// Represents a query (read operation) in the domain
/// </summary>
/// <param name="Name">The query name</param>
/// <param name="Description">Optional description</param>
/// <param name="Inputs">Input parameters for the query</param>
/// <param name="OutputTypeName">The output type name</param>
/// <param name="ReturnsCollection">Whether the output is a collection</param>
/// <param name="OutputEntity">Resolved output entity</param>
public sealed record Query(
    string Name,
    string? Description = null,
    IReadOnlyList<Attribute>? Inputs = null,
    string? OutputTypeName = null,
    bool ReturnsCollection = false,
    Entity? OutputEntity = null)
{
    public IReadOnlyList<Attribute> Inputs { get; init; } = Inputs ?? [];
}

/// <summary>
/// Represents an error type that a command can return
/// </summary>
/// <param name="Name">The error name</param>
/// <param name="Description">Optional description</param>
public sealed record ErrorType(
    string Name,
    string? Description = null);

