namespace JJs.UnitsManagement;

/// <summary>
/// Status values for background sync job operations
/// </summary>
public sealed record SyncJobStatus
{
    internal SyncJobStatus(
        Guid publicId,
        string name,
        string? description = null,
        IReadOnlyList<SyncJobStatusValue>? values = null) =>
        (PublicId, Name, Description, Values) = (publicId, name, description, values ?? []);

    public Guid PublicId { get; }
    public string Name { get; }
    public string? Description { get; }
    public IReadOnlyList<SyncJobStatusValue> Values { get; }
}

/// <summary>
/// Extension methods for SyncJobStatus
/// </summary>
public static class SyncJobStatusExtensions
{
    extension(SyncJobStatus)
    {
        public static SyncJobStatus? New(
            string name,
            string? description = null,
            IReadOnlyList<SyncJobStatusValue>? values = null) =>
            SyncJobStatus.CreateValid(Guid.CreateVersion7(), name, description, values);

        public static SyncJobStatus? CreateValid(
            Guid publicId,
            string name,
            string? description = null,
            IReadOnlyList<SyncJobStatusValue>? values = null) =>
            string.IsNullOrWhiteSpace(name) ? null
            : publicId.Version != 7 ? null
            : new SyncJobStatus(publicId, name, description, values);
    }
}

/// <summary>
/// Represents a single value in the SyncJobStatus enumeration
/// </summary>
public sealed record SyncJobStatusValue
{
    internal SyncJobStatusValue(
        string name,
        string? description = null,
        int? value = null) =>
        (Name, Description, Value) = (name, description, value);

    public string Name { get; }
    public string? Description { get; }
    public int? Value { get; }
}

/// <summary>
/// Extension methods for SyncJobStatusValue
/// </summary>
public static class SyncJobStatusValueExtensions
{
    extension(SyncJobStatusValue)
    {
        public static SyncJobStatusValue? New(
            string name,
            string? description = null,
            int? value = null) =>
            string.IsNullOrWhiteSpace(name) ? null
            : new SyncJobStatusValue(name, description, value);
    }
}

