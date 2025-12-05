namespace JJs.UnitsManagement.Sdk.Sync;

/// <summary>
/// Request model for sync operations
/// </summary>
public class SyncRequest
{
    /// <summary>
    /// Whether to sync units
    /// </summary>
    [JsonPropertyName("syncUnits")]
    public bool SyncUnits { get; set; } = true;

    /// <summary>
    /// Whether to create new records that don't exist
    /// </summary>
    [JsonPropertyName("createNew")]
    public bool CreateNew { get; set; } = true;

    /// <summary>
    /// Whether to update existing records
    /// </summary>
    [JsonPropertyName("updateExisting")]
    public bool UpdateExisting { get; set; } = true;
}

/// <summary>
/// Response model for sync operations
/// </summary>
public class SyncResponse
{
    /// <summary>
    /// Overall sync success status
    /// </summary>
    [JsonPropertyName("isSuccess")]
    public bool IsSuccess { get; set; }

    /// <summary>
    /// Summary message
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Unit sync results
    /// </summary>
    [JsonPropertyName("unitResults")]
    public SyncEntityResult UnitResults { get; set; } = new();

    /// <summary>
    /// Total sync duration
    /// </summary>
    [JsonPropertyName("duration")]
    public TimeSpan Duration { get; set; }

    /// <summary>
    /// Sync timestamp
    /// </summary>
    [JsonPropertyName("syncedAt")]
    public DateTimeOffset SyncedAt { get; set; } = DateTimeOffset.UtcNow;

    /// <summary>
    /// Any errors that occurred during sync
    /// </summary>
    [JsonPropertyName("errors")]
    public List<string> Errors { get; set; } = [];
}

/// <summary>
/// Sync results for a specific entity type
/// </summary>
public class SyncEntityResult
{
    /// <summary>
    /// Number of records processed
    /// </summary>
    [JsonPropertyName("processed")]
    public int Processed { get; set; }

    /// <summary>
    /// Number of new records created
    /// </summary>
    [JsonPropertyName("created")]
    public int Created { get; set; }

    /// <summary>
    /// Number of existing records updated
    /// </summary>
    [JsonPropertyName("updated")]
    public int Updated { get; set; }

    /// <summary>
    /// Number of records that failed to sync
    /// </summary>
    [JsonPropertyName("failed")]
    public int Failed { get; set; }

    /// <summary>
    /// Number of records skipped
    /// </summary>
    [JsonPropertyName("skipped")]
    public int Skipped { get; set; }

    /// <summary>
    /// Specific errors for this entity type
    /// </summary>
    [JsonPropertyName("errors")]
    public List<string> Errors { get; set; } = [];

    /// <summary>
    /// Whether this entity type sync was successful
    /// </summary>
    [JsonPropertyName("isSuccess")]
    public bool IsSuccess => Failed == 0 && Errors.Count == 0;
}

/// <summary>
/// Progress update model for sync operations
/// </summary>
public class SyncProgress
{
    /// <summary>
    /// Current step being processed
    /// </summary>
    [JsonPropertyName("currentStep")]
    public string CurrentStep { get; set; } = string.Empty;

    /// <summary>
    /// Overall progress percentage (0-100)
    /// </summary>
    [JsonPropertyName("overallProgress")]
    public int OverallProgress { get; set; }

    /// <summary>
    /// Current entity type being processed
    /// </summary>
    [JsonPropertyName("currentEntityType")]
    public string CurrentEntityType { get; set; } = string.Empty;

    /// <summary>
    /// Number of items processed in current step
    /// </summary>
    [JsonPropertyName("itemsProcessed")]
    public int ItemsProcessed { get; set; }

    /// <summary>
    /// Total number of items in current step
    /// </summary>
    [JsonPropertyName("totalItems")]
    public int TotalItems { get; set; }

    /// <summary>
    /// Whether the sync is complete
    /// </summary>
    [JsonPropertyName("isComplete")]
    public bool IsComplete { get; set; }

    /// <summary>
    /// Any status message
    /// </summary>
    [JsonPropertyName("statusMessage")]
    public string StatusMessage { get; set; } = string.Empty;

    /// <summary>
    /// Sync job ID for tracking background operations
    /// </summary>
    [JsonPropertyName("jobId")]
    public Guid? JobId { get; set; }

    /// <summary>
    /// Job status for background operations
    /// </summary>
    [JsonPropertyName("status")]
    public SyncJobStatus Status { get; set; } = SyncJobStatus.NotStarted;

    /// <summary>
    /// Error message if the sync failed
    /// </summary>
    [JsonPropertyName("errorMessage")]
    public string? ErrorMessage { get; set; }

    /// <summary>
    /// When the sync started
    /// </summary>
    [JsonPropertyName("startedAt")]
    public DateTimeOffset? StartedAt { get; set; }

    /// <summary>
    /// When the sync completed
    /// </summary>
    [JsonPropertyName("completedAt")]
    public DateTimeOffset? CompletedAt { get; set; }

    /// <summary>
    /// Final sync results (available when complete)
    /// </summary>
    [JsonPropertyName("results")]
    public SyncResponse? Results { get; set; }
}

/// <summary>
/// Status of a background sync job
/// </summary>
public enum SyncJobStatus
{
    /// <summary>
    /// Job has not started yet
    /// </summary>
    NotStarted = 0,

    /// <summary>
    /// Job is currently running
    /// </summary>
    Running = 1,

    /// <summary>
    /// Job completed successfully
    /// </summary>
    Completed = 2,

    /// <summary>
    /// Job failed with errors
    /// </summary>
    Failed = 3,

    /// <summary>
    /// Job was cancelled
    /// </summary>
    Cancelled = 4
}

/// <summary>
/// Request to start a background sync job
/// </summary>
public class StartSyncJobRequest : SyncRequest
{
    /// <summary>
    /// Optional job identifier for tracking
    /// </summary>
    [JsonPropertyName("jobName")]
    public string? JobName { get; set; }
}

/// <summary>
/// Response when starting a background sync job
/// </summary>
public class StartSyncJobResponse
{
    /// <summary>
    /// Unique identifier for the sync job
    /// </summary>
    [JsonPropertyName("jobId")]
    public Guid JobId { get; set; }

    /// <summary>
    /// Message about the job start
    /// </summary>
    [JsonPropertyName("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// When the job was started
    /// </summary>
    [JsonPropertyName("startedAt")]
    public DateTimeOffset StartedAt { get; set; } = DateTimeOffset.UtcNow;
}

/// <summary>
/// Response model for paginated sync job list
/// </summary>
public class SyncJobListResponse
{
    /// <summary>
    /// List of sync jobs
    /// </summary>
    [JsonPropertyName("jobs")]
    public List<SyncProgress> Jobs { get; set; } = [];

    /// <summary>
    /// Total number of jobs
    /// </summary>
    [JsonPropertyName("totalCount")]
    public int TotalCount { get; set; }

    /// <summary>
    /// Current page index (0-based)
    /// </summary>
    [JsonPropertyName("pageIndex")]
    public int PageIndex { get; set; }

    /// <summary>
    /// Page size
    /// </summary>
    [JsonPropertyName("pageSize")]
    public int PageSize { get; set; }

    /// <summary>
    /// Whether there are more pages
    /// </summary>
    [JsonPropertyName("hasNextPage")]
    public bool HasNextPage => (PageIndex + 1) * PageSize < TotalCount;

    /// <summary>
    /// Whether there are previous pages
    /// </summary>
    [JsonPropertyName("hasPreviousPage")]
    public bool HasPreviousPage => PageIndex > 0;
}
