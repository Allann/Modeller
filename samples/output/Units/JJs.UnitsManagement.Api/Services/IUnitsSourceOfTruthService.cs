namespace JJs.UnitsManagement.Api.Services;

/// <summary>
/// Interface for Units source of truth integration
/// </summary>
public interface IUnitsSourceOfTruthService
{
    /// <summary>
    /// Gets all units from the TRUCK_COMBINED table for bulk synchronization
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of all units from TRUCK_COMBINED</returns>
    Task<IEnumerable<TruckCombinedUnit>> GetAllUnitsFromTruckCombinedAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a specific unit from TRUCK_COMBINED by truck number
    /// </summary>
    /// <param name="truckNumber">The truck number to lookup</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Unit information from TRUCK_COMBINED or null if not found</returns>
    Task<TruckCombinedUnit?> GetUnitFromTruckCombinedAsync(string truckNumber, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets units modified since a specific date for incremental sync
    /// </summary>
    /// <param name="lastSyncDate">The date of the last successful sync</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>List of units modified since the last sync date</returns>
    Task<IEnumerable<TruckCombinedUnit>> GetUnitsModifiedSinceAsync(DateTime lastSyncDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets the total count of units in TRUCK_COMBINED for progress reporting
    /// </summary>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Total count of units</returns>
    Task<int> GetTotalUnitsCountAsync(CancellationToken cancellationToken = default);
}

/// <summary>
/// Represents a unit from the TRUCK_COMBINED source of truth table
/// </summary>
public class TruckCombinedUnit
{
    /// <summary>
    /// Country code (AUS, NZ, USA)
    /// </summary>
    public string Country { get; set; } = string.Empty;

    /// <summary>
    /// Truck number (primary identifier)
    /// </summary>
    public string TruckNumber { get; set; } = string.Empty;

    /// <summary>
    /// Description of the unit
    /// </summary>
    public string? Description { get; set; }

    /// <summary>
    /// Registration number
    /// </summary>
    public string? Registration { get; set; }

    /// <summary>
    /// State/location
    /// </summary>
    public string? State { get; set; }

    /// <summary>
    /// Company name
    /// </summary>
    public string? Company { get; set; }

    /// <summary>
    /// Department
    /// </summary>
    public string? Department { get; set; }

    /// <summary>
    /// Engine number
    /// </summary>
    public string? EngineNumber { get; set; }

    /// <summary>
    /// Chassis number
    /// </summary>
    public string? ChassisNumber { get; set; }

    /// <summary>
    /// Activity information
    /// </summary>
    public string? Activity { get; set; }

    /// <summary>
    /// Truck type (Commercial, Garbage, Recycling)
    /// </summary>
    public string? TruckType { get; set; }

    /// <summary>
    /// Warranty date
    /// </summary>
    public DateTime? WarrantyDate { get; set; }

    /// <summary>
    /// Extra information
    /// </summary>
    public string? Extra { get; set; }

    /// <summary>
    /// Truck model information
    /// </summary>
    public string? TruckModel { get; set; }

    /// <summary>
    /// Euro emission standard type
    /// </summary>
    public string? EuroType { get; set; }
}
