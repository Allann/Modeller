using JJs.UnitsManagement.Sdk.Sync;
using JJs.UnitsManagement.Sdk.Unit;

namespace JJs.UnitsManagement.Sdk.Examples;

/// <summary>
/// Examples demonstrating how to use the Units SDK
/// </summary>
public class UnitsSdkExamples
{
    private readonly UnitApiService _unitService;
    private readonly IUnitsSyncApiService _syncService;

    /// <summary>
    /// Initializes a new instance of the UnitsSdkExamples class
    /// </summary>
    /// <param name="unitService">The unit API service</param>
    /// <param name="syncService">The sync API service</param>
    public UnitsSdkExamples(UnitApiService unitService, IUnitsSyncApiService syncService)
    {
        _unitService = unitService;
        _syncService = syncService;
    }

    /// <summary>
    /// Example: Creating a new unit
    /// </summary>
    public async Task<UnitResponse?> CreateUnitExample()
    {
        var request = new CreateUnitRequest
        {
            TruckNumber = "T001",
            RegistrationNumber = "ABC123",
            Description = "Garbage collection truck",
            Make = "Volvo",
            Model = "FE 280",
            TruckType = TruckType.Garbage,
            EuroType = "Euro 6",
            State = "NSW",
            Company = "JJR",
            Department = "Operations",
            Activity = "Waste Collection",
            CountryCode = "AU",
            Active = true
        };

        var result = await _unitService.CreateAsync(request);

        if (result.IsSuccess)
        {
            Console.WriteLine($"Created unit: {result.Data?.TruckNumber}");
            return result.Data;
        }

        Console.WriteLine($"Failed to create unit: {result.ErrorMessage}");
        return null;
    }

    /// <summary>
    /// Example: Reading all units with filtering
    /// </summary>
    public async Task<List<UnitResponse>> GetUnitsExample()
    {
        var request = new ReadAllUnitsRequest
        {
            Page = 1,
            PageSize = 50,
            SearchTerm = "garbage",
            TruckType = TruckType.Garbage,
            Company = "JJR",
            State = "NSW",
            ActiveOnly = true
        };

        var result = await _unitService.ReadAllAsync(request);

        if (result.IsSuccess)
        {
            Console.WriteLine($"Found {result.Data?.Units.Count} units");
            return result.Data?.Units.ToList() ?? [];
        }

        Console.WriteLine($"Failed to get units: {result.ErrorMessage}");
        return [];
    }

    /// <summary>
    /// Example: Getting a specific unit by ID
    /// </summary>
    public async Task<UnitResponse?> GetUnitByIdExample(Guid unitId)
    {
        var result = await _unitService.GetByIdAsync(unitId);

        if (result.IsSuccess)
        {
            var unit = result.Data;
            Console.WriteLine($"Unit: {unit?.TruckNumber} - {unit?.Description}");
            Console.WriteLine($"Type: {unit?.TruckType}, State: {unit?.State}");
            return unit;
        }

        Console.WriteLine($"Failed to get unit: {result.ErrorMessage}");
        return null;
    }

    /// <summary>
    /// Example: Updating a unit
    /// </summary>
    public async Task<UnitResponse?> UpdateUnitExample(Guid unitId)
    {
        var request = new UpdateUnitRequest
        {
            TruckNumber = "T001-UPDATED",
            Description = "Updated garbage collection truck",
            TruckType = TruckType.Recycling,
            Active = true
        };

        var result = await _unitService.UpdateAsync(unitId, request);

        if (result.IsSuccess)
        {
            Console.WriteLine($"Updated unit: {result.Data?.TruckNumber}");
            return result.Data;
        }

        Console.WriteLine($"Failed to update unit: {result.ErrorMessage}");
        return null;
    }

    /// <summary>
    /// Example: Deleting a unit
    /// </summary>
    public async Task<bool> DeleteUnitExample(Guid unitId)
    {
        var request = new DeleteUnitRequest { UnitId = unitId };
        var result = await _unitService.DeleteAsync(request);

        if (result.IsSuccess)
        {
            Console.WriteLine("Unit deleted successfully");
            return true;
        }

        Console.WriteLine($"Failed to delete unit: {result.ErrorMessage}");
        return false;
    }

    /// <summary>
    /// Example: Starting a background sync job
    /// </summary>
    public async Task<Guid?> StartSyncJobExample()
    {
        var request = new StartSyncJobRequest
        {
            JobName = "Manual Units Sync",
            SyncUnits = true,
            CreateNew = true,
            UpdateExisting = true
        };

        var result = await _syncService.StartSyncJobAsync(request);

        if (result.IsSuccess)
        {
            Console.WriteLine($"Started sync job: {result.Data?.JobId}");
            return result.Data?.JobId;
        }

        Console.WriteLine($"Failed to start sync job: {result.ErrorMessage}");
        return null;
    }

    /// <summary>
    /// Example: Monitoring sync job progress
    /// </summary>
    public async Task MonitorSyncJobExample(Guid jobId)
    {
        while (true)
        {
            var result = await _syncService.GetSyncJobStatusAsync(jobId);

            if (result.IsSuccess && result.Data != null)
            {
                var progress = result.Data;
                Console.WriteLine($"Job {jobId}: {progress.Status} - {progress.OverallProgress}%");
                Console.WriteLine($"Step: {progress.CurrentStep} - {progress.StatusMessage}");

                if (progress.IsComplete)
                {
                    Console.WriteLine("Sync job completed!");
                    if (progress.Results != null)
                    {
                        Console.WriteLine($"Units processed: {progress.Results.UnitResults.Processed}");
                        Console.WriteLine($"Units created: {progress.Results.UnitResults.Created}");
                        Console.WriteLine($"Units updated: {progress.Results.UnitResults.Updated}");
                    }
                    break;
                }

                if (progress.Status == SyncJobStatus.Failed)
                {
                    Console.WriteLine($"Sync job failed: {progress.ErrorMessage}");
                    break;
                }
            }
            else
            {
                Console.WriteLine($"Failed to get sync status: {result.ErrorMessage}");
                break;
            }

            // Wait 5 seconds before checking again
            await Task.Delay(5000);
        }
    }

    /// <summary>
    /// Example: Checking if sync is currently running
    /// </summary>
    public async Task<bool> CheckSyncStatusExample()
    {
        var result = await _syncService.IsSyncRunningAsync();

        if (result.IsSuccess)
        {
            var isRunning = result.Data;
            Console.WriteLine($"Sync is currently running: {isRunning}");
            return isRunning;
        }

        Console.WriteLine($"Failed to check sync status: {result.ErrorMessage}");
        return false;
    }

    /// <summary>
    /// Example: Getting last sync date
    /// </summary>
    public async Task<DateTimeOffset?> GetLastSyncDateExample()
    {
        var result = await _syncService.GetLastSyncDateAsync();

        if (result.IsSuccess)
        {
            var lastSyncDate = result.Data;
            if (lastSyncDate.HasValue)
            {
                Console.WriteLine($"Last sync: {lastSyncDate.Value:yyyy-MM-dd HH:mm:ss}");
            }
            else
            {
                Console.WriteLine("No previous sync found");
            }
            return lastSyncDate;
        }

        Console.WriteLine($"Failed to get last sync date: {result.ErrorMessage}");
        return null;
    }
}
