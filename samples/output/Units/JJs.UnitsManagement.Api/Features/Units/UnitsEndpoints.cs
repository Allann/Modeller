using FluentValidation;
using JJs.UnitsManagement.Infrastructure;
using JJs.UnitsManagement.Infrastructure.Extensions;
using JJs.UnitsManagement.Sdk;
using JJs.UnitsManagement.Sdk.Common;
using JJs.UnitsManagement.Sdk.Unit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JJs.UnitsManagement.Api.Features.Units;

/// <summary>
/// Units endpoints for the Units Management API
/// </summary>
public static class UnitsEndpoints
{
    /// <summary>
    /// Maps Units endpoints to the route builder
    /// </summary>
    /// <param name="app">The endpoint route builder</param>
    /// <returns>The endpoint route builder for chaining</returns>
    public static IEndpointRouteBuilder MapUnitsEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/units")
            .WithTags("Units")
            .WithOpenApi()
            .RequireAuthorization();

        group.MapPost("/", CreateUnit)
            .WithName("CreateUnit")
            .WithSummary("Create new unit")
            .WithDescription("Creates a new unit (physical truck)")
            .Produces<ApiResult<UnitResponse>>(StatusCodes.Status201Created)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status400BadRequest);

        group.MapGet("/{id:guid}", GetUnit)
            .WithName("GetUnit")
            .WithSummary("Get unit by ID")
            .WithDescription("Retrieves a specific unit by its ID")
            .Produces<ApiResult<UnitResponse>>()
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/", GetAllUnits)
            .WithName("GetAllUnits")
            .WithSummary("Get all units")
            .WithDescription("Retrieves all units with optional pagination and filtering")
            .Produces<ApiResult<UnitListResponse>>();

        group.MapPut("/{id:guid}", UpdateUnit)
            .WithName("UpdateUnit")
            .WithSummary("Update unit")
            .WithDescription("Updates an existing unit")
            .Produces<ApiResult<UnitResponse>>()
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id:guid}", DeleteUnit)
            .WithName("DeleteUnit")
            .WithSummary("Delete unit")
            .WithDescription("Deletes a unit by its ID")
            .Produces<ApiResult<bool>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/statistics", GetUnitsStatistics)
            .WithName("GetUnitsStatistics")
            .WithSummary("Get units statistics")
            .WithDescription("Retrieves comprehensive statistics about units for dashboard display")
            .Produces<ApiResult<UnitsStatisticsResponse>>();

        return app;
    }

    /// <summary>
    /// Creates a new unit
    /// </summary>
    /// <param name="request">The create unit request</param>
    /// <param name="context">The database context</param>
    /// <param name="validator">The request validator</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The created unit response</returns>
    private static async Task<IResult> CreateUnit(
        [FromBody] CreateUnitRequest request,
        [FromServices] UnitsManagementDbContext context,
        [FromServices] IValidator<CreateUnitRequest> validator,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        try
        {
            // Check for duplicate truck number
            var existingUnit = await context.Units
                .FirstOrDefaultAsync(u => u.TruckNumber == request.TruckNumber, cancellationToken);

            if (existingUnit != null)
            {
                var duplicateResult = ApiResult<UnitResponse>.Failure("A unit with this truck number already exists");
                return Results.BadRequest(duplicateResult);
            }

            var unit = request.ToEntity();
            context.Units.Add(unit);
            await context.SaveChangesAsync(cancellationToken);

            var response = unit.ToResponse();
            var result = ApiResult<UnitResponse>.Success(response);
            return Results.Created($"/units/{unit.UnitId}", result);
        }
        catch (InvalidOperationException ex)
        {
            var result = ApiResult<UnitResponse>.Failure(ex.Message);
            return Results.BadRequest(result);
        }
        catch (Exception ex)
        {
            var result = ApiResult<UnitResponse>.Failure($"Failed to create unit: {ex.Message}");
            return Results.BadRequest(result);
        }
    }

    /// <summary>
    /// Gets a unit by ID
    /// </summary>
    /// <param name="id">The unit ID</param>
    /// <param name="context">The database context</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The unit response</returns>
    private static async Task<IResult> GetUnit(
        [FromRoute] Guid id,
        [FromServices] UnitsManagementDbContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var unit = await context.Units
                .FirstOrDefaultAsync(u => u.UnitId == id, cancellationToken);

            if (unit == null)
            {
                var notFoundResult = ApiResult<UnitResponse>.Failure("Unit not found");
                return Results.NotFound(notFoundResult);
            }

            var response = unit.ToResponse();
            var result = ApiResult<UnitResponse>.Success(response);
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            var result = ApiResult<UnitResponse>.Failure($"Failed to get unit: {ex.Message}");
            return Results.BadRequest(result);
        }
    }

    /// <summary>
    /// Gets all units with pagination and filtering
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="validator">The request validator</param>
    /// <param name="page">The page number for pagination</param>
    /// <param name="pageSize">The page size for pagination</param>
    /// <param name="searchTerm">Search term for filtering units</param>
    /// <param name="truckType">Filter by truck type</param>
    /// <param name="company">Filter by company</param>
    /// <param name="state">Filter by state</param>
    /// <param name="activeOnly">Filter by active status only</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>A list of units</returns>
    private static async Task<IResult> GetAllUnits(
        [FromServices] UnitsManagementDbContext context,
        [FromServices] IValidator<ReadAllUnitsRequest> validator,
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? searchTerm = null,
        [FromQuery] TruckType? truckType = null,
        [FromQuery] string? company = null,
        [FromQuery] string? state = null,
        [FromQuery] bool? activeOnly = null,
        CancellationToken cancellationToken = default)
    {
        var request = new ReadAllUnitsRequest
        {
            Page = page,
            PageSize = pageSize,
            SearchTerm = searchTerm,
            TruckType = truckType,
            Company = company,
            State = state,
            ActiveOnly = activeOnly
        };

        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        try
        {
            var query = context.Units.AsQueryable();

            // Apply filters
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                var searchLower = request.SearchTerm.ToLower();
                query = query.Where(u =>
                    u.TruckNumber.ToLower().Contains(searchLower) ||
                    (u.RegistrationNumber != null && u.RegistrationNumber.ToLower().Contains(searchLower)) ||
                    (u.Description != null && u.Description.ToLower().Contains(searchLower)));
            }

            if (request.TruckType.HasValue)
            {
                query = query.Where(u => u.TruckType == request.TruckType.Value);
            }

            if (!string.IsNullOrWhiteSpace(request.Company))
            {
                query = query.Where(u => u.Company == request.Company);
            }

            if (!string.IsNullOrWhiteSpace(request.State))
            {
                query = query.Where(u => u.State == request.State);
            }

            if (request.ActiveOnly.HasValue)
            {
                query = query.Where(u => u.Active == request.ActiveOnly.Value);
            }

            // Get total count for pagination
            var totalCount = await query.CountAsync(cancellationToken);

            // Apply pagination and ordering
            var units = await query
                .OrderBy(u => u.TruckNumber)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync(cancellationToken);

            var response = units.ToListResponse(totalCount, request.Page, request.PageSize);
            var result = ApiResult<UnitListResponse>.Success(response);
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            var result = ApiResult<UnitListResponse>.Failure($"Failed to get units: {ex.Message}");
            return Results.BadRequest(result);
        }
    }

    /// <summary>
    /// Updates a unit
    /// </summary>
    /// <param name="id">The unit ID</param>
    /// <param name="request">The update unit request</param>
    /// <param name="context">The database context</param>
    /// <param name="validator">The request validator</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The updated unit response</returns>
    private static async Task<IResult> UpdateUnit(
        [FromRoute] Guid id,
        [FromBody] UpdateUnitRequest request,
        [FromServices] UnitsManagementDbContext context,
        [FromServices] IValidator<UpdateUnitRequest> validator,
        CancellationToken cancellationToken = default)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        try
        {
            var unit = await context.Units
                .FirstOrDefaultAsync(u => u.UnitId == id, cancellationToken);

            if (unit == null)
            {
                var notFoundResult = ApiResult<UnitResponse>.Failure("Unit not found");
                return Results.NotFound(notFoundResult);
            }

            // Check for duplicate truck number (excluding current unit)
            var existingUnit = await context.Units
                .FirstOrDefaultAsync(u => u.TruckNumber == request.TruckNumber && u.UnitId != id, cancellationToken);

            if (existingUnit != null)
            {
                var duplicateResult = ApiResult<UnitResponse>.Failure("A unit with this truck number already exists");
                return Results.BadRequest(duplicateResult);
            }

            unit.UpdateFromRequest(request);
            await context.SaveChangesAsync(cancellationToken);

            var response = unit.ToResponse();
            var result = ApiResult<UnitResponse>.Success(response);
            return Results.Ok(result);
        }
        catch (InvalidOperationException ex)
        {
            var result = ApiResult<UnitResponse>.Failure(ex.Message);
            return Results.BadRequest(result);
        }
        catch (Exception ex)
        {
            var result = ApiResult<UnitResponse>.Failure($"Failed to update unit: {ex.Message}");
            return Results.BadRequest(result);
        }
    }

    /// <summary>
    /// Deletes a unit
    /// </summary>
    /// <param name="id">The unit ID</param>
    /// <param name="context">The database context</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The deletion result</returns>
    private static async Task<IResult> DeleteUnit(
        [FromRoute] Guid id,
        [FromServices] UnitsManagementDbContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var unit = await context.Units
                .FirstOrDefaultAsync(u => u.UnitId == id, cancellationToken);

            if (unit == null)
            {
                var notFoundResult = ApiResult<bool>.Failure("Unit not found");
                return Results.NotFound(notFoundResult);
            }

            context.Units.Remove(unit);
            await context.SaveChangesAsync(cancellationToken);

            var result = ApiResult<bool>.Success(true);
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            var result = ApiResult<bool>.Failure($"Failed to delete unit: {ex.Message}");
            return Results.BadRequest(result);
        }
    }

    /// <summary>
    /// Gets comprehensive statistics about units for dashboard display
    /// </summary>
    /// <param name="context">Database context</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Units statistics</returns>
    private static async Task<IResult> GetUnitsStatistics(
        [FromServices] UnitsManagementDbContext context,
        CancellationToken cancellationToken = default)
    {
        try
        {
            // Use a single efficient query to get all statistics at once
            var thirtyDaysAgo = DateTime.UtcNow.AddDays(-30);

            var statisticsQuery = await context.Units
                .GroupBy(u => 1) // Group all units together
                .Select(g => new
                {
                    TotalUnits = g.Count(),
                    ActiveUnits = g.Count(u => !string.IsNullOrEmpty(u.TruckNumber)),
                    CommercialUnits = g.Count(u => u.TruckType == TruckType.Commercial),
                    GarbageUnits = g.Count(u => u.TruckType == TruckType.Garbage),
                    RecyclingUnits = g.Count(u => u.TruckType == TruckType.Recycling),
                    RecentUnits = g.Count(u => u.CreatedAt >= thirtyDaysAgo)
                })
                .FirstOrDefaultAsync(cancellationToken);

            // Handle case where no units exist
            if (statisticsQuery == null)
            {
                var emptyStatistics = new UnitsStatisticsResponse
                {
                    TotalUnits = 0,
                    ActiveUnits = 0,
                    InactiveUnits = 0,
                    CommercialUnits = 0,
                    GarbageUnits = 0,
                    RecyclingUnits = 0,
                    RecentUnits = 0
                };

                var emptyResult = ApiResult<UnitsStatisticsResponse>.Success(emptyStatistics);
                return Results.Ok(emptyResult);
            }

            var statistics = new UnitsStatisticsResponse
            {
                TotalUnits = statisticsQuery.TotalUnits,
                ActiveUnits = statisticsQuery.ActiveUnits,
                InactiveUnits = statisticsQuery.TotalUnits - statisticsQuery.ActiveUnits,
                CommercialUnits = statisticsQuery.CommercialUnits,
                GarbageUnits = statisticsQuery.GarbageUnits,
                RecyclingUnits = statisticsQuery.RecyclingUnits,
                RecentUnits = statisticsQuery.RecentUnits
            };

            var result = ApiResult<UnitsStatisticsResponse>.Success(statistics);
            return Results.Ok(result);
        }
        catch (Exception ex)
        {
            var result = ApiResult<UnitsStatisticsResponse>.Failure($"Failed to get units statistics: {ex.Message}");
            return Results.BadRequest(result);
        }
    }
}
