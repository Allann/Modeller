namespace Modeller.Domain;

/// <summary>
/// Represents an enumeration type
/// </summary>
public sealed record Enumeration
{
    internal Enumeration(
        Guid publicId,
        string name,
        string? description = null,
        IReadOnlyList<EnumValue>? values = null) =>
        (PublicId, Name, Description, Values) = (publicId, name, description, values ?? []);

    public Guid PublicId { get; }
    public string Name { get; }
    public string? Description { get; }
    public IReadOnlyList<EnumValue> Values { get; }
}

/// <summary>
/// Extension methods for Enumeration
/// </summary>
public static class EnumerationExtensions
{
    extension(Enumeration)
    {
        public static Enumeration? New(
            string name,
            string? description = null,
            IReadOnlyList<EnumValue>? values = null) =>
            Enumeration.CreateValid(Guid.CreateVersion7(), name, description, values);

        public static Enumeration? CreateValid(
            Guid publicId,
            string name,
            string? description = null,
            IReadOnlyList<EnumValue>? values = null) =>
            string.IsNullOrWhiteSpace(name) ? null
            : !publicId.IsVersion7() ? null
            : new Enumeration(publicId, name, description, values);
    }
}

/// <summary>
/// Represents a single value in an enumeration
/// </summary>
public sealed record EnumValue
{
    internal EnumValue(
        string name,
        string? description = null,
        int? value = null) =>
        (Name, Description, Value) = (name, description, value);

    public string Name { get; }
    public string? Description { get; }
    public int? Value { get; }
}

/// <summary>
/// Extension methods for EnumValue
/// </summary>
public static class EnumValueExtensions
{
    extension(EnumValue)
    {
        public static EnumValue? New(
            string name,
            string? description = null,
            int? value = null) =>
            string.IsNullOrWhiteSpace(name) ? null
            : new EnumValue(name, description, value);
    }
}
