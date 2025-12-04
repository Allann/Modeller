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
/// <param name="References">External references</param>
/// <param name="Span">Source location</param>
public sealed record ServiceNode(
    string Name,
    string? Description = null,
    IReadOnlyList<string>? Entities = null,
    IReadOnlyList<string>? Enums = null,
    IReadOnlyList<ReferenceNode>? References = null,
    SourceSpan Span = default) : AstNode(Span);

/// <summary>
/// An external reference to another service's entities
/// </summary>
/// <param name="ServiceName">The referenced service name</param>
/// <param name="EntityNames">List of referenced entity names</param>
/// <param name="Span">Source location</param>
public sealed record ReferenceNode(
    string ServiceName,
    IReadOnlyList<string> EntityNames,
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
    ManyToMany
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
