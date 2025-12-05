using JJs.UnitsManagement.Infrastructure;
using JJs.UnitsManagement.Infrastructure.Entities;
using JJs.UnitsManagement.Sdk.Common;
using JJs.UnitsManagement.Sdk.Sync;
using Microsoft.EntityFrameworkCore;
using System.Threading.Channels;

namespace JJs.UnitsManagement.Api.Services;

/// <summary>
/// Service for managing background sync jobs
/// </summary>
public class SyncJobService : ISyncJobService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SyncJobService> _logger;
    private readonly Channel<Guid> _jobQueue;
    private readonly ChannelWriter<Guid> _jobWriter;
    private readonly ChannelReader<Guid> _jobReader;

    /// <summary>
    /// Initializes a new instance of the SyncJobService
    /// </summary>
    /// <param name="scopeFactory">Service scope factory for creating scoped services</param>
    /// <param name="logger">Logger instance</param>
    public SyncJobService(
        IServiceScopeFactory scopeFactory,
        ILogger<SyncJobService> logger)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        // Create unbounded channel for job queue
        var options = new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        };
        _jobQueue = Channel.CreateUnbounded<Guid>(options);
        _jobWriter = _jobQueue.Writer;
        _jobReader = _jobQueue.Reader;
    }

    /// <inheritdoc />
    public async Task<ApiResult<StartSyncJobResponse>> StartSyncJobAsync(StartSyncJobRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<UnitsManagementDbContext>();

            // Check if a sync is already running
            var isRunning = await IsSyncRunningInternalAsync(context, cancellationToken);
            if (isRunning)
            {
                return ApiResult<StartSyncJobResponse>.Failure("A sync operation is already running. Please wait for it to complete before starting another.");
            }

            // Create new sync job
            var syncJob = SyncJob.Create(request);
            context.SyncJobs.Add(syncJob);
            await context.SaveChangesAsync(cancellationToken);

            // Queue the job for background processing
            await _jobWriter.WriteAsync(syncJob.JobId, cancellationToken);

            _logger.LogInformation("Started sync job {JobId} with configuration: Units={SyncUnits}, CreateNew={CreateNew}, UpdateExisting={UpdateExisting}",
                syncJob.JobId, request.SyncUnits, request.CreateNew, request.UpdateExisting);

            var response = new StartSyncJobResponse
            {
                JobId = syncJob.JobId,
                Message = "Sync job started successfully and queued for processing",
                StartedAt = syncJob.CreatedAt
            };

            return ApiResult<StartSyncJobResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to start sync job");
            return ApiResult<StartSyncJobResponse>.Failure($"Failed to start sync job: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<ApiResult<SyncProgress>> GetSyncJobStatusAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<UnitsManagementDbContext>();

            var syncJob = await context.SyncJobs
                .FirstOrDefaultAsync(j => j.JobId == jobId, cancellationToken);

            if (syncJob == null)
            {
                return ApiResult<SyncProgress>.Failure($"Sync job {jobId} not found");
            }

            var progress = syncJob.ToSyncProgress();
            return ApiResult<SyncProgress>.Success(progress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get sync job status for {JobId}", jobId);
            return ApiResult<SyncProgress>.Failure($"Failed to get sync job status: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<ApiResult<SyncProgress>> GetLatestSyncStatusAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<UnitsManagementDbContext>();

            var latestJob = await context.SyncJobs
                .OrderByDescending(j => j.CreatedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (latestJob == null)
            {
                var noSyncProgress = new SyncProgress
                {
                    JobId = Guid.Empty,
                    Status = SyncJobStatus.NotStarted,
                    CurrentStep = "No sync jobs found",
                    OverallProgress = 0,
                    IsComplete = true,
                    StatusMessage = "No synchronization has been performed yet",
                    StartedAt = null,
                    CompletedAt = null
                };

                return ApiResult<SyncProgress>.Success(noSyncProgress);
            }

            var progress = latestJob.ToSyncProgress();
            return ApiResult<SyncProgress>.Success(progress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get latest sync status");
            return ApiResult<SyncProgress>.Failure($"Failed to get latest sync status: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<ApiResult<SyncJobListResponse>> GetSyncJobsAsync(int pageIndex = 0, int pageSize = 50, CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<UnitsManagementDbContext>();

            var totalCount = await context.SyncJobs.CountAsync(cancellationToken);

            var jobs = await context.SyncJobs
                .OrderByDescending(j => j.CreatedAt)
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .Select(j => j.ToSyncProgress())
                .ToListAsync(cancellationToken);

            var response = new SyncJobListResponse
            {
                Jobs = jobs,
                TotalCount = totalCount,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            return ApiResult<SyncJobListResponse>.Success(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get sync jobs");
            return ApiResult<SyncJobListResponse>.Failure($"Failed to get sync jobs: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<ApiResult<bool>> CancelSyncJobAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<UnitsManagementDbContext>();

            var syncJob = await context.SyncJobs
                .FirstOrDefaultAsync(j => j.JobId == jobId, cancellationToken);

            if (syncJob == null)
            {
                return ApiResult<bool>.Failure($"Sync job {jobId} not found");
            }

            if (syncJob.Status != SyncJobStatus.Running && syncJob.Status != SyncJobStatus.NotStarted)
            {
                return ApiResult<bool>.Failure($"Cannot cancel sync job {jobId} with status {syncJob.Status}");
            }

            syncJob.Cancel();
            await context.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("Cancelled sync job {JobId}", jobId);
            return ApiResult<bool>.Success(true);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to cancel sync job {JobId}", jobId);
            return ApiResult<bool>.Failure($"Failed to cancel sync job: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<ApiResult<bool>> IsSyncRunningAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<UnitsManagementDbContext>();

            var isRunning = await IsSyncRunningInternalAsync(context, cancellationToken);
            return ApiResult<bool>.Success(isRunning);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check if sync is running");
            return ApiResult<bool>.Failure($"Failed to check sync status: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets the job queue reader for background processing
    /// </summary>
    /// <returns>Channel reader for job queue</returns>
    public ChannelReader<Guid> GetJobQueueReader() => _jobReader;

    /// <summary>
    /// Internal method to check if sync is running
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if sync is running</returns>
    private async Task<bool> IsSyncRunningInternalAsync(UnitsManagementDbContext context, CancellationToken cancellationToken)
    {
        return await context.SyncJobs
            .AnyAsync(j => j.Status == SyncJobStatus.Running || j.Status == SyncJobStatus.NotStarted, cancellationToken);
    }
}
