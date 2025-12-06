namespace JJs.UnitsManagement;

/// <summary>
/// Types of trucks used in garbage collection and recycling operations
/// </summary>
public sealed record TruckType
{
    internal TruckType(
        Guid publicId,
        string name,
        string? description = null,
        IReadOnlyList<TruckTypeValue>? values = null) =>
        (PublicId, Name, Description, Values) = (publicId, name, description, values ?? []);

    public Guid PublicId { get; }
    public string Name { get; }
    public string? Description { get; }
    public IReadOnlyList<TruckTypeValue> Values { get; }
}

/// <summary>
/// Extension methods for TruckType
/// </summary>
public static class TruckTypeExtensions
{
    extension(TruckType)
    {
        public static TruckType? New(
            string name,
            string? description = null,
            IReadOnlyList<TruckTypeValue>? values = null) =>
            TruckType.CreateValid(Guid.CreateVersion7(), name, description, values);

        public static TruckType? CreateValid(
            Guid publicId,
            string name,
            string? description = null,
            IReadOnlyList<TruckTypeValue>? values = null) =>
            string.IsNullOrWhiteSpace(name) ? null
            : publicId.Version != 7 ? null
            : new TruckType(publicId, name, description, values);
    }
}

/// <summary>
/// Represents a single value in the TruckType enumeration
/// </summary>
public sealed record TruckTypeValue
{
    internal TruckTypeValue(
        string name,
        string? description = null,
        int? value = null) =>
        (Name, Description, Value) = (name, description, value);

    public string Name { get; }
    public string? Description { get; }
    public int? Value { get; }
}

/// <summary>
/// Extension methods for TruckTypeValue
/// </summary>
public static class TruckTypeValueExtensions
{
    extension(TruckTypeValue)
    {
        public static TruckTypeValue? New(
            string name,
            string? description = null,
            int? value = null) =>
            string.IsNullOrWhiteSpace(name) ? null
            : new TruckTypeValue(name, description, value);
    }
}

