using JJs.UnitsManagement.Infrastructure.Entities;
using JJs.UnitsManagement.Sdk.Unit;

namespace JJs.UnitsManagement.Infrastructure.Extensions;

/// <summary>
/// Extension methods for mapping Unit entities to SDK models
/// </summary>
public static class UnitMappingExtensions
{
    /// <summary>
    /// Maps a Unit entity to UnitResponse
    /// </summary>
    /// <param name="unit">The unit entity</param>
    /// <returns>UnitResponse</returns>
    public static UnitResponse ToResponse(this Unit unit)
    {
        return new()
        {
            UnitId = unit.UnitId,
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
            Active = unit.Active,
            CreatedAt = unit.CreatedAt,
            UpdatedAt = unit.UpdatedAt
        };
    }

    /// <summary>
    /// Maps a collection of Unit entities to UnitListResponse
    /// </summary>
    /// <param name="units">The unit entities</param>
    /// <param name="totalCount">Total count for pagination</param>
    /// <param name="pageNumber">Current page number</param>
    /// <param name="pageSize">Page size</param>
    /// <returns>UnitListResponse</returns>
    public static UnitListResponse ToListResponse(
        this IEnumerable<Unit> units,
        int totalCount,
        int pageNumber,
        int pageSize)
    {
        return new()
        {
            Units = units.Select(ToResponse).ToList(),
            TotalCount = totalCount,
            PageNumber = pageNumber,
            PageSize = pageSize
        };
    }

    /// <summary>
    /// Creates a Unit entity from CreateUnitRequest
    /// </summary>
    /// <param name="request">The create request</param>
    /// <returns>New Unit entity</returns>
    public static Unit ToEntity(this CreateUnitRequest request)
    {
        return Unit.Create(
            truckNumber: request.TruckNumber,
            registrationNumber: request.RegistrationNumber,
            description: request.Description,
            make: request.Make,
            model: request.Model,
            truckType: request.TruckType,
            active: request.Active);
    }

    /// <summary>
    /// Updates a Unit entity from UpdateUnitRequest
    /// </summary>
    /// <param name="unit">The unit entity to update</param>
    /// <param name="request">The update request</param>
    /// <param name="updatedBy">Who is updating the unit</param>
    public static void UpdateFromRequest(this Unit unit, UpdateUnitRequest request, string updatedBy = "System")
    {
        unit.Update(
            truckNumber: request.TruckNumber,
            registrationNumber: request.RegistrationNumber,
            description: request.Description,
            make: request.Make,
            model: request.Model,
            truckType: request.TruckType,
            euroType: request.EuroType,
            engineNumber: request.EngineNumber,
            chassisNumber: request.ChassisNumber,
            warrantyDate: request.WarrantyDate,
            state: request.State,
            company: request.Company,
            department: request.Department,
            activity: request.Activity,
            countryCode: request.CountryCode,
            dcn: request.DCN,
            extra: request.Extra,
            active: request.Active,
            updatedBy: updatedBy);
    }
}
