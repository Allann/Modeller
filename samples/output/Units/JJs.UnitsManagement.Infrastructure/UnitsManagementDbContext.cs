using JJs.UnitsManagement.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace JJs.UnitsManagement.Infrastructure;

/// <summary>
/// Entity Framework DbContext for the Units Management system
/// </summary>
/// <remarks>
/// Initializes a new instance of the UnitsManagementDbContext
/// </remarks>
/// <param name="options">The DbContext options</param>
public class UnitsManagementDbContext(DbContextOptions<UnitsManagementDbContext> options) : DbContext(options)
{
    /// <summary>
    /// Units (physical trucks) in the units management system
    /// </summary>
    public DbSet<Unit> Units { get; set; } = null!;

    /// <summary>
    /// Background sync jobs for units synchronization
    /// </summary>
    public DbSet<SyncJob> SyncJobs { get; set; } = null!;

    /// <summary>
    /// Configures the model using entity type configurations
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("Unit");

        // Apply all entity configurations from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(UnitsManagementDbContext).Assembly);
    }
}
