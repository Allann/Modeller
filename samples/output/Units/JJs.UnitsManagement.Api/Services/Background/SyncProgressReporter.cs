using JJs.UnitsManagement.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace JJs.UnitsManagement.Api.Services.Background;

/// <summary>
/// Implementation of sync progress reporter that updates database and can be extended for SignalR
/// </summary>
public class SyncProgressReporter
{
    private readonly UnitsManagementDbContext _context;
    private readonly ILogger<SyncProgressReporter> _logger;
    private Guid? _currentJobId;

    /// <summary>
    /// Initializes a new instance of the SyncProgressReporter class
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="logger">Logger instance</param>
    public SyncProgressReporter(
        UnitsManagementDbContext context,
        ILogger<SyncProgressReporter> logger)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Sets the current job ID for progress reporting
    /// </summary>
    /// <param name="jobId">The sync job ID</param>
    public void SetCurrentJobId(Guid jobId)
    {
        _currentJobId = jobId;
    }

    public async Task ReportProgressAsync(string message, int percentage, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Sync Progress: {Message} ({Percentage}%)", message, percentage);

            if (_currentJobId.HasValue)
            {
                var syncJob = await _context.SyncJobs
                    .FirstOrDefaultAsync(j => j.JobId == _currentJobId.Value, cancellationToken);

                if (syncJob != null)
                {
                    syncJob.UpdateProgress(message, percentage, "Units", 0, 0, message);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }

            // TODO: Add SignalR hub notification here for real-time UI updates
            // await _hubContext.Clients.All.SendAsync("SyncProgress", new { message, percentage }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reporting sync progress: {Message}", message);
        }
    }

    public async Task ReportErrorAsync(string error, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogError("Sync Error: {Error}", error);

            if (_currentJobId.HasValue)
            {
                var syncJob = await _context.SyncJobs
                    .FirstOrDefaultAsync(j => j.JobId == _currentJobId.Value, cancellationToken);

                if (syncJob != null)
                {
                    syncJob.Fail(error);
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }

            // TODO: Add SignalR hub notification here for real-time UI updates
            // await _hubContext.Clients.All.SendAsync("SyncError", new { error }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reporting sync error: {Error}", error);
        }
    }

    public async Task ReportCompletionAsync(bool success, string message, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Sync Completion: {Success} - {Message}", success, message);

            if (_currentJobId.HasValue)
            {
                var syncJob = await _context.SyncJobs
                    .FirstOrDefaultAsync(j => j.JobId == _currentJobId.Value, cancellationToken);

                if (syncJob != null)
                {
                    if (success)
                    {
                        // Create a dummy sync response for completion
                        var syncResponse = new JJs.UnitsManagement.Sdk.Sync.SyncResponse
                        {
                            IsSuccess = true,
                            Message = message,
                            Duration = TimeSpan.Zero,
                            SyncedAt = DateTimeOffset.UtcNow,
                            Errors = []
                        };
                        syncJob.Complete(syncResponse);
                    }
                    else
                    {
                        syncJob.Fail(message);
                    }
                    
                    await _context.SaveChangesAsync(cancellationToken);
                }
            }

            // TODO: Add SignalR hub notification here for real-time UI updates
            // await _hubContext.Clients.All.SendAsync("SyncCompleted", new { success, message }, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error reporting sync completion: {Message}", message);
        }
    }
}
