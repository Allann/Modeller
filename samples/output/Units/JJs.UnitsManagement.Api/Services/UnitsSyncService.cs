using JJs.UnitsManagement.Infrastructure;
using JJs.UnitsManagement.Infrastructure.Entities;
using JJs.UnitsManagement.Sdk.Common;
using JJs.UnitsManagement.Sdk.Sync;
using JJs.UnitsManagement.Sdk.Unit;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using JJs.UnitsManagement.Api.Services.Background;

namespace JJs.UnitsManagement.Api.Services;

/// <summary>
/// Service for Units synchronization operations with source of truth systems
/// </summary>
public class UnitsSyncService : IUnitsSyncService
{
    private readonly IUnitsSourceOfTruthService _sourceOfTruthService;
    private readonly UnitsManagementDbContext _context;
    private readonly ILogger<UnitsSyncService> _logger;
    private readonly SyncProgressReporter _progressReporter;

    private const int BatchSize = 1000; // Process units in batches for memory efficiency

    /// <summary>
    /// Initializes a new instance of the UnitsSyncService class
    /// </summary>
    /// <param name="sourceOfTruthService">Source of truth service</param>
    /// <param name="context">Database context</param>
    /// <param name="logger">Logger instance</param>
    /// <param name="progressReporter">Progress reporter for real-time updates</param>
    public UnitsSyncService(
        IUnitsSourceOfTruthService sourceOfTruthService,
        UnitsManagementDbContext context,
        ILogger<UnitsSyncService> logger,
        SyncProgressReporter progressReporter)
    {
        _sourceOfTruthService = sourceOfTruthService ?? throw new ArgumentNullException(nameof(sourceOfTruthService));
        _context = context ?? throw new ArgumentNullException(nameof(context));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _progressReporter = progressReporter ?? throw new ArgumentNullException(nameof(progressReporter));
    }

    /// <inheritdoc />
    public async Task<ApiResult<SyncEntityResult>> SyncUnitsAsync(SyncRequest request, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new SyncEntityResult();

        try
        {
            _logger.LogInformation("Starting units sync with CreateNew={CreateNew}, UpdateExisting={UpdateExisting}",
                request.CreateNew, request.UpdateExisting);

            await _progressReporter.ReportProgressAsync("Starting units sync...", 0, cancellationToken);

            // Get total count for progress reporting
            var totalCount = await _sourceOfTruthService.GetTotalUnitsCountAsync(cancellationToken);
            result.Processed = totalCount;

            if (totalCount == 0)
            {
                _logger.LogWarning("No units found in TRUCK_COMBINED table");
                await _progressReporter.ReportProgressAsync("No units found in source system", 100, cancellationToken);
                return ApiResult<SyncEntityResult>.Success(result);
            }

            await _progressReporter.ReportProgressAsync($"Found {totalCount} units to process", 5, cancellationToken);

            // Get all units from source of truth
            var sourceUnits = await _sourceOfTruthService.GetAllUnitsFromTruckCombinedAsync(cancellationToken);
            var sourceUnitsList = sourceUnits.ToList();

            await _progressReporter.ReportProgressAsync("Retrieved source data, starting sync...", 10, cancellationToken);

            // Process units in batches for memory efficiency
            var processedCount = 0;
            var batches = sourceUnitsList.Chunk(BatchSize);

            foreach (var batch in batches)
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    _logger.LogWarning("Units sync cancelled by user");
                    break;
                }

                var batchResult = await ProcessUnitsBatch(batch, request, cancellationToken);
                
                // Aggregate results
                result.Created += batchResult.Created;
                result.Updated += batchResult.Updated;
                result.Failed += batchResult.Failed;
                result.Skipped += batchResult.Skipped;
                result.Errors.AddRange(batchResult.Errors);

                processedCount += batch.Length;
                var progressPercentage = Math.Min(90, 10 + (processedCount * 80 / totalCount));
                
                await _progressReporter.ReportProgressAsync(
                    $"Processed {processedCount}/{totalCount} units", 
                    progressPercentage, 
                    cancellationToken);

                // Save changes after each batch
                if (batchResult.Created > 0 || batchResult.Updated > 0)
                {
                    try
                    {
                        await _context.SaveChangesAsync(cancellationToken);
                    }
                    catch (Microsoft.EntityFrameworkCore.DbUpdateException ex) when (ex.InnerException?.Message?.Contains("IX_Units_TruckNumber") == true)
                    {
                        // This should be rare since we pre-process duplicates, but could happen with race conditions
                        // between concurrent sync operations or if data changed between our duplicate check and save
                        _logger.LogError("Unexpected duplicate truck number constraint violation during batch save - this indicates a race condition or concurrent sync operation");

                        var errorMsg = $"UNEXPECTED DUPLICATE: Database constraint violation during save - possible concurrent sync operations";
                        result.Errors.Add(errorMsg);
                        result.Failed += batchResult.Created;
                        result.Created -= batchResult.Created;

                        // Don't continue processing if we hit unexpected database constraint issues
                        throw new InvalidOperationException($"Database constraint violation suggests data integrity issues or concurrent operations: {ex.Message}", ex);
                    }
                }
            }

            await _progressReporter.ReportProgressAsync("Finalizing sync...", 95, cancellationToken);

            stopwatch.Stop();
            _logger.LogInformation("Units sync completed in {Duration}ms. Created: {Created}, Updated: {Updated}, Failed: {Failed}, Skipped: {Skipped}",
                stopwatch.ElapsedMilliseconds, result.Created, result.Updated, result.Failed, result.Skipped);

            await _progressReporter.ReportProgressAsync("Units sync completed successfully", 100, cancellationToken);

            return ApiResult<SyncEntityResult>.Success(result);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Units sync failed after {Duration}ms", stopwatch.ElapsedMilliseconds);
            await _progressReporter.ReportProgressAsync($"Units sync failed: {ex.Message}", 0, cancellationToken);
            return ApiResult<SyncEntityResult>.Failure($"Units sync failed: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<ApiResult<SyncResponse>> SyncAllAsync(SyncRequest request, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            _logger.LogInformation("Starting sync all entities");
            await _progressReporter.ReportProgressAsync("Starting complete sync...", 0, cancellationToken);

            // For Units plugin, we only have units to sync
            var unitsResult = await SyncUnitsAsync(request, cancellationToken);

            var response = new SyncResponse
            {
                IsSuccess = unitsResult.IsSuccess,
                Message = unitsResult.IsSuccess ? "All entities synced successfully" : "Sync completed with errors",
                Duration = stopwatch.Elapsed,
                SyncedAt = DateTimeOffset.UtcNow,
                UnitResults = unitsResult.Data ?? new SyncEntityResult(),
                Errors = unitsResult.IsSuccess ? [] : [unitsResult.ErrorMessage ?? "Unknown error"]
            };

            stopwatch.Stop();
            _logger.LogInformation("Sync all completed in {Duration}ms", stopwatch.ElapsedMilliseconds);

            return ApiResult<SyncResponse>.Success(response);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Sync all failed after {Duration}ms", stopwatch.ElapsedMilliseconds);
            await _progressReporter.ReportProgressAsync($"Complete sync failed: {ex.Message}", 0, cancellationToken);
            
            var errorResponse = new SyncResponse
            {
                IsSuccess = false,
                Message = $"Sync all failed: {ex.Message}",
                Duration = stopwatch.Elapsed,
                SyncedAt = DateTimeOffset.UtcNow,
                Errors = [ex.Message]
            };

            return ApiResult<SyncResponse>.Failure(errorResponse.Message);
        }
    }

    /// <inheritdoc />
    public async Task<ApiResult<SyncProgress>> GetSyncStatusAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            // Get the latest sync job from the database
            var latestSyncJob = await _context.SyncJobs
                .OrderByDescending(j => j.StartedAt)
                .FirstOrDefaultAsync(cancellationToken);

            if (latestSyncJob == null)
            {
                var noSyncProgress = new SyncProgress
                {
                    JobId = Guid.Empty,
                    Status = SyncJobStatus.NotStarted,
                    CurrentStep = "No sync jobs found",
                    OverallProgress = 0,
                    IsComplete = true,
                    StatusMessage = "No synchronization has been performed yet",
                    StartedAt = null,
                    CompletedAt = null
                };

                return ApiResult<SyncProgress>.Success(noSyncProgress);
            }

            var progress = latestSyncJob.ToSyncProgress();
            return ApiResult<SyncProgress>.Success(progress);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting sync status");
            return ApiResult<SyncProgress>.Failure($"Error getting sync status: {ex.Message}");
        }
    }

    /// <inheritdoc />
    public async Task<ApiResult<SyncEntityResult>> SyncUnitsIncrementalAsync(SyncRequest request, DateTime lastSyncDate, CancellationToken cancellationToken = default)
    {
        var stopwatch = Stopwatch.StartNew();
        var result = new SyncEntityResult();

        try
        {
            _logger.LogInformation("Starting incremental units sync since {LastSyncDate}", lastSyncDate);
            await _progressReporter.ReportProgressAsync($"Starting incremental sync since {lastSyncDate:yyyy-MM-dd HH:mm}", 0, cancellationToken);

            // Get units modified since last sync
            var modifiedUnits = await _sourceOfTruthService.GetUnitsModifiedSinceAsync(lastSyncDate, cancellationToken);
            var modifiedUnitsList = modifiedUnits.ToList();
            result.Processed = modifiedUnitsList.Count;

            if (!modifiedUnitsList.Any())
            {
                _logger.LogInformation("No units modified since {LastSyncDate}", lastSyncDate);
                await _progressReporter.ReportProgressAsync("No modified units found", 100, cancellationToken);
                return ApiResult<SyncEntityResult>.Success(result);
            }

            await _progressReporter.ReportProgressAsync($"Found {modifiedUnitsList.Count} modified units", 10, cancellationToken);

            // Process modified units
            var batchResult = await ProcessUnitsBatch(modifiedUnitsList, request, cancellationToken);
            
            result.Created = batchResult.Created;
            result.Updated = batchResult.Updated;
            result.Failed = batchResult.Failed;
            result.Skipped = batchResult.Skipped;
            result.Errors = batchResult.Errors;

            // Save changes
            if (result.Created > 0 || result.Updated > 0)
            {
                try
                {
                    await _context.SaveChangesAsync(cancellationToken);
                }
                catch (Microsoft.EntityFrameworkCore.DbUpdateException ex) when (ex.InnerException?.Message?.Contains("IX_Units_TruckNumber") == true)
                {
                    // This should be rare since incremental sync typically processes fewer records
                    _logger.LogError("Unexpected duplicate truck number constraint violation during incremental sync save");

                    var errorMsg = $"UNEXPECTED DUPLICATE: Database constraint violation during incremental sync - possible concurrent operations";
                    result.Errors.Add(errorMsg);
                    result.Failed += result.Created;
                    result.Created = 0;

                    // Don't continue if we hit unexpected database constraint issues
                    throw new InvalidOperationException($"Database constraint violation during incremental sync: {ex.Message}", ex);
                }
            }

            stopwatch.Stop();
            _logger.LogInformation("Incremental units sync completed in {Duration}ms. Created: {Created}, Updated: {Updated}, Failed: {Failed}, Skipped: {Skipped}",
                stopwatch.ElapsedMilliseconds, result.Created, result.Updated, result.Failed, result.Skipped);

            await _progressReporter.ReportProgressAsync("Incremental sync completed successfully", 100, cancellationToken);

            return ApiResult<SyncEntityResult>.Success(result);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "Incremental units sync failed after {Duration}ms", stopwatch.ElapsedMilliseconds);
            await _progressReporter.ReportProgressAsync($"Incremental sync failed: {ex.Message}", 0, cancellationToken);
            return ApiResult<SyncEntityResult>.Failure($"Incremental units sync failed: {ex.Message}");
        }
    }

    /// <summary>
    /// Processes a batch of units for synchronization
    /// </summary>
    /// <param name="sourceUnits">Batch of units from source of truth</param>
    /// <param name="request">Sync request parameters</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>Sync result for the batch</returns>
    private async Task<SyncEntityResult> ProcessUnitsBatch(IEnumerable<TruckCombinedUnit> sourceUnits, SyncRequest request, CancellationToken cancellationToken)
    {
        var result = new SyncEntityResult();
        var sourceUnitsList = sourceUnits.ToList();

        if (!sourceUnitsList.Any())
        {
            return result;
        }

        // First, detect and report duplicates in the source data
        var duplicateGroups = sourceUnitsList
            .GroupBy(u => u.TruckNumber)
            .Where(g => g.Count() > 1)
            .ToList();

        if (duplicateGroups.Any())
        {
            foreach (var duplicateGroup in duplicateGroups)
            {
                var truckNumber = duplicateGroup.Key;
                var count = duplicateGroup.Count();
                var countries = string.Join(", ", duplicateGroup.Select(u => u.Country).Distinct());

                var errorMsg = $"DUPLICATE TRUCK NUMBER DETECTED: {truckNumber} appears {count} times in source data (Countries: {countries})";
                result.Errors.Add(errorMsg);
                result.Failed += count - 1; // Count all but one as failed

                _logger.LogError("Data quality issue: {ErrorMessage}", errorMsg);
            }
        }

        // Remove duplicates from processing, keeping only the first occurrence
        var uniqueSourceUnits = sourceUnitsList
            .GroupBy(u => u.TruckNumber)
            .Select(g => g.First())
            .ToList();

        // Get existing units for this batch
        var truckNumbers = uniqueSourceUnits.Select(u => u.TruckNumber).ToList();
        var existingUnits = await _context.Units
            .Where(u => truckNumbers.Contains(u.TruckNumber))
            .ToListAsync(cancellationToken);

        var existingByTruckNumber = existingUnits.ToDictionary(u => u.TruckNumber, u => u);

        foreach (var sourceUnit in uniqueSourceUnits)
        {
            try
            {
                if (cancellationToken.IsCancellationRequested)
                {
                    break;
                }

                var existingUnit = existingByTruckNumber.TryGetValue(sourceUnit.TruckNumber, out var unit) ? unit : null;

                if (existingUnit == null)
                {
                    // Create new unit
                    if (request.CreateNew)
                    {
                        var newUnit = await CreateUnitFromSource(sourceUnit);
                        _context.Units.Add(newUnit);
                        result.Created++;

                        _logger.LogDebug("Created new unit with truck number {TruckNumber}", sourceUnit.TruckNumber);
                    }
                    else
                    {
                        result.Skipped++;
                        _logger.LogDebug("Skipped creating unit {TruckNumber} (CreateNew=false)", sourceUnit.TruckNumber);
                    }
                }
                else
                {
                    // Update existing unit
                    if (request.UpdateExisting)
                    {
                        var hasChanges = await UpdateUnitFromSource(existingUnit, sourceUnit);
                        if (hasChanges)
                        {
                            result.Updated++;
                            _logger.LogDebug("Updated unit {TruckNumber}", sourceUnit.TruckNumber);
                        }
                        else
                        {
                            result.Skipped++;
                        }
                    }
                    else
                    {
                        result.Skipped++;
                        _logger.LogDebug("Skipped updating unit {TruckNumber} (UpdateExisting=false)", sourceUnit.TruckNumber);
                    }
                }
            }
            catch (Exception ex)
            {
                result.Failed++;
                var errorMsg = $"Failed to sync unit {sourceUnit.TruckNumber}: {ex.Message}";
                result.Errors.Add(errorMsg);
                _logger.LogError(ex, "Error syncing unit {TruckNumber}", sourceUnit.TruckNumber);
            }
        }

        return result;
    }

    /// <summary>
    /// Creates a new Unit entity from source data
    /// </summary>
    /// <param name="sourceUnit">Source unit data</param>
    /// <returns>New Unit entity</returns>
    private async Task<Unit> CreateUnitFromSource(TruckCombinedUnit sourceUnit)
    {
        // Map truck type from source to enum
        var truckType = MapTruckType(sourceUnit.TruckType);

        // Extract make and model from TruckModel field
        var (make, model) = ExtractMakeAndModel(sourceUnit.TruckModel);

        var unit = Unit.Create(
            truckNumber: sourceUnit.TruckNumber,
            registrationNumber: sourceUnit.Registration,
            description: sourceUnit.Description,
            make: make,
            model: model,
            truckType: truckType,
            active: true); // Default to active since source doesn't have this field

        // Set additional properties using the Update method
        unit.Update(
            truckNumber: sourceUnit.TruckNumber,
            registrationNumber: sourceUnit.Registration,
            description: sourceUnit.Description,
            make: make,
            model: model,
            truckType: truckType,
            euroType: sourceUnit.EuroType,
            engineNumber: sourceUnit.EngineNumber,
            chassisNumber: sourceUnit.ChassisNumber,
            warrantyDate: sourceUnit.WarrantyDate,
            state: sourceUnit.State,
            company: sourceUnit.Company,
            department: sourceUnit.Department,
            activity: sourceUnit.Activity,
            countryCode: sourceUnit.Country,
            extra: sourceUnit.Extra,
            active: true,
            updatedBy: "Sync");

        return await Task.FromResult(unit);
    }

    /// <summary>
    /// Updates an existing Unit entity from source data
    /// </summary>
    /// <param name="existingUnit">Existing unit entity</param>
    /// <param name="sourceUnit">Source unit data</param>
    /// <returns>True if changes were made, false otherwise</returns>
    private async Task<bool> UpdateUnitFromSource(Unit existingUnit, TruckCombinedUnit sourceUnit)
    {
        var hasChanges = false;
        var truckType = MapTruckType(sourceUnit.TruckType);
        var (make, model) = ExtractMakeAndModel(sourceUnit.TruckModel);

        // Check for changes and update if necessary
        if (existingUnit.TruckType != truckType ||
            existingUnit.Company != sourceUnit.Company ||
            existingUnit.State != sourceUnit.State ||
            existingUnit.RegistrationNumber != sourceUnit.Registration ||
            existingUnit.Make != make ||
            existingUnit.Model != model ||
            existingUnit.EngineNumber != sourceUnit.EngineNumber ||
            existingUnit.ChassisNumber != sourceUnit.ChassisNumber ||
            existingUnit.Description != sourceUnit.Description ||
            existingUnit.EuroType != sourceUnit.EuroType ||
            existingUnit.WarrantyDate != sourceUnit.WarrantyDate ||
            existingUnit.Department != sourceUnit.Department ||
            existingUnit.Activity != sourceUnit.Activity ||
            existingUnit.CountryCode != sourceUnit.Country ||
            existingUnit.Extra != sourceUnit.Extra)
        {
            existingUnit.Update(
                truckNumber: sourceUnit.TruckNumber,
                registrationNumber: sourceUnit.Registration,
                description: sourceUnit.Description,
                make: make,
                model: model,
                truckType: truckType,
                euroType: sourceUnit.EuroType,
                engineNumber: sourceUnit.EngineNumber,
                chassisNumber: sourceUnit.ChassisNumber,
                warrantyDate: sourceUnit.WarrantyDate,
                state: sourceUnit.State,
                company: sourceUnit.Company,
                department: sourceUnit.Department,
                activity: sourceUnit.Activity,
                countryCode: sourceUnit.Country,
                extra: sourceUnit.Extra,
                active: true, // Keep active status as true for synced units
                updatedBy: "Sync");

            hasChanges = true;
        }

        return await Task.FromResult(hasChanges);
    }

    /// <summary>
    /// Maps source truck type string to TruckType enum
    /// </summary>
    /// <param name="sourceTruckType">Source truck type string</param>
    /// <returns>Mapped TruckType enum value</returns>
    private static TruckType MapTruckType(string? sourceTruckType)
    {
        return sourceTruckType?.ToUpperInvariant() switch
        {
            "COMMERCIAL" => TruckType.Commercial,
            "GARBAGE" => TruckType.Garbage,
            "RECYCLING" => TruckType.Recycling,
            _ => TruckType.Commercial // Default to Commercial if unknown
        };
    }

    /// <summary>
    /// Extracts make and model from the TruckModel field
    /// </summary>
    /// <param name="truckModel">The truck model string from source</param>
    /// <returns>Tuple of (make, model)</returns>
    private static (string? make, string? model) ExtractMakeAndModel(string? truckModel)
    {
        if (string.IsNullOrWhiteSpace(truckModel))
        {
            return (null, null);
        }

        // Try to split on common separators to extract make and model
        var parts = truckModel.Split([' ', '-', '_'], 2, StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length >= 2)
        {
            return (parts[0].Trim(), parts[1].Trim());
        }

        // If we can't split, use the whole string as the model
        return (null, truckModel.Trim());
    }
}
