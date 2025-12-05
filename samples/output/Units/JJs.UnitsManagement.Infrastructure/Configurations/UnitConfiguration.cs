using JJs.UnitsManagement.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace JJs.UnitsManagement.Infrastructure.Configurations;

/// <summary>
/// Entity Framework configuration for Unit entity
/// </summary>
public class UnitConfiguration : IEntityTypeConfiguration<Unit>
{
    /// <summary>
    /// Configures the Unit entity
    /// </summary>
    /// <param name="builder">The entity type builder</param>
    public void Configure(EntityTypeBuilder<Unit> builder)
    {
        // Table configuration
        builder.ToTable("Units");
        builder.HasKey(e => e.UnitId);

        // Property configurations
        builder.Property(e => e.UnitId)
            .IsRequired()
            .ValueGeneratedNever() // Using GUID v7
            .HasComment("The unique identifier for the unit");

        builder.Property(e => e.TruckNumber)
            .IsRequired()
            .HasMaxLength(50)
            .HasComment("The truck number (business key from TRUCK_NUMBER field)");

        builder.Property(e => e.RegistrationNumber)
            .HasMaxLength(20)
            .HasComment("Vehicle registration number (from REGISTRATION_NUMBER field)");

        builder.Property(e => e.Description)
            .HasMaxLength(200)
            .HasComment("Description of the unit (from DESCRIPTION field)");

        builder.Property(e => e.Make)
            .HasMaxLength(50)
            .HasComment("Vehicle make (extracted from TRUCK_MODEL field)");

        builder.Property(e => e.Model)
            .HasMaxLength(100)
            .HasComment("Vehicle model (from TRUCK_MODEL field)");

        builder.Property(e => e.TruckType)
            .HasComment("Type of truck for operational classification (from TRUCK_TYPE field)");

        builder.Property(e => e.EuroType)
            .HasMaxLength(20)
            .HasComment("Euro emission standard type (from EURO_TYPE field)");

        builder.Property(e => e.EngineNumber)
            .HasMaxLength(50)
            .HasComment("Engine number (from ENGINE_NUMBER field)");

        builder.Property(e => e.ChassisNumber)
            .HasMaxLength(50)
            .HasComment("Chassis number (from CHASSIS_NUMBER field)");

        builder.Property(e => e.WarrantyDate)
            .HasComment("Warranty expiration date (from WARRANTY_DATE field)");

        builder.Property(e => e.State)
            .HasMaxLength(10)
            .HasComment("State or territory where unit operates (from STATE field)");

        builder.Property(e => e.Company)
            .HasMaxLength(20)
            .HasComment("Company code for organisation linkage (from COMPANY field)");

        builder.Property(e => e.Department)
            .HasMaxLength(50)
            .HasComment("Department within the company (from DEPARTMENT field)");

        builder.Property(e => e.Activity)
            .HasMaxLength(50)
            .HasComment("Activity or operational area (from ACTIVITY field)");

        builder.Property(e => e.CountryCode)
            .HasMaxLength(5)
            .HasComment("Country code (from COUNTRY_CODE field)");

        builder.Property(e => e.DCN)
            .HasMaxLength(50)
            .HasComment("DCN identifier (from DCN field)");

        builder.Property(e => e.Extra)
            .HasMaxLength(500)
            .HasComment("Additional information (from EXTRA field)");

        builder.Property(e => e.Active)
            .IsRequired()
            .HasDefaultValue(true)
            .HasComment("Determines if the unit is active or not");

        builder.Property(e => e.CreatedAt)
            .IsRequired()
            .HasDefaultValueSql("GETUTCDATE()")
            .HasComment("When the unit was created");

        builder.Property(e => e.UpdatedAt)
            .HasComment("When the unit was last updated");

        builder.Property(e => e.CreatedBy)
            .IsRequired()
            .HasMaxLength(100)
            .HasDefaultValue("System")
            .HasComment("Who created the unit");

        builder.Property(e => e.UpdatedBy)
            .HasMaxLength(100)
            .HasComment("Who last updated the unit");

        // Indexes for performance
        builder.HasIndex(e => e.TruckNumber)
            .IsUnique()
            .HasDatabaseName("IX_Units_TruckNumber");

        builder.HasIndex(e => e.RegistrationNumber)
            .HasDatabaseName("IX_Units_RegistrationNumber");

        builder.HasIndex(e => e.Active)
            .HasDatabaseName("IX_Units_Active");

        builder.HasIndex(e => e.TruckType)
            .HasDatabaseName("IX_Units_TruckType");

        builder.HasIndex(e => e.Company)
            .HasDatabaseName("IX_Units_Company");

        builder.HasIndex(e => e.State)
            .HasDatabaseName("IX_Units_State");

        builder.HasIndex(e => e.CreatedAt)
            .HasDatabaseName("IX_Units_CreatedAt");

        // Composite index for common queries
        builder.HasIndex(e => new { e.Active, e.TruckType, e.Company })
            .HasDatabaseName("IX_Units_Active_TruckType_Company");
    }
}
