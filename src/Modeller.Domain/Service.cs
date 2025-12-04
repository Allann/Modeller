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
public sealed record Service
{
    internal Service(
        Guid publicId,
        string name,
        string? description = null,
        Domain? domain = null,
        IReadOnlyList<Entity>? entities = null,
        IReadOnlyList<Command>? commands = null,
        IReadOnlyList<Query>? queries = null) =>
        (PublicId, Name, Description, Domain, Entities, Commands, Queries) =
        (publicId, name, description, domain, entities ?? [], commands ?? [], queries ?? []);

    public Guid PublicId { get; }
    public string Name { get; }
    public string? Description { get; }
    public Domain? Domain { get; }

    public IReadOnlyList<Entity> Entities { get; }
    public IReadOnlyList<Command> Commands { get; }
    public IReadOnlyList<Query> Queries { get; }
}

/// <summary>
/// Extension methods for Service
/// </summary>
public static class ServiceExtensions
{
    extension(Service)
    {
        public static Service? New(
            string name,
            string? description = null,
            Domain? domain = null,
            IReadOnlyList<Entity>? entities = null,
            IReadOnlyList<Command>? commands = null,
            IReadOnlyList<Query>? queries = null) =>
            Service.CreateValid(Guid.CreateVersion7(), name, description, domain, entities, commands, queries);

        public static Service? CreateValid(
            Guid publicId,
            string name,
            string? description = null,
            Domain? domain = null,
            IReadOnlyList<Entity>? entities = null,
            IReadOnlyList<Command>? commands = null,
            IReadOnlyList<Query>? queries = null) =>
            string.IsNullOrWhiteSpace(name) ? null
            : !publicId.IsVersion7() ? null
            : new Service(publicId, name, description, domain, entities, commands, queries);
    }
}
