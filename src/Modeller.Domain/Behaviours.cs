namespace Modeller.Domain;

/// <summary>
/// Represents a command (write operation) in the domain
/// </summary>
public sealed record Command
{
    internal Command(
        Guid publicId,
        string name,
        string? description = null,
        IReadOnlyList<Attribute>? inputs = null,
        string? outputTypeName = null,
        Entity? outputEntity = null,
        IReadOnlyList<ErrorType>? errors = null,
        IReadOnlyList<string>? events = null) =>
        (PublicId, Name, Description, Inputs, OutputTypeName, OutputEntity, Errors, Events) =
        (publicId, name, description, inputs ?? [], outputTypeName, outputEntity, errors ?? [], events ?? []);

    public Guid PublicId { get; }
    public string Name { get; }
    public string? Description { get; }
    public IReadOnlyList<Attribute> Inputs { get; }
    public string? OutputTypeName { get; }
    public Entity? OutputEntity { get; }
    public IReadOnlyList<ErrorType> Errors { get; }
    public IReadOnlyList<string> Events { get; }
}

/// <summary>
/// Extension methods for Command
/// </summary>
public static class CommandExtensions
{
    extension(Command)
    {
        public static Command? New(
            string name,
            string? description = null,
            IReadOnlyList<Attribute>? inputs = null,
            string? outputTypeName = null,
            Entity? outputEntity = null,
            IReadOnlyList<ErrorType>? errors = null,
            IReadOnlyList<string>? events = null) =>
            Command.CreateValid(Guid.CreateVersion7(), name, description, inputs, outputTypeName, outputEntity, errors, events);

        public static Command? CreateValid(
            Guid publicId,
            string name,
            string? description = null,
            IReadOnlyList<Attribute>? inputs = null,
            string? outputTypeName = null,
            Entity? outputEntity = null,
            IReadOnlyList<ErrorType>? errors = null,
            IReadOnlyList<string>? events = null) =>
            string.IsNullOrWhiteSpace(name) ? null
            : !publicId.IsVersion7() ? null
            : new Command(publicId, name, description, inputs, outputTypeName, outputEntity, errors, events);
    }
}

/// <summary>
/// Represents a query (read operation) in the domain
/// </summary>
public sealed record Query
{
    internal Query(
        Guid publicId,
        string name,
        string? description = null,
        IReadOnlyList<Attribute>? inputs = null,
        string? outputTypeName = null,
        bool returnsCollection = false,
        Entity? outputEntity = null) =>
        (PublicId, Name, Description, Inputs, OutputTypeName, ReturnsCollection, OutputEntity) =
        (publicId, name, description, inputs ?? [], outputTypeName, returnsCollection, outputEntity);

    public Guid PublicId { get; }
    public string Name { get; }
    public string? Description { get; }
    public IReadOnlyList<Attribute> Inputs { get; }
    public string? OutputTypeName { get; }
    public bool ReturnsCollection { get; }
    public Entity? OutputEntity { get; }
}

/// <summary>
/// Extension methods for Query
/// </summary>
public static class QueryExtensions
{
    extension(Query)
    {
        public static Query? New(
            string name,
            string? description = null,
            IReadOnlyList<Attribute>? inputs = null,
            string? outputTypeName = null,
            bool returnsCollection = false,
            Entity? outputEntity = null) =>
            Query.CreateValid(Guid.CreateVersion7(), name, description, inputs, outputTypeName, returnsCollection, outputEntity);

        public static Query? CreateValid(
            Guid publicId,
            string name,
            string? description = null,
            IReadOnlyList<Attribute>? inputs = null,
            string? outputTypeName = null,
            bool returnsCollection = false,
            Entity? outputEntity = null) =>
            string.IsNullOrWhiteSpace(name) ? null
            : !publicId.IsVersion7() ? null
            : new Query(publicId, name, description, inputs, outputTypeName, returnsCollection, outputEntity);
    }
}

/// <summary>
/// Represents an error type that a command can return
/// </summary>
public sealed record ErrorType
{
    internal ErrorType(
        string name,
        string? description = null) =>
        (Name, Description) = (name, description);

    public string Name { get; }
    public string? Description { get; }
}

/// <summary>
/// Extension methods for ErrorType
/// </summary>
public static class ErrorTypeExtensions
{
    extension(ErrorType)
    {
        public static ErrorType? New(
            string name,
            string? description = null) =>
            string.IsNullOrWhiteSpace(name) ? null
            : new ErrorType(name, description);
    }
}
