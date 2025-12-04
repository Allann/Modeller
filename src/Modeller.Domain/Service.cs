namespace Modeller.Domain;

/// <summary>
/// Represents a bounded context or service within the domain
/// </summary>
/// <param name="Name">The service name</param>
/// <param name="Description">Optional description</param>
/// <param name="Domain">Reference to the parent domain</param>
/// <param name="Entities">Entities that belong to this service</param>
/// <param name="Commands">Commands that belong to this service</param>
/// <param name="Queries">Queries that belong to this service</param>
public sealed record Service(
    string Name,
    string? Description = null,
    Domain? Domain = null,
    IReadOnlyList<Entity>? Entities = null,
    IReadOnlyList<Command>? Commands = null,
    IReadOnlyList<Query>? Queries = null)
{
    public IReadOnlyList<Entity> Entities { get; init; } = Entities ?? [];
    public IReadOnlyList<Command> Commands { get; init; } = Commands ?? [];
    public IReadOnlyList<Query> Queries { get; init; } = Queries ?? [];
}

