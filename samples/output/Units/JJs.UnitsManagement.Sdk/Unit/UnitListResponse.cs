namespace JJs.UnitsManagement.Sdk.Unit;

/// <summary>
/// Response containing a list of units
/// </summary>
public record UnitListResponse
{
    /// <summary>
    /// List of units
    /// </summary>
    public required IReadOnlyList<UnitResponse> Units { get; init; }

    /// <summary>
    /// Total number of units available
    /// </summary>
    public required int TotalCount { get; init; }

    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public required int PageNumber { get; init; }

    /// <summary>
    /// Number of items per page
    /// </summary>
    public required int PageSize { get; init; }

    /// <summary>
    /// Total number of pages available
    /// </summary>
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);

    /// <summary>
    /// Whether there are more pages available
    /// </summary>
    public bool HasNextPage => PageNumber * PageSize < TotalCount;

    /// <summary>
    /// Whether there are previous pages available
    /// </summary>
    public bool HasPreviousPage => PageNumber > 1;
}
