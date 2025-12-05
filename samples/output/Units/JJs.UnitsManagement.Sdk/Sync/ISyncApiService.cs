namespace JJs.UnitsManagement.Sdk.Sync;

/// <summary>
/// Interface for units sync API operations
/// </summary>
public interface IUnitsSyncApiService
{
    /// <summary>
    /// Synchronizes all units from the source of truth
    /// </summary>
    /// <param name="request">Sync configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Sync results for units</returns>
    Task<ApiResult<SyncEntityResult>> SyncUnitsAsync(SyncRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs a complete synchronization of all units from the source of truth
    /// </summary>
    /// <param name="request">Sync configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Complete sync results</returns>
    Task<ApiResult<SyncResponse>> SyncAllAsync(SyncRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current sync status
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Current sync progress</returns>
    Task<ApiResult<SyncProgress>> GetSyncStatusAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Starts a background sync job
    /// </summary>
    /// <param name="request">Sync job configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Job start response with job ID</returns>
    Task<ApiResult<StartSyncJobResponse>> StartSyncJobAsync(StartSyncJobRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the status and progress of a specific sync job
    /// </summary>
    /// <param name="jobId">Job identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Current sync progress</returns>
    Task<ApiResult<SyncProgress>> GetSyncJobStatusAsync(Guid jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the status of the most recent sync job
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Current sync progress</returns>
    Task<ApiResult<SyncProgress>> GetLatestSyncStatusAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels a running sync job
    /// </summary>
    /// <param name="jobId">Job identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cancellation result</returns>
    Task<ApiResult<bool>> CancelSyncJobAsync(Guid jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a sync operation is currently running
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if sync is running</returns>
    Task<ApiResult<bool>> IsSyncRunningAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the last successful sync date
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Last sync date or null if never synced</returns>
    Task<ApiResult<DateTimeOffset?>> GetLastSyncDateAsync(CancellationToken cancellationToken = default);
}
