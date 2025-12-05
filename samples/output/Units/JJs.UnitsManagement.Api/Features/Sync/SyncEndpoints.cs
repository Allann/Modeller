using FluentValidation;
using JJs.UnitsManagement.Api.Services;
using JJs.UnitsManagement.Infrastructure;
using JJs.UnitsManagement.Infrastructure.Extensions;
using JJs.UnitsManagement.Sdk;
using JJs.UnitsManagement.Sdk.Common;
using JJs.UnitsManagement.Sdk.Sync;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JJs.UnitsManagement.Api.Features.Sync;

/// <summary>
/// Sync endpoints for the Units Management API
/// </summary>
public static class SyncEndpoints
{
    /// <summary>
    /// Maps Sync endpoints to the route builder
    /// </summary>
    /// <param name="app">The endpoint route builder</param>
    /// <returns>The endpoint route builder for chaining</returns>
    public static IEndpointRouteBuilder MapSyncEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/sync")
            .WithTags("Sync")
            .WithOpenApi()
            .RequireAuthorization();

        group.MapPost("/units", SyncUnits)
            .WithName("SyncUnits")
            .WithSummary("Sync all units from source of truth")
            .WithDescription("Synchronizes all units from the TRUCK_COMBINED source of truth system")
            .Produces<ApiResult<SyncEntityResult>>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status400BadRequest);

        group.MapPost("/all", SyncAll)
            .WithName("SyncAll")
            .WithSummary("Sync all entities from source of truth")
            .WithDescription("Performs a complete synchronization of units from source of truth systems")
            .Produces<ApiResult<SyncResponse>>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status400BadRequest);

        group.MapGet("/status", GetSyncStatus)
            .WithName("GetSyncStatus")
            .WithSummary("Get current sync status")
            .WithDescription("Retrieves the current synchronization status and progress")
            .Produces<ApiResult<SyncProgress>>(StatusCodes.Status200OK);

        // Background sync job endpoints
        group.MapPost("/jobs/start", StartSyncJob)
            .WithName("StartSyncJob")
            .WithSummary("Start a background sync job")
            .WithDescription("Starts a background synchronization job and returns immediately with a job ID")
            .Produces<ApiResult<StartSyncJobResponse>>(StatusCodes.Status200OK)
            .ProducesValidationProblem()
            .Produces(StatusCodes.Status400BadRequest);

        group.MapGet("/jobs/{jobId:guid}/status", GetSyncJobStatus)
            .WithName("GetSyncJobStatus")
            .WithSummary("Get sync job status")
            .WithDescription("Retrieves the status and progress of a specific sync job")
            .Produces<ApiResult<SyncProgress>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/jobs/latest/status", GetLatestSyncStatus)
            .WithName("GetLatestSyncStatus")
            .WithSummary("Get latest sync status")
            .WithDescription("Retrieves the status of the most recent sync job")
            .Produces<ApiResult<SyncProgress>>(StatusCodes.Status200OK);

        group.MapPost("/jobs/{jobId:guid}/cancel", CancelSyncJob)
            .WithName("CancelSyncJob")
            .WithSummary("Cancel a running sync job")
            .WithDescription("Cancels a running synchronization job")
            .Produces<ApiResult<bool>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapGet("/jobs/running", IsSyncRunning)
            .WithName("IsSyncRunning")
            .WithSummary("Check if sync is running")
            .WithDescription("Checks if any synchronization job is currently running")
            .Produces<ApiResult<bool>>(StatusCodes.Status200OK);

        group.MapGet("/jobs/last-sync-date", GetLastSyncDate)
            .WithName("GetLastSyncDate")
            .WithSummary("Get last successful sync date")
            .WithDescription("Retrieves the date of the last successful synchronization")
            .Produces<ApiResult<DateTimeOffset?>>(StatusCodes.Status200OK);

        return app;
    }

    /// <summary>
    /// Synchronizes units from source of truth
    /// </summary>
    /// <param name="syncService">The units sync service</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The sync result</returns>
    private static async Task<IResult> SyncUnits(
        [FromServices] IUnitsSyncService syncService,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new SyncRequest
            {
                CreateNew = true,
                UpdateExisting = true
            };

            var result = await syncService.SyncUnitsAsync(request, cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            else
            {
                return Results.BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            var result = ApiResult<SyncEntityResult>.Failure($"Failed to sync units: {ex.Message}");
            return Results.BadRequest(result);
        }
    }

    /// <summary>
    /// Synchronizes all entities from source of truth
    /// </summary>
    /// <param name="syncService">The units sync service</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The sync response</returns>
    private static async Task<IResult> SyncAll(
        [FromServices] IUnitsSyncService syncService,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var request = new SyncRequest
            {
                CreateNew = true,
                UpdateExisting = true
            };

            var result = await syncService.SyncAllAsync(request, cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            else
            {
                return Results.BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            var result = ApiResult<SyncResponse>.Failure($"Failed to sync all entities: {ex.Message}");
            return Results.BadRequest(result);
        }
    }

    /// <summary>
    /// Gets the current sync status
    /// </summary>
    /// <param name="syncService">The units sync service</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The sync progress</returns>
    private static async Task<IResult> GetSyncStatus(
        [FromServices] IUnitsSyncService syncService,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var result = await syncService.GetSyncStatusAsync(cancellationToken);

            if (result.IsSuccess)
            {
                return Results.Ok(result);
            }
            else
            {
                return Results.BadRequest(result);
            }
        }
        catch (Exception ex)
        {
            var result = ApiResult<SyncProgress>.Failure($"Failed to get sync status: {ex.Message}");
            return Results.BadRequest(result);
        }
    }

    /// <summary>
    /// Starts a background sync job
    /// </summary>
    private static async Task<IResult> StartSyncJob(
        [FromBody] StartSyncJobRequest request,
        ISyncJobService syncJobService,
        IValidator<StartSyncJobRequest> validator,
        CancellationToken cancellationToken)
    {
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var validationApiResult = ApiResult<StartSyncJobResponse>.ValidationFailure(validationResult);
            return Results.BadRequest(validationApiResult);
        }

        var result = await syncJobService.StartSyncJobAsync(request, cancellationToken);
        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
    }

    /// <summary>
    /// Gets the status of a specific sync job
    /// </summary>
    private static async Task<IResult> GetSyncJobStatus(
        [FromRoute] Guid jobId,
        ISyncJobService syncJobService,
        CancellationToken cancellationToken)
    {
        var result = await syncJobService.GetSyncJobStatusAsync(jobId, cancellationToken);
        return result.IsSuccess ? Results.Ok(result) : Results.NotFound(result);
    }

    /// <summary>
    /// Gets the latest sync status
    /// </summary>
    private static async Task<IResult> GetLatestSyncStatus(
        ISyncJobService syncJobService,
        CancellationToken cancellationToken)
    {
        var result = await syncJobService.GetLatestSyncStatusAsync(cancellationToken);
        return Results.Ok(result);
    }

    /// <summary>
    /// Cancels a running sync job
    /// </summary>
    private static async Task<IResult> CancelSyncJob(
        [FromRoute] Guid jobId,
        ISyncJobService syncJobService,
        CancellationToken cancellationToken)
    {
        var result = await syncJobService.CancelSyncJobAsync(jobId, cancellationToken);
        return result.IsSuccess ? Results.Ok(result) : Results.BadRequest(result);
    }

    /// <summary>
    /// Checks if any sync job is currently running
    /// </summary>
    private static async Task<IResult> IsSyncRunning(
        ISyncJobService syncJobService,
        CancellationToken cancellationToken)
    {
        var result = await syncJobService.IsSyncRunningAsync(cancellationToken);
        return Results.Ok(result);
    }

    /// <summary>
    /// Gets the date of the last successful sync
    /// </summary>
    private static async Task<IResult> GetLastSyncDate(
        ISyncJobService syncJobService,
        CancellationToken cancellationToken)
    {
        var latestResult = await syncJobService.GetLatestSyncStatusAsync(cancellationToken);
        if (latestResult.IsSuccess && latestResult.Data != null)
        {
            var lastSyncDate = latestResult.Data.Status == SyncJobStatus.Completed ? latestResult.Data.CompletedAt : null;
            var result = ApiResult<DateTimeOffset?>.Success(lastSyncDate);
            return Results.Ok(result);
        }

        var errorResult = ApiResult<DateTimeOffset?>.Success(null);
        return Results.Ok(errorResult);
    }
}
