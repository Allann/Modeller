using System.Text;
using System.Text.Json;

namespace JJs.UnitsManagement.Sdk.Unit;

/// <summary>
/// API service for unit operations
/// </summary>
public class UnitApiService(HttpClient httpClient)
{
    private readonly HttpClient _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    /// Creates a new unit
    /// </summary>
    /// <param name="request">The create unit request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created unit</returns>
    public async Task<ApiResult<UnitResponse>> CreateAsync(
        CreateUnitRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("units", content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            return JsonSerializer.Deserialize<ApiResult<UnitResponse>>(responseContent, _jsonOptions)
                   ?? ApiResult<UnitResponse>.Failure("Failed to deserialize response");
        }
        catch (Exception ex)
        {
            return ApiResult<UnitResponse>.Failure(ex.Message);
        }
    }

    /// <summary>
    /// Reads all units
    /// </summary>
    /// <param name="request">The read all units request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of units</returns>
    public async Task<ApiResult<UnitListResponse>> ReadAllAsync(
        ReadAllUnitsRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var queryParams = new List<string>();

            if (request.Page > 1)
            {
                queryParams.Add($"pageNumber={request.Page}");
            }

            if (request.PageSize != 10)
            {
                queryParams.Add($"pageSize={request.PageSize}");
            }

            if (request.ActiveOnly.HasValue)
            {
                queryParams.Add($"activeOnly={request.ActiveOnly.Value}");
            }

            if (!string.IsNullOrEmpty(request.SearchTerm))
            {
                queryParams.Add($"searchTerm={Uri.EscapeDataString(request.SearchTerm)}");
            }

            if (request.TruckType.HasValue)
            {
                queryParams.Add($"truckType={request.TruckType.Value}");
            }

            if (!string.IsNullOrEmpty(request.Company))
            {
                queryParams.Add($"company={Uri.EscapeDataString(request.Company)}");
            }

            if (!string.IsNullOrEmpty(request.State))
            {
                queryParams.Add($"state={Uri.EscapeDataString(request.State)}");
            }

            var queryString = queryParams.Count > 0 ? "?" + string.Join("&", queryParams) : "";

            var response = await _httpClient.GetAsync($"units{queryString}", cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            return JsonSerializer.Deserialize<ApiResult<UnitListResponse>>(responseContent, _jsonOptions)
                   ?? ApiResult<UnitListResponse>.Failure("Failed to deserialize response");
        }
        catch (Exception ex)
        {
            return ApiResult<UnitListResponse>.Failure(ex.Message);
        }
    }

    /// <summary>
    /// Reads a single unit
    /// </summary>
    /// <param name="request">The read unit request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The unit</returns>
    public async Task<ApiResult<UnitResponse>> ReadAsync(
        ReadUnitRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync($"units/{request.UnitId}", cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            return JsonSerializer.Deserialize<ApiResult<UnitResponse>>(responseContent, _jsonOptions)
                   ?? ApiResult<UnitResponse>.Failure("Failed to deserialize response");
        }
        catch (Exception ex)
        {
            return ApiResult<UnitResponse>.Failure(ex.Message);
        }
    }

    /// <summary>
    /// Convenience method to get a unit by ID
    /// </summary>
    /// <param name="unitId">The unit ID</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The unit</returns>
    public async Task<ApiResult<UnitResponse>> GetByIdAsync(
        Guid unitId,
        CancellationToken cancellationToken = default)
    {
        return await ReadAsync(new()
            { UnitId = unitId }, cancellationToken);
    }

    /// <summary>
    /// Updates a unit
    /// </summary>
    /// <param name="unitId">The unit ID</param>
    /// <param name="request">The update unit request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated unit</returns>
    public async Task<ApiResult<UnitResponse>> UpdateAsync(
        Guid unitId,
        UpdateUnitRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var json = JsonSerializer.Serialize(request, _jsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PutAsync($"units/{unitId}", content, cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            return JsonSerializer.Deserialize<ApiResult<UnitResponse>>(responseContent, _jsonOptions)
                   ?? ApiResult<UnitResponse>.Failure("Failed to deserialize response");
        }
        catch (Exception ex)
        {
            return ApiResult<UnitResponse>.Failure(ex.Message);
        }
    }

    /// <summary>
    /// Deletes a unit
    /// </summary>
    /// <param name="request">The delete unit request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Success or failure result</returns>
    public async Task<ApiResult<bool>> DeleteAsync(
        DeleteUnitRequest request,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.DeleteAsync($"units/{request.UnitId}", cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            return JsonSerializer.Deserialize<ApiResult<bool>>(responseContent, _jsonOptions)
                   ?? ApiResult<bool>.Failure("Failed to deserialize response");
        }
        catch (Exception ex)
        {
            return ApiResult<bool>.Failure(ex.Message);
        }
    }

    /// <summary>
    /// Gets comprehensive statistics about units for dashboard display
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Units statistics</returns>
    public async Task<ApiResult<UnitsStatisticsResponse>> GetStatisticsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var response = await _httpClient.GetAsync("units/statistics", cancellationToken);
            var responseContent = await response.Content.ReadAsStringAsync(cancellationToken);

            return JsonSerializer.Deserialize<ApiResult<UnitsStatisticsResponse>>(responseContent, _jsonOptions)
                   ?? ApiResult<UnitsStatisticsResponse>.Failure("Failed to deserialize response");
        }
        catch (Exception ex)
        {
            return ApiResult<UnitsStatisticsResponse>.Failure(ex.Message);
        }
    }
}
