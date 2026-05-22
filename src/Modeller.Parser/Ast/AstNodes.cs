namespace Modeller.Parser.Ast;

/// <summary>
/// Represents a span of source text for error reporting
/// </summary>
/// <param name="Start">Start position (0-based)</param>
/// <param name="End">End position (exclusive)</param>
/// <param name="Line">Line number (1-based)</param>
/// <param name="Column">Column number (1-based)</param>
public readonly record struct SourceSpan(int Start, int End, int Line, int Column)
{
    public int Length => End - Start;

    public static SourceSpan Empty => new(0, 0, 1, 1);
}

/// <summary>
/// Abstract base for all AST nodes - provides source location tracking
/// </summary>
/// <param name="Span">The source location of this node</param>
public abstract record AstNode(SourceSpan Span = default);

/// <summary>
/// A domain definition - the root container
/// </summary>
/// <param name="Name">The domain name</param>
/// <param name="Description">Optional description</param>
/// <param name="Company">Optional company name</param>
/// <param name="Version">Optional version string</param>
/// <param name="Services">List of service names</param>
/// <param name="Span">Source location</param>
public sealed record DomainNode(
    string Name,
    string? Description = null,
    string? Company = null,
    string? Version = null,
    IReadOnlyList<string>? Services = null,
    SourceSpan Span = default) : AstNode(Span);

/// <summary>
/// A service definition - a bounded context
/// </summary>
/// <param name="Name">The service name</param>
/// <param name="Description">Optional description</param>
/// <param name="Entities">List of entity names owned by this service</param>
/// <param name="Enums">List of enum names owned by this service</param>
/// <param name="References">Per-consumer shared type names this service reads from other contexts</param>
/// <param name="Calls">RPC command names this service invokes on other services</param>
/// <param name="Implements">RPC command names this service handles (is the provider for)</param>
/// <param name="Span">Source location</param>
public sealed record ServiceNode(
    string Name,
    string? Description = null,
    IReadOnlyList<string>? Entities = null,
    IReadOnlyList<string>? Enums = null,
    IReadOnlyList<string>? References = null,
    IReadOnlyList<string>? Calls = null,
    IReadOnlyList<string>? Implements = null,
    SourceSpan Span = default) : AstNode(Span);

/// <summary>
/// An entity definition - a domain object with identity
/// </summary>
/// <param name="Name">The entity name</param>
/// <param name="Description">Optional description</param>
/// <param name="Attributes">List of attributes</param>
/// <param name="Relationships">List of relationships</param>
/// <param name="Key">Optional key definition</param>
/// <param name="Span">Source location</param>
public sealed record EntityNode(
    string Name,
    string? Description = null,
    IReadOnlyList<AttributeNode>? Attributes = null,
    IReadOnlyList<RelationshipNode>? Relationships = null,
    KeyNode? Key = null,
    SourceSpan Span = default) : AstNode(Span);

/// <summary>
/// An attribute on an entity
/// </summary>
/// <param name="Name">The attribute name</param>
/// <param name="DataType">The data type</param>
/// <param name="Description">Optional description</param>
/// <param name="MaxLength">Optional max length for text types</param>
/// <param name="IsOptional">Whether the attribute is optional</param>
/// <param name="DefaultValue">Optional default value expression</param>
/// <param name="Span">Source location</param>
public sealed record AttributeNode(
    string Name,
    string DataType,
    string? Description = null,
    int? MaxLength = null,
    bool IsOptional = false,
    string? DefaultValue = null,
    SourceSpan Span = default) : AstNode(Span);

/// <summary>
/// A relationship between entities
/// </summary>
/// <param name="TargetEntity">The related entity name</param>
/// <param name="Type">The relationship type (has_one, has_many, belongs_to)</param>
/// <param name="Alias">Optional alias for the relationship</param>
/// <param name="Span">Source location</param>
public sealed record RelationshipNode(
    string TargetEntity,
    RelationshipType Type,
    string? Alias = null,
    SourceSpan Span = default) : AstNode(Span);

/// <summary>
/// Types of relationships
/// </summary>
public enum RelationshipType
{
    HasOne,
    HasMany,
    BelongsTo,
    ManyToMany,
    /// <summary>Cross-service reference — reads from another bounded context's shared type</summary>
    References
}

/// <summary>
/// Transport protocol for commands and queries
/// </summary>
public enum TransportType
{
    /// <summary>HTTP/REST (default)</summary>
    Http,
    /// <summary>gRPC — binary protocol, typically service-to-service</summary>
    Grpc
}

/// <summary>
/// Streaming mode for commands and queries
/// </summary>
public enum StreamingMode
{
    /// <summary>Request/response — no streaming (default)</summary>
    None,
    /// <summary>Server streams multiple responses to a single request</summary>
    Server,
    /// <summary>Client streams multiple requests, server replies once</summary>
    Client,
    /// <summary>Both sides stream independently</summary>
    Bidirectional
}

/// <summary>
/// Key definition for an entity
/// </summary>
/// <param name="EntityName">The entity this key belongs to</param>
/// <param name="Fields">The key fields</param>
/// <param name="Indexes">Optional indexes</param>
/// <param name="Span">Source location</param>
public sealed record KeyNode(
    string EntityName,
    IReadOnlyList<KeyFieldNode> Fields,
    IReadOnlyList<IndexNode>? Indexes = null,
    SourceSpan Span = default) : AstNode(Span);

/// <summary>
/// A field in a key definition
/// </summary>
/// <param name="Name">The field name</param>
/// <param name="DataType">The data type</param>
/// <param name="IsGenerated">Whether auto-generated</param>
/// <param name="Span">Source location</param>
public sealed record KeyFieldNode(
    string Name,
    string DataType,
    bool IsGenerated = false,
    SourceSpan Span = default) : AstNode(Span);

/// <summary>
/// An index definition
/// </summary>
/// <param name="Fields">The indexed fields</param>
/// <param name="IsUnique">Whether unique constraint</param>
/// <param name="Span">Source location</param>
public sealed record IndexNode(
    IReadOnlyList<string> Fields,
    bool IsUnique = false,
    SourceSpan Span = default) : AstNode(Span);

/// <summary>
/// An enumeration definition
/// </summary>
/// <param name="Name">The enum name</param>
/// <param name="Description">Optional description</param>
/// <param name="Values">The enum values</param>
/// <param name="Span">Source location</param>
public sealed record EnumNode(
    string Name,
    string? Description = null,
    IReadOnlyList<EnumValueNode>? Values = null,
    SourceSpan Span = default) : AstNode(Span);

/// <summary>
/// A value in an enumeration
/// </summary>
/// <param name="Name">The value name</param>
/// <param name="Value">The numeric value</param>
/// <param name="Description">Optional description</param>
/// <param name="Span">Source location</param>
public sealed record EnumValueNode(
    string Name,
    int Value,
    string? Description = null,
    SourceSpan Span = default) : AstNode(Span);

/// <summary>
/// A value object definition - an immutable object without identity
/// </summary>
/// <param name="Name">The value object name</param>
/// <param name="Description">Optional description</param>
/// <param name="Attributes">List of attributes</param>
/// <param name="Span">Source location</param>
public sealed record ValueNode(
    string Name,
    string? Description = null,
    IReadOnlyList<AttributeNode>? Attributes = null,
    SourceSpan Span = default) : AstNode(Span);

/// <summary>
/// A shared/lookup data definition - external data projection
/// </summary>
/// <param name="Name">The shared data name</param>
/// <param name="Description">Optional description</param>
/// <param name="Attributes">List of attributes exposed</param>
/// <param name="Span">Source location</param>
public sealed record SharedNode(
    string Name,
    string? Description = null,
    IReadOnlyList<AttributeNode>? Attributes = null,
    SourceSpan Span = default) : AstNode(Span);

/// <summary>
/// A domain event definition
/// </summary>
/// <param name="Name">The event name</param>
/// <param name="Description">Optional description</param>
/// <param name="Attributes">Event data attributes</param>
/// <param name="Span">Source location</param>
public sealed record EventNode(
    string Name,
    string? Description = null,
    IReadOnlyList<AttributeNode>? Attributes = null,
    SourceSpan Span = default) : AstNode(Span);

/// <summary>
/// A projection definition - query return shape
/// </summary>
/// <param name="Name">The projection name</param>
/// <param name="Description">Optional description</param>
/// <param name="Attributes">Projected attributes</param>
/// <param name="Span">Source location</param>
public sealed record ProjectionNode(
    string Name,
    string? Description = null,
    IReadOnlyList<AttributeNode>? Attributes = null,
    SourceSpan Span = default) : AstNode(Span);

/// <summary>
/// A single variant within a union type
/// </summary>
/// <param name="Name">The variant name</param>
/// <param name="Description">Optional description</param>
/// <param name="Attributes">Attributes present in this variant</param>
/// <param name="Span">Source location</param>
public sealed record UnionVariantNode(
    string Name,
    string? Description = null,
    IReadOnlyList<AttributeNode>? Attributes = null,
    SourceSpan Span = default) : AstNode(Span);

/// <summary>
/// A discriminated union type - a value that has one of several mutually exclusive shapes
/// </summary>
/// <param name="Name">The union type name</param>
/// <param name="Description">Optional description</param>
/// <param name="Variants">The possible variants</param>
/// <param name="Span">Source location</param>
public sealed record UnionNode(
    string Name,
    string? Description = null,
    IReadOnlyList<UnionVariantNode>? Variants = null,
    SourceSpan Span = default) : AstNode(Span);
