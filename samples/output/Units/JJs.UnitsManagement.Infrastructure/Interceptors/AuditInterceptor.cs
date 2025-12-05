using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using JJs.UnitsManagement.Infrastructure.Entities;

namespace JJs.UnitsManagement.Infrastructure.Interceptors;

/// <summary>
/// Interceptor that automatically sets audit properties for Unit entities
/// </summary>
public class AuditInterceptor : SaveChangesInterceptor
{
    /// <summary>
    /// Intercepts the saving changes operation to set audit properties
    /// </summary>
    /// <param name="eventData">The event data</param>
    /// <param name="result">The result</param>
    /// <returns>The interception result</returns>
    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        SetAuditProperties(eventData.Context);
        return base.SavingChanges(eventData, result);
    }

    /// <summary>
    /// Intercepts the saving changes operation asynchronously to set audit properties
    /// </summary>
    /// <param name="eventData">The event data</param>
    /// <param name="result">The result</param>
    /// <param name="cancellationToken">The cancellation token</param>
    /// <returns>The interception result</returns>
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        SetAuditProperties(eventData.Context);
        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    /// <summary>
    /// Sets audit properties on Unit entities being saved
    /// </summary>
    /// <param name="context">The database context</param>
    private static void SetAuditProperties(DbContext? context)
    {
        if (context == null)
        {
            return;
        }

        var utcNow = DateTime.UtcNow;
        const string systemUser = "System";

        foreach (var entry in context.ChangeTracker.Entries<Unit>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = utcNow;
                    entry.Entity.CreatedBy = systemUser;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = utcNow;
                    entry.Entity.UpdatedBy = systemUser;
                    // Prevent updates to CreatedAt and CreatedBy
                    entry.Property(e => e.CreatedAt).IsModified = false;
                    entry.Property(e => e.CreatedBy).IsModified = false;
                    break;
            }
        }

        // Handle SyncJob entities as well
        foreach (var entry in context.ChangeTracker.Entries<SyncJob>())
        {
            var utcNowOffset = DateTimeOffset.UtcNow;

            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = utcNowOffset;
                    entry.Entity.CreatedBy = systemUser;
                    entry.Entity.UpdatedAt = utcNowOffset;
                    entry.Entity.UpdatedBy = systemUser;
                    break;

                case EntityState.Modified:
                    entry.Entity.UpdatedAt = utcNowOffset;
                    entry.Entity.UpdatedBy = systemUser;
                    // Prevent updates to CreatedAt and CreatedBy
                    entry.Property(e => e.CreatedAt).IsModified = false;
                    entry.Property(e => e.CreatedBy).IsModified = false;
                    break;
            }
        }
    }
}
