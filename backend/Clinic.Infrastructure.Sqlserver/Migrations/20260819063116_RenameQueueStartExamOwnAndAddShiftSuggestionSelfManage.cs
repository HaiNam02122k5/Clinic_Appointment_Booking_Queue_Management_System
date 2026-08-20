using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class RenameQueueStartExamOwnAndAddShiftSuggestionSelfManage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("2000000b-0000-0000-0000-000000000001"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "Manage doctor own shift suggestions (Doctor)", "shift.suggestion.self-manage" });

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedAt", "Description", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[] { new Guid("2000000b-0000-0000-0000-000000000005"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Start exam for own queue ticket (Doctor)", false, "queue.start-exam.own", null });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[] { new Guid("2000000b-0000-0000-0000-000000000005"), new Guid("00000000-0000-0000-0000-000000000004") });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2000000b-0000-0000-0000-000000000005"), new Guid("00000000-0000-0000-0000-000000000004") });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("2000000b-0000-0000-0000-000000000005"));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("2000000b-0000-0000-0000-000000000001"),
                columns: new[] { "Description", "Name" },
                values: new object[] { "Start exam for own queue ticket (Doctor)", "queue.start-exam.own" });
        }
    }
}
