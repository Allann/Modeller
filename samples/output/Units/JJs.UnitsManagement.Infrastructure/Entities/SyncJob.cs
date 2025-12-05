using System.ComponentModel.DataAnnotations;
using JJs.UnitsManagement.Sdk.Sync;

namespace JJs.UnitsManagement.Infrastructure.Entities;

/// <summary>
/// Entity representing a background sync job for units
/// </summary>
public class SyncJob
{
    /// <summary>
    /// Unique identifier for the sync job
    /// </summary>
    [Key]
    public Guid JobId { get; set; }

    /// <summary>
    /// Optional name for the job
    /// </summary>
    [MaxLength(100)]
    public string? JobName { get; set; }

    /// <summary>
    /// Current status of the job
    /// </summary>
    public SyncJobStatus Status { get; set; }

    /// <summary>
    /// Current step being processed
    /// </summary>
    [MaxLength(200)]
    public string CurrentStep { get; set; } = string.Empty;

    /// <summary>
    /// Overall progress percentage (0-100)
    /// </summary>
    public int OverallProgress { get; set; }

    /// <summary>
    /// Current entity type being processed
    /// </summary>
    [MaxLength(50)]
    public string CurrentEntityType { get; set; } = string.Empty;

    /// <summary>
    /// Number of items processed in current step
    /// </summary>
    public int ItemsProcessed { get; set; }

    /// <summary>
    /// Total number of items in current step
    /// </summary>
    public int TotalItems { get; set; }

    /// <summary>
    /// Status message
    /// </summary>
    [MaxLength(500)]
    public string StatusMessage { get; set; } = string.Empty;

    /// <summary>
    /// Error message if the job failed
    /// </summary>
    [MaxLength(2000)]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// When the job was created
    /// </summary>
    public DateTimeOffset CreatedAt { get; set; }

    /// <summary>
    /// When the job started processing
    /// </summary>
    public DateTimeOffset? StartedAt { get; set; }

    /// <summary>
    /// When the job completed
    /// </summary>
    public DateTimeOffset? CompletedAt { get; set; }

    /// <summary>
    /// Sync configuration as JSON
    /// </summary>
    [MaxLength(1000)]
    public string ConfigurationJson { get; set; } = string.Empty;

    /// <summary>
    /// Final sync results as JSON (when completed)
    /// </summary>
    public string? ResultsJson { get; set; }

    /// <summary>
    /// Whether to sync units
    /// </summary>
    public bool SyncUnits { get; set; }

    /// <summary>
    /// Whether to create new records
    /// </summary>
    public bool CreateNew { get; set; }

    /// <summary>
    /// Whether to update existing records
    /// </summary>
    public bool UpdateExisting { get; set; }

    /// <summary>
    /// Who created the sync job
    /// </summary>
    [MaxLength(100)]
    public string CreatedBy { get; set; } = "System";

    /// <summary>
    /// When the sync job was last updated
    /// </summary>
    public DateTimeOffset UpdatedAt { get; set; }

    /// <summary>
    /// Who last updated the sync job
    /// </summary>
    [MaxLength(100)]
    public string UpdatedBy { get; set; } = "System";

    /// <summary>
    /// Creates a new sync job
    /// </summary>
    /// <param name="request">Sync request configuration</param>
    /// <returns>New sync job entity</returns>
    public static SyncJob Create(StartSyncJobRequest request)
    {
        var now = DateTimeOffset.UtcNow;
        return new()
        {
            JobId = Guid.CreateVersion7(),
            JobName = request.JobName,
            Status = SyncJobStatus.NotStarted,
            CurrentStep = "Initializing",
            StatusMessage = "Job created and queued for processing",
            CreatedAt = now,
            CreatedBy = "System",
            UpdatedAt = now,
            UpdatedBy = "System",
            SyncUnits = request.SyncUnits,
            CreateNew = request.CreateNew,
            UpdateExisting = request.UpdateExisting,
            ConfigurationJson = System.Text.Json.JsonSerializer.Serialize(request)
        };
    }

    /// <summary>
    /// Updates the job progress
    /// </summary>
    /// <param name="step">Current step</param>
    /// <param name="progress">Overall progress percentage</param>
    /// <param name="entityType">Current entity type</param>
    /// <param name="itemsProcessed">Items processed</param>
    /// <param name="totalItems">Total items</param>
    /// <param name="message">Status message</param>
    public void UpdateProgress(string step, int progress, string entityType, int itemsProcessed, int totalItems, string message)
    {
        CurrentStep = step;
        OverallProgress = Math.Max(0, Math.Min(100, progress));
        CurrentEntityType = entityType;
        ItemsProcessed = itemsProcessed;
        TotalItems = totalItems;
        StatusMessage = message;
        UpdatedAt = DateTimeOffset.UtcNow;
        UpdatedBy = "System";
    }

    /// <summary>
    /// Marks the job as started
    /// </summary>
    public void Start()
    {
        var now = DateTimeOffset.UtcNow;
        Status = SyncJobStatus.Running;
        StartedAt = now;
        UpdatedAt = now;
        UpdatedBy = "System";
        CurrentStep = "Starting sync operation";
        StatusMessage = "Sync operation has started";
    }

    /// <summary>
    /// Marks the job as completed successfully
    /// </summary>
    /// <param name="results">Final sync results</param>
    public void Complete(SyncResponse results)
    {
        var now = DateTimeOffset.UtcNow;
        Status = SyncJobStatus.Completed;
        CompletedAt = now;
        UpdatedAt = now;
        UpdatedBy = "System";
        OverallProgress = 100;
        CurrentStep = "Completed";
        StatusMessage = results.Message;
        ResultsJson = System.Text.Json.JsonSerializer.Serialize(results);
    }

    /// <summary>
    /// Marks the job as failed
    /// </summary>
    /// <param name="errorMessage">Error message</param>
    public void Fail(string errorMessage)
    {
        var now = DateTimeOffset.UtcNow;
        Status = SyncJobStatus.Failed;
        CompletedAt = now;
        UpdatedAt = now;
        UpdatedBy = "System";
        ErrorMessage = errorMessage;
        StatusMessage = $"Sync failed: {errorMessage}";
    }

    /// <summary>
    /// Marks the job as cancelled
    /// </summary>
    public void Cancel()
    {
        var now = DateTimeOffset.UtcNow;
        Status = SyncJobStatus.Cancelled;
        CompletedAt = now;
        UpdatedAt = now;
        UpdatedBy = "System";
        StatusMessage = "Sync operation was cancelled";
    }

    /// <summary>
    /// Converts to SyncProgress model
    /// </summary>
    /// <returns>SyncProgress representation</returns>
    public SyncProgress ToSyncProgress()
    {
        SyncResponse? results = null;
        if (!string.IsNullOrEmpty(ResultsJson))
        {
            try
            {
                results = System.Text.Json.JsonSerializer.Deserialize<SyncResponse>(ResultsJson);
            }
            catch
            {
                // Ignore deserialization errors
            }
        }

        return new()
        {
            JobId = JobId,
            Status = Status,
            CurrentStep = CurrentStep,
            OverallProgress = OverallProgress,
            CurrentEntityType = CurrentEntityType,
            ItemsProcessed = ItemsProcessed,
            TotalItems = TotalItems,
            IsComplete = Status == SyncJobStatus.Completed || Status == SyncJobStatus.Failed || Status == SyncJobStatus.Cancelled,
            StatusMessage = StatusMessage,
            ErrorMessage = ErrorMessage,
            StartedAt = StartedAt,
            CompletedAt = CompletedAt,
            Results = results
        };
    }
}
