using JJs.UnitsManagement.Sdk.Common;
using JJs.UnitsManagement.Sdk.Sync;

namespace JJs.UnitsManagement.Api.Services;

/// <summary>
/// Interface for background sync job management
/// </summary>
public interface ISyncJobService
{
    /// <summary>
    /// Starts a background sync job
    /// </summary>
    /// <param name="request">Sync job configuration</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Job start response with job ID</returns>
    Task<ApiResult<StartSyncJobResponse>> StartSyncJobAsync(StartSyncJobRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current status and progress of a sync job
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
    /// Gets all sync jobs with optional filtering
    /// </summary>
    /// <param name="pageIndex">Page index (0-based)</param>
    /// <param name="pageSize">Page size</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Paginated list of sync jobs</returns>
    Task<ApiResult<SyncJobListResponse>> GetSyncJobsAsync(int pageIndex = 0, int pageSize = 50, CancellationToken cancellationToken = default);

    /// <summary>
    /// Cancels a running sync job
    /// </summary>
    /// <param name="jobId">Job identifier</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Cancellation result</returns>
    Task<ApiResult<bool>> CancelSyncJobAsync(Guid jobId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if any sync job is currently running
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if a sync is running</returns>
    Task<ApiResult<bool>> IsSyncRunningAsync(CancellationToken cancellationToken = default);
}
