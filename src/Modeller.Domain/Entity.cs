namespace Modeller.Domain;

/// <summary>
/// Represents a domain entity with its attributes and relationships
/// </summary>
public sealed record Entity
{
    internal Entity(
        Guid publicId,
        string name,
        string? description = null,
        bool isAggregateRoot = false,
        Service? service = null,
        IReadOnlyList<Attribute>? attributes = null,
        IReadOnlyList<Relationship>? relationships = null,
        Key? key = null) =>
        (PublicId, Name, Description, IsAggregateRoot, Service, Attributes, Relationships, Key) =
        (publicId, name, description, isAggregateRoot, service, attributes ?? [], relationships ?? [], key);

    public Guid PublicId { get; }
    public string Name { get; }
    public string? Description { get; }
    public bool IsAggregateRoot { get; }
    public Service? Service { get; }
    public IReadOnlyList<Attribute> Attributes { get; }
    public IReadOnlyList<Relationship> Relationships { get; }
    public Key? Key { get; }
}

/// <summary>
/// Extension methods for Entity
/// </summary>
public static class EntityExtensions
{
    extension(Entity)
    {
        public static Entity? New(
            string name,
            string? description = null,
            bool isAggregateRoot = false,
            Service? service = null,
            IReadOnlyList<Attribute>? attributes = null,
            IReadOnlyList<Relationship>? relationships = null,
            Key? key = null) =>
            Entity.CreateValid(Guid.CreateVersion7(), name, description, isAggregateRoot, service, attributes, relationships, key);

        public static Entity? CreateValid(
            Guid publicId,
            string name,
            string? description = null,
            bool isAggregateRoot = false,
            Service? service = null,
            IReadOnlyList<Attribute>? attributes = null,
            IReadOnlyList<Relationship>? relationships = null,
            Key? key = null) =>
            string.IsNullOrWhiteSpace(name) ? null
            : publicId.Version != 7 ? null
            : new Entity(publicId, name, description, isAggregateRoot, service, attributes, relationships, key);
    }
}

/// <summary>
/// Represents an attribute/field on an entity
/// </summary>
public sealed record Attribute
{
    internal Attribute(
        string name,
        DataType dataType,
        string? description = null,
        bool isRequired = true,
        string? defaultValue = null) =>
        (Name, DataType, Description, IsRequired, DefaultValue) =
        (name, dataType, description, isRequired, defaultValue);

    public string Name { get; }
    public DataType DataType { get; }
    public string? Description { get; }
    public bool IsRequired { get; }
    public string? DefaultValue { get; }
}

/// <summary>
/// Extension methods for Attribute
/// </summary>
public static class AttributeExtensions
{
    extension(Attribute)
    {
        public static Attribute? New(
            string name,
            DataType dataType,
            string? description = null,
            bool isRequired = true,
            string? defaultValue = null) =>
            string.IsNullOrWhiteSpace(name) ? null
            : new Attribute(name, dataType, description, isRequired, defaultValue);
    }
}

/// <summary>
/// Represents a data type with optional constraints
/// </summary>
public sealed record DataType
{
    internal DataType(
        string typeName,
        int? length = null,
        int? precision = null,
        int? scale = null) =>
        (TypeName, Length, Precision, Scale) = (typeName, length, precision, scale);

    public string TypeName { get; }
    public int? Length { get; }
    public int? Precision { get; }
    public int? Scale { get; }

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
/// Extension methods for DataType
/// </summary>
public static class DataTypeExtensions
{
    extension(DataType)
    {
        public static DataType? New(
            string typeName,
            int? length = null,
            int? precision = null,
            int? scale = null) =>
            string.IsNullOrWhiteSpace(typeName) ? null
            : new DataType(typeName, length, precision, scale);
    }
}

/// <summary>
/// Represents a relationship between entities
/// </summary>
public sealed record Relationship
{
    internal Relationship(
        RelationshipType type,
        string targetEntityName,
        string? alias = null,
        Entity? targetEntity = null) =>
        (Type, TargetEntityName, Alias, TargetEntity) = (type, targetEntityName, alias, targetEntity);

    public RelationshipType Type { get; }
    public string TargetEntityName { get; }
    public string? Alias { get; }
    public Entity? TargetEntity { get; }
}

/// <summary>
/// Extension methods for Relationship
/// </summary>
public static class RelationshipExtensions
{
    extension(Relationship)
    {
        public static Relationship? New(
            RelationshipType type,
            string targetEntityName,
            string? alias = null,
            Entity? targetEntity = null) =>
            string.IsNullOrWhiteSpace(targetEntityName) ? null
            : new Relationship(type, targetEntityName, alias, targetEntity);
    }
}

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
public sealed record Key
{
    internal Key(
        IReadOnlyList<KeyField>? fields = null,
        IReadOnlyList<Index>? indexes = null) =>
        (Fields, Indexes) = (fields ?? [], indexes ?? []);

    public IReadOnlyList<KeyField> Fields { get; }
    public IReadOnlyList<Index> Indexes { get; }
}

/// <summary>
/// Extension methods for Key
/// </summary>
public static class KeyExtensions
{
    extension(Key)
    {
        public static Key New(
            IReadOnlyList<KeyField>? fields = null,
            IReadOnlyList<Index>? indexes = null) =>
            new Key(fields, indexes);
    }
}

/// <summary>
/// Represents a field in a key
/// </summary>
public sealed record KeyField
{
    internal KeyField(
        string name,
        string typeName,
        bool isGenerated = false) =>
        (Name, TypeName, IsGenerated) = (name, typeName, isGenerated);

    public string Name { get; }
    public string TypeName { get; }
    public bool IsGenerated { get; }
}

/// <summary>
/// Extension methods for KeyField
/// </summary>
public static class KeyFieldExtensions
{
    extension(KeyField)
    {
        public static KeyField? New(
            string name,
            string typeName,
            bool isGenerated = false) =>
            string.IsNullOrWhiteSpace(name) ? null
            : string.IsNullOrWhiteSpace(typeName) ? null
            : new KeyField(name, typeName, isGenerated);
    }
}

/// <summary>
/// Represents an index on an entity
/// </summary>
public sealed record Index
{
    internal Index(
        IReadOnlyList<string>? fields = null,
        bool isUnique = false) =>
        (Fields, IsUnique) = (fields ?? [], isUnique);

    public IReadOnlyList<string> Fields { get; }
    public bool IsUnique { get; }
}

/// <summary>
/// Extension methods for Index
/// </summary>
public static class IndexExtensions
{
    extension(Index)
    {
        public static Index New(
            IReadOnlyList<string>? fields = null,
            bool isUnique = false) =>
            new Index(fields, isUnique);
    }
}
