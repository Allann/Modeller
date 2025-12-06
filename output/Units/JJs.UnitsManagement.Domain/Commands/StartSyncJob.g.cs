namespace JJs.UnitsManagement;

/// <summary>
/// Starts a background sync job to synchronize units from external source
/// </summary>
public sealed record StartSyncJob
{
    internal StartSyncJob(
        Guid publicId,
        string? jobName = null,
        bool? syncUnits = null,
        bool? createNew = null,
        bool? updateExisting = null) =>
        (PublicId, JobName, SyncUnits, CreateNew, UpdateExisting) =
        (publicId, jobName, syncUnits, createNew, updateExisting);

    public Guid PublicId { get; }
    public string? JobName { get; }
    public bool? SyncUnits { get; }
    public bool? CreateNew { get; }
    public bool? UpdateExisting { get; }
}

/// <summary>
/// Extension methods for StartSyncJob
/// </summary>
public static class StartSyncJobExtensions
{
    extension(StartSyncJob)
    {
        public static StartSyncJob? New(
            string? jobName = null,
            bool? syncUnits = null,
            bool? createNew = null,
            bool? updateExisting = null) =>
            StartSyncJob.CreateValid(Guid.CreateVersion7(), jobName, syncUnits, createNew, updateExisting);

        public static StartSyncJob? CreateValid(
            Guid publicId,
            string? jobName = null,
            bool? syncUnits = null,
            bool? createNew = null,
            bool? updateExisting = null) =>
            publicId.Version != 7 ? null
            : new StartSyncJob(publicId, jobName, syncUnits, createNew, updateExisting);
    }
}

