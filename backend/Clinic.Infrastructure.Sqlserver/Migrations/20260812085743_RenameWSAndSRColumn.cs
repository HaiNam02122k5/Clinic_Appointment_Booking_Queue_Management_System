using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class RenameWSAndSRColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PatientLimitPerSlot",
                table: "WorkSchedules",
                newName: "PatientLimit");

            migrationBuilder.RenameColumn(
                name: "PatientLimitPerSlot",
                table: "ShiftRequests",
                newName: "PatientLimit");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PatientLimit",
                table: "WorkSchedules",
                newName: "PatientLimitPerSlot");

            migrationBuilder.RenameColumn(
                name: "PatientLimit",
                table: "ShiftRequests",
                newName: "PatientLimitPerSlot");
        }
    }
}
