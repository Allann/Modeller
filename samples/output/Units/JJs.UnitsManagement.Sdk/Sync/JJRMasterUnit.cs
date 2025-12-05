namespace JJs.UnitsManagement.Sdk.Sync;

/// <summary>
/// Unit data from JJRMaster TRUCK_COMBINED table
/// </summary>
public class JJRMasterUnit
{
    /// <summary>
    /// The truck number (business key)
    /// </summary>
    /// <remarks>From TRUCK_NUMBER field</remarks>
    [JsonPropertyName("truckNumber")]
    public string TruckNumber { get; set; } = string.Empty;

    /// <summary>
    /// Vehicle registration number
    /// </summary>
    /// <remarks>From REGISTRATION_NUMBER field</remarks>
    [JsonPropertyName("registrationNumber")]
    public string? RegistrationNumber { get; set; }

    /// <summary>
    /// Description of the unit
    /// </summary>
    /// <remarks>From DESCRIPTION field</remarks>
    [JsonPropertyName("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Vehicle model information
    /// </summary>
    /// <remarks>From TRUCK_MODEL field</remarks>
    [JsonPropertyName("truckModel")]
    public string? TruckModel { get; set; }

    /// <summary>
    /// Type of truck for operational classification
    /// </summary>
    /// <remarks>From TRUCK_TYPE field</remarks>
    [JsonPropertyName("truckType")]
    public string? TruckType { get; set; }

    /// <summary>
    /// Euro emission standard type
    /// </summary>
    /// <remarks>From EURO_TYPE field</remarks>
    [JsonPropertyName("euroType")]
    public string? EuroType { get; set; }

    /// <summary>
    /// Engine number
    /// </summary>
    /// <remarks>From ENGINE_NUMBER field</remarks>
    [JsonPropertyName("engineNumber")]
    public string? EngineNumber { get; set; }

    /// <summary>
    /// Chassis number
    /// </summary>
    /// <remarks>From CHASSIS_NUMBER field</remarks>
    [JsonPropertyName("chassisNumber")]
    public string? ChassisNumber { get; set; }

    /// <summary>
    /// Warranty expiration date
    /// </summary>
    /// <remarks>From WARRANTY_DATE field</remarks>
    [JsonPropertyName("warrantyDate")]
    public DateTime? WarrantyDate { get; set; }

    /// <summary>
    /// State or territory where unit operates
    /// </summary>
    /// <remarks>From STATE field</remarks>
    [JsonPropertyName("state")]
    public string? State { get; set; }

    /// <summary>
    /// Company code for organisation linkage
    /// </summary>
    /// <remarks>From COMPANY field - links to Organisation</remarks>
    [JsonPropertyName("company")]
    public string? Company { get; set; }

    /// <summary>
    /// Department within the company
    /// </summary>
    /// <remarks>From DEPARTMENT field</remarks>
    [JsonPropertyName("department")]
    public string? Department { get; set; }

    /// <summary>
    /// Activity or operational area
    /// </summary>
    /// <remarks>From ACTIVITY field</remarks>
    [JsonPropertyName("activity")]
    public string? Activity { get; set; }

    /// <summary>
    /// Country code
    /// </summary>
    /// <remarks>From COUNTRY_CODE field</remarks>
    [JsonPropertyName("countryCode")]
    public string? CountryCode { get; set; }

    /// <summary>
    /// DCN identifier
    /// </summary>
    /// <remarks>From DCN field</remarks>
    [JsonPropertyName("dcn")]
    public string? DCN { get; set; }

    /// <summary>
    /// Additional information
    /// </summary>
    /// <remarks>From EXTRA field</remarks>
    [JsonPropertyName("extra")]
    public string? Extra { get; set; }
}
