using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace JJs.UnitsManagement.MigrationService.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Unit");

            migrationBuilder.RenameTable(
                name: "Units",
                newName: "Units",
                newSchema: "Unit");

            migrationBuilder.RenameTable(
                name: "SyncJobs",
                newName: "SyncJobs",
                newSchema: "Unit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameTable(
                name: "Units",
                schema: "Unit",
                newName: "Units");

            migrationBuilder.RenameTable(
                name: "SyncJobs",
                schema: "Unit",
                newName: "SyncJobs");
        }
    }
}
