namespace JJs.UnitsManagement;

/// <summary>
/// Updates an existing unit with new information
/// </summary>
public sealed record UpdateUnit
{
    internal UpdateUnit(
        Guid publicId,
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
        string? dCN = null,
        string? extra = null,
        bool? active = null) =>
        (PublicId, TruckNumber, RegistrationNumber, Description, Make, Model, TruckType, EuroType, EngineNumber, ChassisNumber, WarrantyDate, State, Company, Department, Activity, CountryCode, DCN, Extra, Active) =
        (publicId, truckNumber, registrationNumber, description, make, model, truckType, euroType, engineNumber, chassisNumber, warrantyDate, state, company, department, activity, countryCode, dCN, extra, active);

    public Guid PublicId { get; }
    public string TruckNumber { get; }
    public string? RegistrationNumber { get; }
    public string? Description { get; }
    public string? Make { get; }
    public string? Model { get; }
    public TruckType? TruckType { get; }
    public string? EuroType { get; }
    public string? EngineNumber { get; }
    public string? ChassisNumber { get; }
    public DateTime? WarrantyDate { get; }
    public string? State { get; }
    public string? Company { get; }
    public string? Department { get; }
    public string? Activity { get; }
    public string? CountryCode { get; }
    public string? DCN { get; }
    public string? Extra { get; }
    public bool? Active { get; }
}

/// <summary>
/// Extension methods for UpdateUnit
/// </summary>
public static class UpdateUnitExtensions
{
    extension(UpdateUnit)
    {
        public static UpdateUnit? New(
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
            string? dCN = null,
            string? extra = null,
            bool? active = null) =>
            UpdateUnit.CreateValid(Guid.CreateVersion7(), truckNumber, registrationNumber, description, make, model, truckType, euroType, engineNumber, chassisNumber, warrantyDate, state, company, department, activity, countryCode, dCN, extra, active);

        public static UpdateUnit? CreateValid(
            Guid publicId,
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
            string? dCN = null,
            string? extra = null,
            bool? active = null) =>
            publicId.Version != 7 ? null
            : string.IsNullOrWhiteSpace(truckNumber) ? null
            : new UpdateUnit(publicId, truckNumber, registrationNumber, description, make, model, truckType, euroType, engineNumber, chassisNumber, warrantyDate, state, company, department, activity, countryCode, dCN, extra, active);
    }
}

