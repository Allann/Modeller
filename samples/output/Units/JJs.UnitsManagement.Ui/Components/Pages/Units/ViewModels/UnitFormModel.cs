using System.ComponentModel.DataAnnotations;
using JJs.UnitsManagement.Sdk.Unit;

namespace JJs.UnitsManagement.Ui.Components.Pages.Units.ViewModels;

/// <summary>
/// Form model for Unit creation and editing
/// </summary>
public class UnitFormModel
{
    /// <summary>
    /// The truck number (business key)
    /// </summary>
    [Required]
    [StringLength(50, ErrorMessage = "Truck number must be 50 characters or less")]
    public string TruckNumber { get; set; } = string.Empty;

    /// <summary>
    /// Vehicle registration number
    /// </summary>
    [StringLength(20, ErrorMessage = "Registration number must be 20 characters or less")]
    public string? RegistrationNumber { get; set; }

    /// <summary>
    /// Description of the unit
    /// </summary>
    [StringLength(200, ErrorMessage = "Description must be 200 characters or less")]
    public string? Description { get; set; }

    /// <summary>
    /// Vehicle make
    /// </summary>
    [StringLength(50, ErrorMessage = "Make must be 50 characters or less")]
    public string? Make { get; set; }

    /// <summary>
    /// Vehicle model
    /// </summary>
    [StringLength(100, ErrorMessage = "Model must be 100 characters or less")]
    public string? Model { get; set; }

    /// <summary>
    /// Type of truck for operational classification
    /// </summary>
    public TruckType? TruckType { get; set; }

    /// <summary>
    /// Euro emission standard type
    /// </summary>
    [StringLength(20, ErrorMessage = "Euro type must be 20 characters or less")]
    public string? EuroType { get; set; }

    /// <summary>
    /// Engine number
    /// </summary>
    [StringLength(50, ErrorMessage = "Engine number must be 50 characters or less")]
    public string? EngineNumber { get; set; }

    /// <summary>
    /// Chassis number
    /// </summary>
    [StringLength(50, ErrorMessage = "Chassis number must be 50 characters or less")]
    public string? ChassisNumber { get; set; }

    /// <summary>
    /// Warranty expiration date
    /// </summary>
    public DateTime? WarrantyDate { get; set; }

    /// <summary>
    /// State or territory where unit operates
    /// </summary>
    [StringLength(10, ErrorMessage = "State must be 10 characters or less")]
    public string? State { get; set; }

    /// <summary>
    /// Company code for organisation linkage
    /// </summary>
    [StringLength(20, ErrorMessage = "Company must be 20 characters or less")]
    public string? Company { get; set; }

    /// <summary>
    /// Department within the company
    /// </summary>
    [StringLength(50, ErrorMessage = "Department must be 50 characters or less")]
    public string? Department { get; set; }

    /// <summary>
    /// Activity or operational area
    /// </summary>
    [StringLength(50, ErrorMessage = "Activity must be 50 characters or less")]
    public string? Activity { get; set; }

    /// <summary>
    /// Country code
    /// </summary>
    [StringLength(5, ErrorMessage = "Country code must be 5 characters or less")]
    public string? CountryCode { get; set; }

    /// <summary>
    /// DCN identifier
    /// </summary>
    [StringLength(50, ErrorMessage = "DCN must be 50 characters or less")]
    public string? DCN { get; set; }

    /// <summary>
    /// Additional information
    /// </summary>
    [StringLength(500, ErrorMessage = "Additional information must be 500 characters or less")]
    public string? Extra { get; set; }

    /// <summary>
    /// Determines if the unit is active or not
    /// </summary>
    public bool Active { get; set; } = true;

    /// <summary>
    /// Creates a new UnitFormModel with default values
    /// </summary>
    public UnitFormModel()
    {
        TruckNumber = string.Empty;
        Active = true;
    }

    /// <summary>
    /// Creates a UnitFormModel from a UnitResponse for editing
    /// </summary>
    /// <param name="unit">The unit response to map from</param>
    /// <returns>A new UnitFormModel</returns>
    public static UnitFormModel FromUnitResponse(UnitResponse unit)
    {
        return new()
        {
            TruckNumber = unit.TruckNumber,
            RegistrationNumber = unit.RegistrationNumber,
            Description = unit.Description,
            Make = unit.Make,
            Model = unit.Model,
            TruckType = unit.TruckType,
            EuroType = unit.EuroType,
            EngineNumber = unit.EngineNumber,
            ChassisNumber = unit.ChassisNumber,
            WarrantyDate = unit.WarrantyDate,
            State = unit.State,
            Company = unit.Company,
            Department = unit.Department,
            Activity = unit.Activity,
            CountryCode = unit.CountryCode,
            DCN = unit.DCN,
            Extra = unit.Extra,
            Active = unit.Active
        };
    }

    /// <summary>
    /// Converts the form model to a CreateUnitRequest
    /// </summary>
    /// <returns>A new CreateUnitRequest</returns>
    public CreateUnitRequest ToCreateRequest()
    {
        return new()
        {
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
            Active = Active
        };
    }

    /// <summary>
    /// Converts the form model to an UpdateUnitRequest
    /// </summary>
    /// <returns>A new UpdateUnitRequest</returns>
    public UpdateUnitRequest ToUpdateRequest()
    {
        return new()
        {
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
            Active = Active
        };
    }
}
