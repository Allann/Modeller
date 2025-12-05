using JJs.UnitsManagement.Infrastructure.Entities;
using JJs.UnitsManagement.Sdk.Sync;

namespace JJs.UnitsManagement.Infrastructure.Extensions;

/// <summary>
/// Extension methods for mapping SyncJob entities to SDK models
/// </summary>
public static class SyncJobMappingExtensions
{
    /// <summary>
    /// Maps a SyncJob entity to SyncProgress
    /// </summary>
    /// <param name="syncJob">The sync job entity</param>
    /// <returns>SyncProgress</returns>
    public static SyncProgress ToSyncProgress(this SyncJob syncJob)
    {
        return new()
        {
            JobId = syncJob.JobId,
            Status = syncJob.Status,
            CurrentStep = syncJob.CurrentStep ?? "Initializing",
            OverallProgress = syncJob.OverallProgress,
            IsComplete = syncJob.Status == SyncJobStatus.Completed || syncJob.Status == SyncJobStatus.Failed || syncJob.Status == SyncJobStatus.Cancelled,
            StatusMessage = syncJob.StatusMessage ?? GetDefaultStatusMessage(syncJob.Status),
            StartedAt = syncJob.StartedAt,
            CompletedAt = syncJob.CompletedAt
        };
    }

    /// <summary>
    /// Gets a default status message for a sync job status
    /// </summary>
    /// <param name="status">The sync job status</param>
    /// <returns>Default status message</returns>
    private static string GetDefaultStatusMessage(SyncJobStatus status)
    {
        return status switch
        {
            SyncJobStatus.NotStarted => "Sync job has not started yet",
            SyncJobStatus.Running => "Sync job is currently running",
            SyncJobStatus.Completed => "Sync job completed successfully",
            SyncJobStatus.Failed => "Sync job failed",
            SyncJobStatus.Cancelled => "Sync job was cancelled",
            _ => "Unknown status"
        };
    }
}
