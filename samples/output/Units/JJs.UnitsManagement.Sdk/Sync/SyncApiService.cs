using System.Net.Http.Json;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace JJs.UnitsManagement.Sdk.Sync;

/// <summary>
/// API service for units sync operations
/// </summary>
public class UnitsSyncApiService : IUnitsSyncApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;
    private readonly ILogger<UnitsSyncApiService> _logger;

    /// <summary>
    /// Initializes a new instance of the UnitsSyncApiService class
    /// </summary>
    /// <param name="httpClient">HTTP client for API calls</param>
    /// <param name="logger">Logger instance</param>
    public UnitsSyncApiService(HttpClient httpClient, ILogger<UnitsSyncApiService> logger)
    {
        _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));

        _jsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }

    /// <inheritdoc />
    public async Task<ApiResult<SyncEntityResult>> SyncUnitsAsync(SyncRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting unit sync via API");

            var response = await _httpClient.PostAsJsonAsync("/sync/units", request, _jsonOptions, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResult<SyncEntityResult>>(_jsonOptions, cancellationToken);
                return result ?? ApiResult<SyncEntityResult>.Failure("Failed to deserialize response");
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Unit sync API call failed with status {StatusCode}: {Error}", response.StatusCode, errorContent);

            return ApiResult<SyncEntityResult>.Failure($"API call failed with status {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during unit sync API call");
            return ApiResult<SyncEntityResult>.Failure($"Unit sync failed: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<ApiResult<SyncResponse>> SyncAllAsync(SyncRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting complete sync via API");

            var response = await _httpClient.PostAsJsonAsync("/sync/all", request, _jsonOptions, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResult<SyncResponse>>(_jsonOptions, cancellationToken);
                return result ?? ApiResult<SyncResponse>.Failure("Failed to deserialize response");
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Complete sync API call failed with status {StatusCode}: {Error}", response.StatusCode, errorContent);

            return ApiResult<SyncResponse>.Failure($"API call failed with status {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during complete sync API call");
            return ApiResult<SyncResponse>.Failure($"Complete sync failed: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<ApiResult<SyncProgress>> GetSyncStatusAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting sync status via API");

            var response = await _httpClient.GetAsync("/sync/status", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResult<SyncProgress>>(_jsonOptions, cancellationToken);
                return result ?? ApiResult<SyncProgress>.Failure("Failed to deserialize response");
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Sync status API call failed with status {StatusCode}: {Error}", response.StatusCode, errorContent);

            return ApiResult<SyncProgress>.Failure($"API call failed with status {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during sync status API call");
            return ApiResult<SyncProgress>.Failure($"Get sync status failed: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<ApiResult<StartSyncJobResponse>> StartSyncJobAsync(StartSyncJobRequest request, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Starting background sync job via API");

            var response = await _httpClient.PostAsJsonAsync("/sync/jobs/start", request, _jsonOptions, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResult<StartSyncJobResponse>>(_jsonOptions, cancellationToken);
                return result ?? ApiResult<StartSyncJobResponse>.Failure("Failed to deserialize response");
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Start sync job API call failed with status {StatusCode}: {Error}", response.StatusCode, errorContent);
            return ApiResult<StartSyncJobResponse>.Failure($"API call failed: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error starting sync job");
            return ApiResult<StartSyncJobResponse>.Failure($"Error starting sync job: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<ApiResult<SyncProgress>> GetSyncJobStatusAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting sync job status for {JobId} via API", jobId);

            var response = await _httpClient.GetAsync($"/sync/jobs/{jobId}/status", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResult<SyncProgress>>(_jsonOptions, cancellationToken);
                return result ?? ApiResult<SyncProgress>.Failure("Failed to deserialize response");
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Get sync job status API call failed with status {StatusCode}: {Error}", response.StatusCode, errorContent);
            return ApiResult<SyncProgress>.Failure($"API call failed: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sync job status for {JobId}", jobId);
            return ApiResult<SyncProgress>.Failure($"Error getting sync job status: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<ApiResult<SyncProgress>> GetLatestSyncStatusAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting latest sync status via API");

            var response = await _httpClient.GetAsync("/sync/jobs/latest/status", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResult<SyncProgress>>(_jsonOptions, cancellationToken);
                return result ?? ApiResult<SyncProgress>.Failure("Failed to deserialize response");
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Get latest sync status API call failed with status {StatusCode}: {Error}", response.StatusCode, errorContent);
            return ApiResult<SyncProgress>.Failure($"API call failed: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting latest sync status");
            return ApiResult<SyncProgress>.Failure($"Error getting latest sync status: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<ApiResult<bool>> CancelSyncJobAsync(Guid jobId, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Cancelling sync job {JobId} via API", jobId);

            var response = await _httpClient.PostAsync($"/sync/jobs/{jobId}/cancel", null, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResult<bool>>(_jsonOptions, cancellationToken);
                return result ?? ApiResult<bool>.Failure("Failed to deserialize response");
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Cancel sync job API call failed with status {StatusCode}: {Error}", response.StatusCode, errorContent);
            return ApiResult<bool>.Failure($"API call failed: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error cancelling sync job {JobId}", jobId);
            return ApiResult<bool>.Failure($"Error cancelling sync job: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<ApiResult<bool>> IsSyncRunningAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Checking if sync is running via API");

            var response = await _httpClient.GetAsync("/sync/jobs/running", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResult<bool>>(_jsonOptions, cancellationToken);
                return result ?? ApiResult<bool>.Failure("Failed to deserialize response");
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Is sync running API call failed with status {StatusCode}: {Error}", response.StatusCode, errorContent);
            return ApiResult<bool>.Failure($"API call failed: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error checking if sync is running");
            return ApiResult<bool>.Failure($"Error checking sync status: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<ApiResult<DateTimeOffset?>> GetLastSyncDateAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogDebug("Getting last sync date via API");

            var response = await _httpClient.GetAsync("/sync/jobs/last-sync-date", cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<ApiResult<DateTimeOffset?>>(_jsonOptions, cancellationToken);
                return result ?? ApiResult<DateTimeOffset?>.Failure("Failed to deserialize response");
            }

            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Get last sync date API call failed with status {StatusCode}: {Error}", response.StatusCode, errorContent);
            return ApiResult<DateTimeOffset?>.Failure($"API call failed: {response.StatusCode}");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting last sync date");
            return ApiResult<DateTimeOffset?>.Failure($"Error getting last sync date: {ex.Message}");
        }
    }
}
