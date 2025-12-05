using JJs.UnitsManagement.Sdk.Common;
using JJs.UnitsManagement.Sdk.Sync;

namespace JJs.UnitsManagement.Api.Services;

/// <summary>
/// Interface for Units synchronization operations
/// </summary>
public interface IUnitsSyncService
{
    /// <summary>
    /// Synchronizes units from the source of truth system
    /// </summary>
    /// <param name="request">Sync request parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Sync result with statistics</returns>
    Task<ApiResult<SyncEntityResult>> SyncUnitsAsync(SyncRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Synchronizes all entities (currently just units)
    /// </summary>
    /// <param name="request">Sync request parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Sync response with all entity results</returns>
    Task<ApiResult<SyncResponse>> SyncAllAsync(SyncRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the current sync status
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Current sync progress information</returns>
    Task<ApiResult<SyncProgress>> GetSyncStatusAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Performs an incremental sync of units modified since the last sync
    /// </summary>
    /// <param name="request">Sync request parameters</param>
    /// <param name="lastSyncDate">Date of the last successful sync</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Sync result with statistics</returns>
    Task<ApiResult<SyncEntityResult>> SyncUnitsIncrementalAsync(SyncRequest request, DateTime lastSyncDate, CancellationToken cancellationToken = default);
}
