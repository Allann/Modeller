using JJs.UnitsManagement.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JJs.UnitsManagement.Infrastructure.Configurations;

/// <summary>
/// Entity configuration for SyncJob
/// </summary>
public class SyncJobConfiguration : IEntityTypeConfiguration<SyncJob>
{
    /// <summary>
    /// Configures the SyncJob entity
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    public void Configure(EntityTypeBuilder<SyncJob> builder)
    {
        // Table configuration
        builder.ToTable("SyncJobs");

        // Primary key
        builder.HasKey(e => e.JobId);

        // Properties
        builder.Property(e => e.JobId)
            .IsRequired()
            .HasComment("The unique identifier for the sync job");

        builder.Property(e => e.JobName)
            .HasMaxLength(100)
            .HasComment("Optional name for the job");

        builder.Property(e => e.Status)
            .IsRequired()
            .HasComment("Current status of the job");

        builder.Property(e => e.CurrentStep)
            .IsRequired()
            .HasMaxLength(200)
            .HasComment("Current step being processed");

        builder.Property(e => e.OverallProgress)
            .IsRequired()
            .HasComment("Overall progress percentage (0-100)");

        builder.Property(e => e.CurrentEntityType)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("Current entity type being processed");

        builder.Property(e => e.ItemsProcessed)
            .IsRequired()
            .HasComment("Number of items processed in current step");

        builder.Property(e => e.TotalItems)
            .IsRequired()
            .HasComment("Total number of items in current step");

        builder.Property(e => e.StatusMessage)
            .IsRequired()
            .HasMaxLength(500)
            .HasComment("Status message");

        builder.Property(e => e.ErrorMessage)
            .HasMaxLength(2000)
            .HasComment("Error message if the job failed");

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()")
            .HasComment("When the job was created");

        builder.Property(e => e.CreatedBy)
            .IsRequired()
            .HasMaxLength(100)
            .HasDefaultValue("System")
            .HasComment("Who created the sync job");

        builder.Property(e => e.StartedAt)
            .HasComment("When the job started processing");

        builder.Property(e => e.CompletedAt)
            .HasComment("When the job completed");

        builder.Property(e => e.ConfigurationJson)
            .IsRequired()
            .HasMaxLength(1000)
            .HasComment("Sync configuration as JSON");

        builder.Property(e => e.ResultsJson)
            .HasComment("Final sync results as JSON (when completed)");

        builder.Property(e => e.SyncUnits)
            .IsRequired()
            .HasComment("Whether to sync units");

        builder.Property(e => e.CreateNew)
            .IsRequired()
            .HasComment("Whether to create new records");

        builder.Property(e => e.UpdateExisting)
            .IsRequired()
            .HasComment("Whether to update existing records");

        builder.Property(e => e.UpdatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()")
            .HasComment("When the sync job was last updated");

        builder.Property(e => e.UpdatedBy)
            .IsRequired()
            .HasMaxLength(100)
            .HasDefaultValue("System")
            .HasComment("Who last updated the sync job");

        // Indexes for performance
        builder.HasIndex(e => e.Status)
            .HasDatabaseName("IX_SyncJobs_Status");

        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_SyncJobs_CreatedAt");

        builder.HasIndex(e => e.CompletedAt)
            .HasDatabaseName("IX_SyncJobs_CompletedAt");

        // Composite index for active job queries
        builder.HasIndex(e => new { e.Status, e.CreatedAt })
            .HasDatabaseName("IX_SyncJobs_Status_CreatedAt");
    }
}
