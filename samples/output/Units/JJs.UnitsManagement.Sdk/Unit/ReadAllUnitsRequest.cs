namespace JJs.UnitsManagement.Sdk.Unit;

/// <summary>
/// Request to read all units
/// </summary>
public record ReadAllUnitsRequest
{
    /// <summary>
    /// Page number (1-based)
    /// </summary>
    [Range(1, int.MaxValue, ErrorMessage = "Page number must be greater than 0")]
    public int Page { get; init; } = 1;

    /// <summary>
    /// Number of items per page
    /// </summary>
    [Range(1, 100, ErrorMessage = "Page size must be between 1 and 100")]
    public int PageSize { get; init; } = 10;

    /// <summary>
    /// Filter by active status
    /// </summary>
    public bool? ActiveOnly { get; init; }

    /// <summary>
    /// Search term to filter units (searches truck number, registration, description)
    /// </summary>
    public string? SearchTerm { get; init; }

    /// <summary>
    /// Filter by truck type
    /// </summary>
    public TruckType? TruckType { get; init; }

    /// <summary>
    /// Filter by company
    /// </summary>
    public string? Company { get; init; }

    /// <summary>
    /// Filter by state
    /// </summary>
    public string? State { get; init; }
}
