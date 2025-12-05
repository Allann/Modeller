using System.ComponentModel.DataAnnotations;
using JJs.UnitsManagement.Sdk.Unit;

namespace JJs.UnitsManagement.Infrastructure.Entities;

/// <summary>
/// Infrastructure entity for Unit (physical trucks)
/// </summary>
public class Unit
{
    /// <summary>
    /// The unique identifier for the unit
    /// </summary>
    [Key]
    public Guid UnitId { get; set; }

    /// <summary>
    /// The truck number (business key)
    /// </summary>
    /// <remarks>
    /// Unique identifier from TRUCK_NUMBER field, max 50 characters
    /// </remarks>
    [Required]
    [MaxLength(50)]
    public string TruckNumber { get; set; } = string.Empty;

    /// <summary>
    /// Vehicle registration number
    /// </summary>
    /// <remarks>
    /// From REGISTRATION_NUMBER field, max 20 characters
    /// </remarks>
    [MaxLength(20)]
    public string? RegistrationNumber { get; set; }

    /// <summary>
    /// Description of the unit
    /// </summary>
    /// <remarks>
    /// From DESCRIPTION field, max 200 characters
    /// </remarks>
    [MaxLength(200)]
    public string? Description { get; set; }

    /// <summary>
    /// Vehicle make
    /// </summary>
    /// <remarks>
    /// Extracted from TRUCK_MODEL field, max 50 characters
    /// </remarks>
    [MaxLength(50)]
    public string? Make { get; set; }

    /// <summary>
    /// Vehicle model
    /// </summary>
    /// <remarks>
    /// From TRUCK_MODEL field, max 100 characters
    /// </remarks>
    [MaxLength(100)]
    public string? Model { get; set; }

    /// <summary>
    /// Type of truck for operational classification
    /// </summary>
    /// <remarks>
    /// From TRUCK_TYPE field
    /// </remarks>
    public TruckType? TruckType { get; set; }

    /// <summary>
    /// Euro emission standard type
    /// </summary>
    /// <remarks>
    /// From EURO_TYPE field, max 20 characters
    /// </remarks>
    [MaxLength(20)]
    public string? EuroType { get; set; }

    /// <summary>
    /// Engine number
    /// </summary>
    /// <remarks>
    /// From ENGINE_NUMBER field, max 50 characters
    /// </remarks>
    [MaxLength(50)]
    public string? EngineNumber { get; set; }

    /// <summary>
    /// Chassis number
    /// </summary>
    /// <remarks>
    /// From CHASSIS_NUMBER field, max 50 characters
    /// </remarks>
    [MaxLength(50)]
    public string? ChassisNumber { get; set; }

    /// <summary>
    /// Warranty expiration date
    /// </summary>
    /// <remarks>
    /// From WARRANTY_DATE field
    /// </remarks>
    public DateTime? WarrantyDate { get; set; }

    /// <summary>
    /// State or territory where unit operates
    /// </summary>
    /// <remarks>
    /// From STATE field, max 10 characters
    /// </remarks>
    [MaxLength(10)]
    public string? State { get; set; }

    /// <summary>
    /// Company code for organisation linkage
    /// </summary>
    /// <remarks>
    /// From COMPANY field - links to Organisation, max 20 characters
    /// </remarks>
    [MaxLength(20)]
    public string? Company { get; set; }

    /// <summary>
    /// Department within the company
    /// </summary>
    /// <remarks>
    /// From DEPARTMENT field, max 50 characters
    /// </remarks>
    [MaxLength(50)]
    public string? Department { get; set; }

    /// <summary>
    /// Activity or operational area
    /// </summary>
    /// <remarks>
    /// From ACTIVITY field, max 50 characters
    /// </remarks>
    [MaxLength(50)]
    public string? Activity { get; set; }

    /// <summary>
    /// Country code
    /// </summary>
    /// <remarks>
    /// From COUNTRY_CODE field, max 5 characters
    /// </remarks>
    [MaxLength(5)]
    public string? CountryCode { get; set; }

    /// <summary>
    /// DCN identifier
    /// </summary>
    /// <remarks>
    /// From DCN field, max 50 characters
    /// </remarks>
    [MaxLength(50)]
    public string? DCN { get; set; }

    /// <summary>
    /// Additional information
    /// </summary>
    /// <remarks>
    /// From EXTRA field, max 500 characters
    /// </remarks>
    [MaxLength(500)]
    public string? Extra { get; set; }

    /// <summary>
    /// Determines if the unit is active or not
    /// </summary>
    /// <remarks>
    /// Operational status - default is true
    /// </remarks>
    public bool Active { get; set; } = true;

    /// <summary>
    /// When the unit was created
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// When the unit was last updated
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Who created the unit
    /// </summary>
    [MaxLength(100)]
    public string CreatedBy { get; set; } = "System";

    /// <summary>
    /// Who last updated the unit
    /// </summary>
    [MaxLength(100)]
    public string? UpdatedBy { get; set; }

    /// <summary>
    /// Creates a new Unit entity with GUID v7
    /// </summary>
    /// <param name="truckNumber">The truck number (business key)</param>
    /// <param name="registrationNumber">Vehicle registration number</param>
    /// <param name="description">Description of the unit</param>
    /// <param name="make">Vehicle make</param>
    /// <param name="model">Vehicle model</param>
    /// <param name="truckType">Type of truck</param>
    /// <param name="active">Whether the unit is active</param>
    /// <returns>A new Unit entity</returns>
    public static Unit Create(
        string truckNumber,
        string? registrationNumber = null,
        string? description = null,
        string? make = null,
        string? model = null,
        TruckType? truckType = null,
        bool active = true)
    {
        var now = DateTime.UtcNow;
        return new()
        {
            UnitId = Guid.CreateVersion7(),
            TruckNumber = truckNumber,
            RegistrationNumber = registrationNumber,
            Description = description,
            Make = make,
            Model = model,
            TruckType = truckType,
            Active = active,
            CreatedAt = now,
            CreatedBy = "System"
        };
    }

    /// <summary>
    /// Updates the unit with new information
    /// </summary>
    /// <param name="truckNumber">The truck number</param>
    /// <param name="registrationNumber">Vehicle registration number</param>
    /// <param name="description">Description of the unit</param>
    /// <param name="make">Vehicle make</param>
    /// <param name="model">Vehicle model</param>
    /// <param name="truckType">Type of truck</param>
    /// <param name="euroType">Euro emission standard</param>
    /// <param name="engineNumber">Engine number</param>
    /// <param name="chassisNumber">Chassis number</param>
    /// <param name="warrantyDate">Warranty expiration date</param>
    /// <param name="state">State or territory</param>
    /// <param name="company">Company code</param>
    /// <param name="department">Department</param>
    /// <param name="activity">Activity or operational area</param>
    /// <param name="countryCode">Country code</param>
    /// <param name="dcn">DCN identifier</param>
    /// <param name="extra">Additional information</param>
    /// <param name="active">Whether the unit is active</param>
    /// <param name="updatedBy">Who is updating the unit</param>
    public void Update(
        string truckNumber,
        string? registrationNumber = null,
        string? description = null,
        string? make = null,
        string? model = null,
        TruckType? truckType = null,
        string? euroType = null,
        string? engineNumber = null,
        string? chassisNumber = null,
        DateTime? warrantyDate = null,
        string? state = null,
        string? company = null,
        string? department = null,
        string? activity = null,
        string? countryCode = null,
        string? dcn = null,
        string? extra = null,
        bool active = true,
        string updatedBy = "System")
    {
        TruckNumber = truckNumber;
        RegistrationNumber = registrationNumber;
        Description = description;
        Make = make;
        Model = model;
        TruckType = truckType;
        EuroType = euroType;
        EngineNumber = engineNumber;
        ChassisNumber = chassisNumber;
        WarrantyDate = warrantyDate;
        State = state;
        Company = company;
        Department = department;
        Activity = activity;
        CountryCode = countryCode;
        DCN = dcn;
        Extra = extra;
        Active = active;
        UpdatedAt = DateTime.UtcNow;
        UpdatedBy = updatedBy;
    }

    /// <summary>
    /// Converts the entity to a response model
    /// </summary>
    /// <returns>UnitResponse model</returns>
    public UnitResponse ToResponse()
    {
        return new()
        {
            UnitId = UnitId,
            TruckNumber = TruckNumber,
            RegistrationNumber = RegistrationNumber,
            Description = Description,
            Make = Make,
            Model = Model,
            TruckType = TruckType,
            EuroType = EuroType,
            EngineNumber = EngineNumber,
            ChassisNumber = ChassisNumber,
            WarrantyDate = WarrantyDate,
            State = State,
            Company = Company,
            Department = Department,
            Activity = Activity,
            CountryCode = CountryCode,
            DCN = DCN,
            Extra = Extra,
            Active = Active,
            CreatedAt = CreatedAt,
            UpdatedAt = UpdatedAt
        };
    }
}
