# JJs.UnitsManagement.Sdk

A comprehensive SDK for managing physical trucks used in garbage collection and recycling operations in the JJs Branch CRM system.

## Features

- **Unit Management**: Full CRUD operations for units (trucks)
- **Background Sync**: Sync units from JJRMaster TRUCK_COMBINED table
- **Validation**: Built-in FluentValidation for all requests
- **HTTP Client**: Pre-configured HTTP clients for API communication
- **Dependency Injection**: Easy integration with ASP.NET Core DI container

## Installation

Add the UnitsManagement SDK to your project:

```bash
dotnet add package JJs.UnitsManagement.Sdk
```

## Configuration

### ASP.NET Core Integration

```csharp
// Program.cs or Startup.cs
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddUnitsSdk("https://localhost:7002", builder.Configuration);
```

### Manual Configuration

```csharp
services.AddHttpClient<UnitApiService>("UnitsApiService", client =>
{
    client.BaseAddress = new Uri("https://localhost:7002");
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

services.AddScoped<CreateUnitValidator>();
services.AddScoped<ReadUnitValidator>();
services.AddScoped<ReadAllUnitsValidator>();
services.AddScoped<UpdateUnitValidator>();
services.AddScoped<DeleteUnitValidator>();
```

## Usage Examples

### Creating a Unit

```csharp
public class UnitsController : ControllerBase
{
    private readonly UnitApiService _unitService;

    public UnitsController(UnitApiService unitService)
    {
        _unitService = unitService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateUnit(CreateUnitRequest request)
    {
        var result = await _unitService.CreateAsync(request);
        
        if (result.IsSuccess)
        {
            return Ok(result.Data);
        }
        
        return BadRequest(result.ErrorMessage);
    }
}
```

### Reading Units

```csharp
// Get all units with filtering
var request = new ReadAllUnitsRequest
{
    Page = 1,
    PageSize = 20,
    SearchTerm = "truck",
    TruckType = TruckType.Garbage,
    ActiveOnly = true
};

var result = await _unitService.ReadAllAsync(request);

if (result.IsSuccess)
{
    foreach (var unit in result.Data.Units)
    {
        Console.WriteLine($"Unit: {unit.TruckNumber} - {unit.Description}");
    }
}
```

### Getting a Single Unit

```csharp
// Using convenience method
var result = await _unitService.GetByIdAsync(unitId);

// Or using the formal request
var request = new ReadUnitRequest { UnitId = unitId };
var result = await _unitService.ReadAsync(request);
```

### Updating a Unit

```csharp
var request = new UpdateUnitRequest
{
    TruckNumber = "T001",
    Description = "Updated garbage truck",
    TruckType = TruckType.Garbage,
    Active = true
};

var result = await _unitService.UpdateAsync(unitId, request);
```

### Background Sync Operations

```csharp
public class SyncController : ControllerBase
{
    private readonly ISyncApiService _syncService;

    public SyncController(ISyncApiService syncService)
    {
        _syncService = syncService;
    }

    [HttpPost("sync/start")]
    public async Task<IActionResult> StartSync()
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
            return Ok(new { JobId = result.Data.JobId });
        }
        
        return BadRequest(result.ErrorMessage);
    }

    [HttpGet("sync/status/{jobId}")]
    public async Task<IActionResult> GetSyncStatus(Guid jobId)
    {
        var result = await _syncService.GetSyncJobStatusAsync(jobId);
        return Ok(result.Data);
    }
}
```

## Validation

All request models include built-in validation using FluentValidation:

```csharp
var validator = new CreateUnitValidator();
var validationResult = validator.Validate(request);

if (!validationResult.IsValid)
{
    foreach (var error in validationResult.Errors)
    {
        Console.WriteLine($"Validation Error: {error.ErrorMessage}");
    }
}
```

## Error Handling

All API methods return `ApiResult<T>` which provides consistent error handling:

```csharp
var result = await _unitService.CreateAsync(request);

if (result.IsSuccess)
{
    // Success - use result.Data
    var unit = result.Data;
}
else
{
    // Error - check result.ErrorMessage and result.ValidationErrors
    Console.WriteLine($"Error: {result.ErrorMessage}");
    
    if (result.ValidationErrors != null)
    {
        foreach (var error in result.ValidationErrors)
        {
            Console.WriteLine($"Validation: {error}");
        }
    }
}
```

## Project Structure

```
JJs.UnitsManagement.Sdk/
├── Common/
│   └── ApiResult.cs
├── Unit/
│   ├── UnitApiService.cs
│   ├── Create/Update/Read/Delete Requests & Validators
│   ├── UnitResponse.cs
│   ├── UnitListResponse.cs
│   └── TruckType.cs
├── Sync/
│   ├── SyncApiService.cs
│   ├── ISyncApiService.cs
│   ├── SyncModels.cs
│   └── JJRMasterUnit.cs
├── Examples/
│   └── UnitsSdkExamples.cs
├── GlobalUsings.cs
└── ServiceCollectionExtensions.cs
```

## License

This project is part of the JJs Branch CRM system and is proprietary software.
