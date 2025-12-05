namespace JJs.UnitsManagement.Sdk.Unit;

/// <summary>
/// Response containing unit details
/// </summary>
public record UnitResponse
{
    /// <summary>
    /// Unique identifier for the unit
    /// </summary>
    public required Guid UnitId { get; init; }

    /// <summary>
    /// The truck number (business key)
    /// </summary>
    /// <remarks>Unique identifier from TRUCK_NUMBER field</remarks>
    public required string TruckNumber { get; init; }

    /// <summary>
    /// Vehicle registration number
    /// </summary>
    /// <remarks>From REGISTRATION_NUMBER field</remarks>
    public string? RegistrationNumber { get; init; }

    /// <summary>
    /// Description of the unit
    /// </summary>
    /// <remarks>From DESCRIPTION field</remarks>
    public string? Description { get; init; }

    /// <summary>
    /// Vehicle make
    /// </summary>
    /// <remarks>Extracted from TRUCK_MODEL field</remarks>
    public string? Make { get; init; }

    /// <summary>
    /// Vehicle model
    /// </summary>
    /// <remarks>From TRUCK_MODEL field</remarks>
    public string? Model { get; init; }

    /// <summary>
    /// Type of truck for operational classification
    /// </summary>
    /// <remarks>From TRUCK_TYPE field</remarks>
    public TruckType? TruckType { get; init; }

    /// <summary>
    /// Euro emission standard type
    /// </summary>
    /// <remarks>From EURO_TYPE field</remarks>
    public string? EuroType { get; init; }

    /// <summary>
    /// Engine number
    /// </summary>
    /// <remarks>From ENGINE_NUMBER field</remarks>
    public string? EngineNumber { get; init; }

    /// <summary>
    /// Chassis number
    /// </summary>
    /// <remarks>From CHASSIS_NUMBER field</remarks>
    public string? ChassisNumber { get; init; }

    /// <summary>
    /// Warranty expiration date
    /// </summary>
    /// <remarks>From WARRANTY_DATE field</remarks>
    public DateTime? WarrantyDate { get; init; }

    /// <summary>
    /// State or territory where unit operates
    /// </summary>
    /// <remarks>From STATE field</remarks>
    public string? State { get; init; }

    /// <summary>
    /// Company code for organisation linkage
    /// </summary>
    /// <remarks>From COMPANY field - links to Organisation</remarks>
    public string? Company { get; init; }

    /// <summary>
    /// Department within the company
    /// </summary>
    /// <remarks>From DEPARTMENT field</remarks>
    public string? Department { get; init; }

    /// <summary>
    /// Activity or operational area
    /// </summary>
    /// <remarks>From ACTIVITY field</remarks>
    public string? Activity { get; init; }

    /// <summary>
    /// Country code
    /// </summary>
    /// <remarks>From COUNTRY_CODE field</remarks>
    public string? CountryCode { get; init; }

    /// <summary>
    /// DCN identifier
    /// </summary>
    /// <remarks>From DCN field</remarks>
    public string? DCN { get; init; }

    /// <summary>
    /// Additional information
    /// </summary>
    /// <remarks>From EXTRA field</remarks>
    public string? Extra { get; init; }

    /// <summary>
    /// Determines if the unit is active or not
    /// </summary>
    /// <remarks>Operational status - default is true</remarks>
    public required bool Active { get; init; }

    /// <summary>
    /// When the unit was created
    /// </summary>
    public DateTime CreatedAt { get; init; }

    /// <summary>
    /// When the unit was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; init; }
}

/// <summary>
/// Response model for units statistics used in dashboard
/// </summary>
public class UnitsStatisticsResponse
{
    /// <summary>
    /// Total number of units in the system
    /// </summary>
    public int TotalUnits { get; set; }

    /// <summary>
    /// Number of active units (units with truck numbers)
    /// </summary>
    public int ActiveUnits { get; set; }

    /// <summary>
    /// Number of inactive units (units without truck numbers)
    /// </summary>
    public int InactiveUnits { get; set; }

    /// <summary>
    /// Number of commercial truck units
    /// </summary>
    public int CommercialUnits { get; set; }

    /// <summary>
    /// Number of garbage collection truck units
    /// </summary>
    public int GarbageUnits { get; set; }

    /// <summary>
    /// Number of recycling truck units
    /// </summary>
    public int RecyclingUnits { get; set; }

    /// <summary>
    /// Number of units created in the last 30 days
    /// </summary>
    public int RecentUnits { get; set; }
}
