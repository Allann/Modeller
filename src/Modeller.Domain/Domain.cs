namespace Modeller.Domain;

/// <summary>
/// Represents a complete domain model with all its services, entities, and behaviours.
/// This is the semantic model that resolves references and provides a rich API for code generation.
/// </summary>
/// <param name="Name">The domain name</param>
/// <param name="Description">Optional description</param>
/// <param name="Version">Optional version</param>
/// <param name="Services">Services in this domain</param>
/// <param name="Entities">Entities in this domain</param>
/// <param name="Enums">Enumerations in this domain</param>
/// <param name="Commands">Commands in this domain</param>
/// <param name="Queries">Queries in this domain</param>
public sealed record Domain(
    string Name,
    string? Description = null,
    string? Version = null,
    IReadOnlyList<Service>? Services = null,
    IReadOnlyList<Entity>? Entities = null,
    IReadOnlyList<Enumeration>? Enums = null,
    IReadOnlyList<Command>? Commands = null,
    IReadOnlyList<Query>? Queries = null)
{
    public IReadOnlyList<Service> Services { get; init; } = Services ?? [];
    public IReadOnlyList<Entity> Entities { get; init; } = Entities ?? [];
    public IReadOnlyList<Enumeration> Enums { get; init; } = Enums ?? [];
    public IReadOnlyList<Command> Commands { get; init; } = Commands ?? [];
    public IReadOnlyList<Query> Queries { get; init; } = Queries ?? [];

    /// <summary>
    /// Gets an entity by name, or null if not found
    /// </summary>
    public Entity? GetEntity(string name) =>
        Entities.FirstOrDefault(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Gets a service by name, or null if not found
    /// </summary>
    public Service? GetService(string name) =>
        Services.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

    /// <summary>
    /// Gets an enum by name, or null if not found
    /// </summary>
    public Enumeration? GetEnum(string name) =>
        Enums.FirstOrDefault(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
}

