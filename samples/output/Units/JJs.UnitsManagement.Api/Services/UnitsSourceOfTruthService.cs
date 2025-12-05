using Dapper;

using Microsoft.Data.SqlClient;

namespace JJs.UnitsManagement.Api.Services;

/// <summary>
/// Service for Units source of truth integration with TRUCK_COMBINED table
/// </summary>
public class UnitsSourceOfTruthService : IUnitsSourceOfTruthService
{
    private readonly ILogger<UnitsSourceOfTruthService> _logger;
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;
    private const string JJRMasterConnectionString = "Data Source=devsql-01;Initial Catalog=JJRMaster;Integrated Security=True;TrustServerCertificate=True;";

    private const string _sql = """
                                SELECT 'AUS' as Country,
                                   TRUCK_NUMBER as TruckNumber,
                                   [DESCRIPTION],
                                   REGISTRATION_NUMBER as Registration,
                                   [STATE],
                                   COMPANY,
                                   DEPARTMENT,
                                   ENGINE_NUMBER as EngineNumber,
                                   CHASSIS_NUMBER as ChassisNumber,
                                   ACTIVITY,
                                   TRUCK_TYPE as TruckType,
                                   CASE
                                       WHEN WARRANTY_DATE IS NULL OR LTRIM(RTRIM(WARRANTY_DATE)) = ''
                                       THEN NULL
                                       ELSE TRY_CAST(WARRANTY_DATE AS DATETIME)
                                   END as WarrantyDate,
                                   EXTRA,
                                   TRUCK_MODEL as TruckModel,
                                   EURO_TYPE as EuroType
                                FROM TRUCK_AUS with (nolock)
                                UNION
                                SELECT 'NZ' as Country, TRUCK_NUMBER as TruckNumber,
                                    [DESCRIPTION],
                                    REGISTRATION_NUMBER as Registration,
                                    [STATE],
                                    COMPANY,
                                    DEPARTMENT,
                                    ENGINE_NUMBER as EngineNumber,
                                    CHASSIS_NUMBER as ChassisNumber,
                                    ACTIVITY,
                                    TRUCK_TYPE as TruckType,
                                    CASE
                                        WHEN WARRANTY_DATE IS NULL OR LTRIM(RTRIM(WARRANTY_DATE)) = ''
                                        THEN NULL
                                        ELSE TRY_CAST(WARRANTY_DATE AS DATETIME)
                                    END as WarrantyDate,
                                    EXTRA,
                                    TRUCK_MODEL as TruckModel,
                                    EURO_TYPE as EuroType
                                FROM DBO.TRUCK_NZ with (nolock)
                                UNION
                                SELECT 'USA' as Country, TRUCK_NUMBER as TruckNumber,
                                    [DESCRIPTION],
                                    REGISTRATION_NUMBER as Registration,
                                    [STATE],
                                    COMPANY,
                                    DEPARTMENT,
                                    ENGINE_NUMBER as EngineNumber,
                                    CHASSIS_NUMBER as ChassisNumber,
                                    ACTIVITY,
                                    TRUCK_TYPE as TruckType,
                                    CASE
                                        WHEN WARRANTY_DATE IS NULL OR LTRIM(RTRIM(WARRANTY_DATE)) = ''
                                        THEN NULL
                                        ELSE TRY_CAST(WARRANTY_DATE AS DATETIME)
                                    END as WarrantyDate,
                                    EXTRA,
                                    TRUCK_MODEL as TruckModel,
                                    EURO_TYPE as EuroType
                                FROM DBO.TRUCK_USA with (nolock)
                                """;
    
    /// <summary>
    /// Initializes a new instance of the UnitsSourceOfTruthService class
    /// </summary>
    /// <param name="logger">The logger instance</param>
    /// <param name="configuration">Configuration instance</param>
    public UnitsSourceOfTruthService(
        ILogger<UnitsSourceOfTruthService> logger,
        IConfiguration configuration)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

        // Get connection string from configuration
        _connectionString = _configuration.GetConnectionString("JJRMaster") ?? JJRMasterConnectionString;
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TruckCombinedUnit>> GetAllUnitsFromTruckCombinedAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            const string sql = _sql + """
                                      WHERE TRUCK_NUMBER IS NOT NULL AND TRUCK_NUMBER != ''
                                      ORDER BY TRUCK_NUMBER
                                      """;

            var units = await connection.QueryAsync<TruckCombinedUnit>(sql);
            var unitsList = units.ToList();

            _logger.LogInformation("Retrieved {Count} units from TRUCK_COMBINED table", unitsList.Count);
            return unitsList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving units from TRUCK_COMBINED table");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<TruckCombinedUnit?> GetUnitFromTruckCombinedAsync(string truckNumber, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            const string sql = _sql + "WHERE TRUCK_NUMBER = @TruckNumber";
                               
            var unit = await connection.QuerySingleOrDefaultAsync<TruckCombinedUnit>(sql, new { TruckNumber = truckNumber });

            if (unit != null)
            {
                _logger.LogInformation("Retrieved unit {TruckNumber} from TRUCK_COMBINED", truckNumber);
            }
            else
            {
                _logger.LogWarning("Unit {TruckNumber} not found in TRUCK_COMBINED", truckNumber);
            }

            return unit;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving unit {TruckNumber} from TRUCK_COMBINED", truckNumber);
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<IEnumerable<TruckCombinedUnit>> GetUnitsModifiedSinceAsync(DateTime lastSyncDate, CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            // Since the source tables don't have a LAST_MODIFIED field,
            // we'll return all units for now (full sync approach)
            const string sql = _sql + """
                                     WHERE TRUCK_NUMBER IS NOT NULL AND TRUCK_NUMBER != ''
                                     ORDER BY TRUCK_NUMBER
                                     """;

            var units = await connection.QueryAsync<TruckCombinedUnit>(sql);
            var unitsList = units.ToList();

            _logger.LogInformation("Retrieved {Count} units (full sync - no modification date available) from TRUCK_COMBINED",
                unitsList.Count);
            return unitsList;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving units from TRUCK_COMBINED for incremental sync");
            throw;
        }
    }

    /// <inheritdoc />
    public async Task<int> GetTotalUnitsCountAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync(cancellationToken);

            const string sql = @"
                SELECT COUNT(*) FROM (
                    SELECT TRUCK_NUMBER FROM TRUCK_AUS WHERE TRUCK_NUMBER IS NOT NULL AND TRUCK_NUMBER != ''
                    UNION
                    SELECT TRUCK_NUMBER FROM DBO.TRUCK_NZ WHERE TRUCK_NUMBER IS NOT NULL AND TRUCK_NUMBER != ''
                    UNION
                    SELECT TRUCK_NUMBER FROM DBO.TRUCK_USA WHERE TRUCK_NUMBER IS NOT NULL AND TRUCK_NUMBER != ''
                ) AS combined_trucks";

            var count = await connection.QuerySingleAsync<int>(sql);

            _logger.LogInformation("Total units count from combined truck tables: {Count}", count);
            return count;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting total units count from combined truck tables");
            throw;
        }
    }
}
