namespace Modeller.Domain;

/// <summary>
/// Represents a domain entity with its attributes and relationships
/// </summary>
/// <param name="Name">The entity name</param>
/// <param name="Description">Optional description</param>
/// <param name="IsAggregateRoot">Whether this is an aggregate root</param>
/// <param name="Service">Reference to the parent service (if any)</param>
/// <param name="Attributes">The entity's attributes/fields</param>
/// <param name="Relationships">Relationships to other entities</param>
/// <param name="Key">Key definition for this entity (if defined)</param>
public sealed record Entity(
    string Name,
    string? Description = null,
    bool IsAggregateRoot = false,
    Service? Service = null,
    IReadOnlyList<Attribute>? Attributes = null,
    IReadOnlyList<Relationship>? Relationships = null,
    Key? Key = null)
{
    public IReadOnlyList<Attribute> Attributes { get; init; } = Attributes ?? [];
    public IReadOnlyList<Relationship> Relationships { get; init; } = Relationships ?? [];
}

/// <summary>
/// Represents an attribute/field on an entity
/// </summary>
/// <param name="Name">The attribute name</param>
/// <param name="DataType">The data type</param>
/// <param name="Description">Optional description</param>
/// <param name="IsRequired">Whether the attribute is required</param>
/// <param name="DefaultValue">Optional default value</param>
public sealed record Attribute(
    string Name,
    DataType DataType,
    string? Description = null,
    bool IsRequired = true,
    string? DefaultValue = null);

/// <summary>
/// Represents a data type with optional constraints
/// </summary>
/// <param name="TypeName">The type name</param>
/// <param name="Length">Optional length constraint</param>
/// <param name="Precision">Optional precision for decimals</param>
/// <param name="Scale">Optional scale for decimals</param>
public sealed record DataType(
    string TypeName,
    int? Length = null,
    int? Precision = null,
    int? Scale = null)
{
    /// <summary>
    /// Returns a string representation like "text(100)" or "decimal(18,2)"
    /// </summary>
    public override string ToString()
    {
        if (Precision.HasValue && Scale.HasValue)
            return $"{TypeName}({Precision},{Scale})";
        if (Length.HasValue)
            return $"{TypeName}({Length})";
        return TypeName;
    }
}

/// <summary>
/// Represents a relationship between entities
/// </summary>
/// <param name="Type">The relationship type</param>
/// <param name="TargetEntityName">The target entity name</param>
/// <param name="Alias">Optional alias for the relationship</param>
/// <param name="TargetEntity">Resolved reference to the target entity</param>
public sealed record Relationship(
    RelationshipType Type,
    string TargetEntityName,
    string? Alias = null,
    Entity? TargetEntity = null);

/// <summary>
/// Types of relationships between entities
/// </summary>
public enum RelationshipType
{
    HasOne,
    HasMany,
    BelongsTo
}

/// <summary>
/// Represents a key definition for an entity
/// </summary>
/// <param name="Fields">The key fields</param>
/// <param name="Indexes">Optional indexes</param>
public sealed record Key(
    IReadOnlyList<KeyField>? Fields = null,
    IReadOnlyList<Index>? Indexes = null)
{
    public IReadOnlyList<KeyField> Fields { get; init; } = Fields ?? [];
    public IReadOnlyList<Index> Indexes { get; init; } = Indexes ?? [];
}

/// <summary>
/// Represents a field in a key
/// </summary>
/// <param name="Name">The field name</param>
/// <param name="TypeName">The type name</param>
/// <param name="IsGenerated">Whether auto-generated</param>
public sealed record KeyField(
    string Name,
    string TypeName,
    bool IsGenerated = false);

/// <summary>
/// Represents an index on an entity
/// </summary>
/// <param name="Fields">The indexed fields</param>
/// <param name="IsUnique">Whether unique constraint</param>
public sealed record Index(
    IReadOnlyList<string>? Fields = null,
    bool IsUnique = false)
{
    public IReadOnlyList<string> Fields { get; init; } = Fields ?? [];
}

