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
public sealed record Domain
{
    internal Domain(
        Guid publicId,
        string name,
        string? description = null,
        string? version = null,
        IReadOnlyList<Service>? services = null,
        IReadOnlyList<Entity>? entities = null,
        IReadOnlyList<Enumeration>? enums = null,
        IReadOnlyList<Command>? commands = null,
        IReadOnlyList<Query>? queries = null)=>
        (PublicId, Name, Description, Version, Services, Entities, Enums, Commands, Queries) = (publicId, name, description, version, services??[], entities??[], enums??[], commands??[], queries??[]);

    public Guid PublicId { get; }
    public string Name { get; }
    public string? Description { get; }
    public string? Version { get; }

    public IReadOnlyList<Service> Services { get; }
    public IReadOnlyList<Entity> Entities { get; }
    public IReadOnlyList<Enumeration> Enums { get; }
    public IReadOnlyList<Command> Commands { get; }
    public IReadOnlyList<Query> Queries { get; }
}

/// <summary>
/// Extension methods for Domain lookups
/// </summary>
public static class DomainExtensions
{
    extension(Domain)
    {
        public static Domain? New(string name,
        string? description = null,
        string? version = null,
        IReadOnlyList<Service>? services = null,
        IReadOnlyList<Entity>? entities = null,
        IReadOnlyList<Enumeration>? enums = null,
        IReadOnlyList<Command>? commands = null,
        IReadOnlyList<Query>? queries = null) =>
            Domain.CreateValid(Guid.CreateVersion7(), name, description, version, services, entities, enums, commands, queries);    

        public static Domain? CreateValid(
            Guid publicId,
            string name,
            string? description = null,
            string? version = null,
            IReadOnlyList<Service>? services = null,
            IReadOnlyList<Entity>? entities = null,
            IReadOnlyList<Enumeration>? enums = null,
            IReadOnlyList<Command>? commands = null,
            IReadOnlyList<Query>? queries = null) =>
                string.IsNullOrWhiteSpace(name) ? null
                : publicId.Version != 7 ? null 
                : new Domain(publicId, name, description, version, services, entities, enums, commands, queries);
    }

    extension(Domain domain)
    {
        /// <summary>
        /// Gets an entity by name, or null if not found
        /// </summary>
        public Entity? GetEntity(string name) =>
            domain.Entities.FirstOrDefault(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Gets a service by name, or null if not found
        /// </summary>
        public Service? GetService(string name) =>
            domain.Services.FirstOrDefault(s => s.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Gets an enum by name, or null if not found
        /// </summary>
        public Enumeration? GetEnum(string name) =>
            domain.Enums.FirstOrDefault(e => e.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
    }
}
