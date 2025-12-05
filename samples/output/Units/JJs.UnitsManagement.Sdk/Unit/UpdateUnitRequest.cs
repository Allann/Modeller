namespace JJs.UnitsManagement.Sdk.Unit;

/// <summary>
/// Request to update an existing unit
/// </summary>
public record UpdateUnitRequest
{
    /// <summary>
    /// The truck number (business key)
    /// </summary>
    /// <remarks>Unique identifier from TRUCK_NUMBER field</remarks>
    [Required]
    [StringLength(50, ErrorMessage = "Truck number must be 50 characters or less")]
    public required string TruckNumber { get; init; }

    /// <summary>
    /// Vehicle registration number
    /// </summary>
    /// <remarks>From REGISTRATION_NUMBER field</remarks>
    [StringLength(20, ErrorMessage = "Registration number must be 20 characters or less")]
    public string? RegistrationNumber { get; init; }

    /// <summary>
    /// Description of the unit
    /// </summary>
    /// <remarks>From DESCRIPTION field</remarks>
    [StringLength(200, ErrorMessage = "Description must be 200 characters or less")]
    public string? Description { get; init; }

    /// <summary>
    /// Vehicle make
    /// </summary>
    /// <remarks>Extracted from TRUCK_MODEL field</remarks>
    [StringLength(50, ErrorMessage = "Make must be 50 characters or less")]
    public string? Make { get; init; }

    /// <summary>
    /// Vehicle model
    /// </summary>
    /// <remarks>From TRUCK_MODEL field</remarks>
    [StringLength(100, ErrorMessage = "Model must be 100 characters or less")]
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
    [StringLength(20, ErrorMessage = "Euro type must be 20 characters or less")]
    public string? EuroType { get; init; }

    /// <summary>
    /// Engine number
    /// </summary>
    /// <remarks>From ENGINE_NUMBER field</remarks>
    [StringLength(50, ErrorMessage = "Engine number must be 50 characters or less")]
    public string? EngineNumber { get; init; }

    /// <summary>
    /// Chassis number
    /// </summary>
    /// <remarks>From CHASSIS_NUMBER field</remarks>
    [StringLength(50, ErrorMessage = "Chassis number must be 50 characters or less")]
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
    [StringLength(10, ErrorMessage = "State must be 10 characters or less")]
    public string? State { get; init; }

    /// <summary>
    /// Company code for organisation linkage
    /// </summary>
    /// <remarks>From COMPANY field - links to Organisation</remarks>
    [StringLength(20, ErrorMessage = "Company must be 20 characters or less")]
    public string? Company { get; init; }

    /// <summary>
    /// Department within the company
    /// </summary>
    /// <remarks>From DEPARTMENT field</remarks>
    [StringLength(50, ErrorMessage = "Department must be 50 characters or less")]
    public string? Department { get; init; }

    /// <summary>
    /// Activity or operational area
    /// </summary>
    /// <remarks>From ACTIVITY field</remarks>
    [StringLength(50, ErrorMessage = "Activity must be 50 characters or less")]
    public string? Activity { get; init; }

    /// <summary>
    /// Country code
    /// </summary>
    /// <remarks>From COUNTRY_CODE field</remarks>
    [StringLength(5, ErrorMessage = "Country code must be 5 characters or less")]
    public string? CountryCode { get; init; }

    /// <summary>
    /// DCN identifier
    /// </summary>
    /// <remarks>From DCN field</remarks>
    [StringLength(50, ErrorMessage = "DCN must be 50 characters or less")]
    public string? DCN { get; init; }

    /// <summary>
    /// Additional information
    /// </summary>
    /// <remarks>From EXTRA field</remarks>
    [StringLength(500, ErrorMessage = "Extra information must be 500 characters or less")]
    public string? Extra { get; init; }

    /// <summary>
    /// Determines if the unit is active or not
    /// </summary>
    /// <remarks>Operational status - default is true</remarks>
    public bool Active { get; init; } = true;
}
