namespace JJs.UnitsManagement;

/// <summary>
/// Retrieves a paginated list of sync jobs with optional filtering
/// </summary>
public sealed record ListSyncJobs
{
    internal ListSyncJobs(
        Guid publicId,
        int? pageIndex = null,
        int? pageSize = null,
        SyncJobStatus? status = null) =>
        (PublicId, PageIndex, PageSize, Status) =
        (publicId, pageIndex, pageSize, status);

    public Guid PublicId { get; }
    public int? PageIndex { get; }
    public int? PageSize { get; }
    public SyncJobStatus? Status { get; }
}

/// <summary>
/// Extension methods for ListSyncJobs
/// </summary>
public static class ListSyncJobsExtensions
{
    extension(ListSyncJobs)
    {
        public static ListSyncJobs? New(
            int? pageIndex = null,
            int? pageSize = null,
            SyncJobStatus? status = null) =>
            ListSyncJobs.CreateValid(Guid.CreateVersion7(), pageIndex, pageSize, status);

        public static ListSyncJobs? CreateValid(
            Guid publicId,
            int? pageIndex = null,
            int? pageSize = null,
            SyncJobStatus? status = null) =>
            publicId.Version != 7 ? null
            : new ListSyncJobs(publicId, pageIndex, pageSize, status);
    }
}

