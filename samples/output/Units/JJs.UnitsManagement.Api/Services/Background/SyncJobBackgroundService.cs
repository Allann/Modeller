using JJs.UnitsManagement.Infrastructure;
using JJs.UnitsManagement.Sdk.Common;
using JJs.UnitsManagement.Sdk.Sync;
using Microsoft.EntityFrameworkCore;

namespace JJs.UnitsManagement.Api.Services.Background;

/// <summary>
/// Background service for processing Units sync jobs
/// </summary>
public class SyncJobBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SyncJobBackgroundService> _logger;


    /// <summary>
    /// Initializes a new instance of the SyncJobBackgroundService
    /// </summary>
    /// <param name="scopeFactory">Service scope factory</param>
    /// <param name="logger">Logger instance</param>
    public SyncJobBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<SyncJobBackgroundService> logger)
    {
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Executes the background service
    /// </summary>
    /// <param name="stoppingToken">Cancellation token</param>
    /// <returns>Task representing the background execution</returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Units Sync Job Background Service started");

        // Get the SyncJobService from the service provider (don't dispose the scope)
        using var scope = _scopeFactory.CreateScope();
        var syncJobService = scope.ServiceProvider.GetRequiredService<SyncJobService>();
        var jobReader = syncJobService.GetJobQueueReader();

        await foreach (var jobId in jobReader.ReadAllAsync(stoppingToken))
        {
            try
            {
                await ProcessSyncJob(jobId, stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing sync job {JobId}", jobId);
            }
        }

        _logger.LogInformation("Units Sync Job Background Service stopped");
    }



    /// <summary>
    /// Processes a specific sync job
    /// </summary>
    /// <param name="jobId">The sync job ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Task representing the async operation</returns>
    private async Task ProcessSyncJob(Guid jobId, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<UnitsManagementDbContext>();
        var syncService = scope.ServiceProvider.GetRequiredService<IUnitsSyncService>();
        var progressReporter = scope.ServiceProvider.GetRequiredService<SyncProgressReporter>();

        try
        {
            _logger.LogInformation("Processing sync job {JobId}", jobId);

            // Get the sync job
            var syncJob = await context.SyncJobs
                .FirstOrDefaultAsync(j => j.JobId == jobId, cancellationToken);

            if (syncJob == null)
            {
                _logger.LogWarning("Sync job {JobId} not found", jobId);
                return;
            }

            // Check if job is still pending (might have been processed by another instance)
            if (syncJob.Status != SyncJobStatus.NotStarted)
            {
                _logger.LogInformation("Sync job {JobId} is no longer pending (Status: {Status})", jobId, syncJob.Status);
                return;
            }

            // Set up progress reporter
            if (progressReporter is SyncProgressReporter reporter)
            {
                reporter.SetCurrentJobId(jobId);
            }

            // Start the job
            syncJob.Start();
            await context.SaveChangesAsync(cancellationToken);

            // Create sync request from job configuration
            var syncRequest = new SyncRequest
            {
                CreateNew = syncJob.CreateNew,
                UpdateExisting = syncJob.UpdateExisting
            };

            // Create progress reporter
            var progressLogger = scope.ServiceProvider.GetRequiredService<ILogger<SyncProgressReporter>>();
            var syncProgressReporter = new SyncProgressReporter(context, progressLogger);
            syncProgressReporter.SetCurrentJobId(jobId);

            // Execute sync with progress reporting
            var result = await ExecuteSyncWithProgress(syncService, syncRequest, syncProgressReporter, context, jobId, cancellationToken);

            // Update job status based on result
            if (result.IsSuccess && result.Data != null)
            {
                syncJob.Complete(result.Data);
                _logger.LogInformation("Sync job {JobId} completed successfully", jobId);
            }
            else
            {
                var errorMessage = result.ErrorMessage ?? "Unknown error occurred during sync";
                syncJob.Fail(errorMessage);
                _logger.LogError("Sync job {JobId} failed: {Error}", jobId, errorMessage);
            }

            await context.SaveChangesAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Sync job {JobId} was cancelled", jobId);
            
            // Update job status to cancelled
            using var cancelScope = _scopeFactory.CreateScope();
            var cancelContext = cancelScope.ServiceProvider.GetRequiredService<UnitsManagementDbContext>();
            var job = await cancelContext.SyncJobs.FirstOrDefaultAsync(j => j.JobId == jobId);
            if (job != null)
            {
                job.Cancel();
                await cancelContext.SaveChangesAsync();
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing sync job {JobId}", jobId);
            
            // Update job status to failed
            try
            {
                using var errorScope = _scopeFactory.CreateScope();
                var errorContext = errorScope.ServiceProvider.GetRequiredService<UnitsManagementDbContext>();
                var job = await errorContext.SyncJobs.FirstOrDefaultAsync(j => j.JobId == jobId);
                if (job != null)
                {
                    job.Fail($"Background processing error: {ex.Message}");
                    await errorContext.SaveChangesAsync();
                }
            }
            catch (Exception saveEx)
            {
                _logger.LogError(saveEx, "Error updating failed sync job {JobId} status", jobId);
            }
        }
    }

    /// <summary>
    /// Executes sync with progress reporting and cancellation support
    /// </summary>
    /// <param name="syncService">Sync service</param>
    /// <param name="request">Sync request</param>
    /// <param name="progressReporter">Progress reporter</param>
    /// <param name="context">Database context</param>
    /// <param name="jobId">Job ID for cancellation checks</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Sync response</returns>
    private async Task<ApiResult<SyncResponse>> ExecuteSyncWithProgress(
        IUnitsSyncService syncService,
        SyncRequest request,
        SyncProgressReporter progressReporter,
        UnitsManagementDbContext context,
        Guid jobId,
        CancellationToken cancellationToken)
    {
        var response = new SyncResponse
        {
            IsSuccess = true,
            Message = "Sync completed successfully",
            SyncedAt = DateTimeOffset.UtcNow,
            Errors = []
        };

        try
        {
            // Check for cancellation before starting
            if (await IsJobCancelledAsync(context, jobId, cancellationToken))
            {
                throw new OperationCanceledException("Sync job was cancelled before starting");
            }

            await progressReporter.ReportProgressAsync("Starting units synchronization...", 10, cancellationToken);

            // Sync units
            var unitsResult = await syncService.SyncUnitsAsync(request, cancellationToken);
            if (unitsResult.IsSuccess)
            {
                response.UnitResults = unitsResult.Data!;
                await progressReporter.ReportProgressAsync("Units synchronization completed", 100, cancellationToken);
            }
            else
            {
                response.Errors.Add($"Units sync failed: {unitsResult.ErrorMessage}");
                response.UnitResults.Errors.Add(unitsResult.ErrorMessage ?? "Unknown error");
            }

            // Final status
            response.IsSuccess = !response.Errors.Any();
            response.Message = response.IsSuccess ? "All entities synced successfully" : "Sync completed with errors";

            return ApiResult<SyncResponse>.Success(response);
        }
        catch (OperationCanceledException)
        {
            response.IsSuccess = false;
            response.Message = "Sync operation was cancelled";
            response.Errors.Add("Operation was cancelled by user or system");
            return ApiResult<SyncResponse>.Failure(response.Message);
        }
        catch (Exception ex)
        {
            response.IsSuccess = false;
            response.Message = $"Sync failed: {ex.Message}";
            response.Errors.Add(ex.Message);
            return ApiResult<SyncResponse>.Failure(response.Message);
        }
    }

    /// <summary>
    /// Checks if a sync job has been cancelled
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="jobId">Job ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if job is cancelled</returns>
    private async Task<bool> IsJobCancelledAsync(UnitsManagementDbContext context, Guid jobId, CancellationToken cancellationToken)
    {
        var job = await context.SyncJobs
            .FirstOrDefaultAsync(j => j.JobId == jobId, cancellationToken);

        return job?.Status == SyncJobStatus.Cancelled;
    }


}
