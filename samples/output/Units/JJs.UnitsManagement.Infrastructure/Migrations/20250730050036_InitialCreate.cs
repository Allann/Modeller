using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JJs.UnitsManagement.MigrationService.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SyncJobs",
                columns: table => new
                {
                    JobId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "The unique identifier for the sync job"),
                    JobName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "Optional name for the job"),
                    Status = table.Column<int>(type: "int", nullable: false, comment: "Current status of the job"),
                    CurrentStep = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false, comment: "Current step being processed"),
                    OverallProgress = table.Column<int>(type: "int", nullable: false, comment: "Overall progress percentage (0-100)"),
                    CurrentEntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "Current entity type being processed"),
                    ItemsProcessed = table.Column<int>(type: "int", nullable: false, comment: "Number of items processed in current step"),
                    TotalItems = table.Column<int>(type: "int", nullable: false, comment: "Total number of items in current step"),
                    StatusMessage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false, comment: "Status message"),
                    ErrorMessage = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true, comment: "Error message if the job failed"),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "When the job was created"),
                    StartedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true, comment: "When the job started processing"),
                    CompletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true, comment: "When the job completed"),
                    ConfigurationJson = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false, comment: "Sync configuration as JSON"),
                    ResultsJson = table.Column<string>(type: "nvarchar(max)", nullable: true, comment: "Final sync results as JSON (when completed)"),
                    SyncUnits = table.Column<bool>(type: "bit", nullable: false, comment: "Whether to sync units"),
                    CreateNew = table.Column<bool>(type: "bit", nullable: false, comment: "Whether to create new records"),
                    UpdateExisting = table.Column<bool>(type: "bit", nullable: false, comment: "Whether to update existing records"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "System", comment: "Who created the sync job"),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "When the sync job was last updated"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "System", comment: "Who last updated the sync job")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SyncJobs", x => x.JobId);
                });

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    UnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false, comment: "The unique identifier for the unit"),
                    TruckNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, comment: "The truck number (business key from TRUCK_NUMBER field)"),
                    RegistrationNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, comment: "Vehicle registration number (from REGISTRATION_NUMBER field)"),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true, comment: "Description of the unit (from DESCRIPTION field)"),
                    Make = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Vehicle make (extracted from TRUCK_MODEL field)"),
                    Model = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "Vehicle model (from TRUCK_MODEL field)"),
                    TruckType = table.Column<int>(type: "int", nullable: true, comment: "Type of truck for operational classification (from TRUCK_TYPE field)"),
                    EuroType = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, comment: "Euro emission standard type (from EURO_TYPE field)"),
                    EngineNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Engine number (from ENGINE_NUMBER field)"),
                    ChassisNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Chassis number (from CHASSIS_NUMBER field)"),
                    WarrantyDate = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "Warranty expiration date (from WARRANTY_DATE field)"),
                    State = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true, comment: "State or territory where unit operates (from STATE field)"),
                    Company = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: true, comment: "Company code for organisation linkage (from COMPANY field)"),
                    Department = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Department within the company (from DEPARTMENT field)"),
                    Activity = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "Activity or operational area (from ACTIVITY field)"),
                    CountryCode = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: true, comment: "Country code (from COUNTRY_CODE field)"),
                    DCN = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true, comment: "DCN identifier (from DCN field)"),
                    Extra = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true, comment: "Additional information (from EXTRA field)"),
                    Active = table.Column<bool>(type: "bit", nullable: false, defaultValue: true, comment: "Determines if the unit is active or not"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()", comment: "When the unit was created"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true, comment: "When the unit was last updated"),
                    CreatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false, defaultValue: "System", comment: "Who created the unit"),
                    UpdatedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true, comment: "Who last updated the unit")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.UnitId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SyncJobs_CompletedAt",
                table: "SyncJobs",
                column: "CompletedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SyncJobs_CreatedAt",
                table: "SyncJobs",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_SyncJobs_Status",
                table: "SyncJobs",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SyncJobs_Status_CreatedAt",
                table: "SyncJobs",
                columns: new[] { "Status", "CreatedAt" });

            migrationBuilder.CreateIndex(
                name: "IX_Units_Active",
                table: "Units",
                column: "Active");

            migrationBuilder.CreateIndex(
                name: "IX_Units_Active_TruckType_Company",
                table: "Units",
                columns: new[] { "Active", "TruckType", "Company" });

            migrationBuilder.CreateIndex(
                name: "IX_Units_Company",
                table: "Units",
                column: "Company");

            migrationBuilder.CreateIndex(
                name: "IX_Units_CreatedAt",
                table: "Units",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Units_RegistrationNumber",
                table: "Units",
                column: "RegistrationNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Units_State",
                table: "Units",
                column: "State");

            migrationBuilder.CreateIndex(
                name: "IX_Units_TruckNumber",
                table: "Units",
                column: "TruckNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Units_TruckType",
                table: "Units",
                column: "TruckType");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SyncJobs");

            migrationBuilder.DropTable(
                name: "Units");
        }
    }
}
