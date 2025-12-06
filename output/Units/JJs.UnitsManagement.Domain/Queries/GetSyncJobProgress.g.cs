namespace JJs.UnitsManagement;

/// <summary>
/// Retrieves the current progress of a background sync job
/// </summary>
public sealed record GetSyncJobProgress
{
    internal GetSyncJobProgress(
        Guid publicId,
        Guid jobId) =>
        (PublicId, JobId) =
        (publicId, jobId);

    public Guid PublicId { get; }
    public Guid JobId { get; }
}

/// <summary>
/// Extension methods for GetSyncJobProgress
/// </summary>
public static class GetSyncJobProgressExtensions
{
    extension(GetSyncJobProgress)
    {
        public static GetSyncJobProgress? New(
            Guid jobId) =>
            GetSyncJobProgress.CreateValid(Guid.CreateVersion7(), jobId);

        public static GetSyncJobProgress? CreateValid(
            Guid publicId,
            Guid jobId) =>
            publicId.Version != 7 ? null
            : new GetSyncJobProgress(publicId, jobId);
    }
}

