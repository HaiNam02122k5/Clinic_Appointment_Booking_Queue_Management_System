using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Clinic.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class AddPatientReschedulePermission : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("20000000-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("20000000-0000-0000-0000-000000000003"), new Guid("00000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("20000000-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000004") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("20000000-0000-0000-0000-000000000003"), new Guid("00000000-0000-0000-0000-000000000004") });

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"),
                column: "IsDeleted",
                value: true);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000003"),
                column: "IsDeleted",
                value: true);

            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedAt", "Description", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-00000000000a"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "View own medical reports (Patient)", false, "medical-report.view.own", null },
                    { new Guid("20000000-0000-0000-0000-00000000000b"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "View medical reports of patients in own exam cases (Doctor)", false, "medical-report.view.related", null },
                    { new Guid("20000000-0000-0000-0000-00000000000c"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "View any medical report (Admin)", false, "medical-report.view.any", null },
                    { new Guid("20000000-0000-0000-0000-00000000000d"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "View own medical history (Patient)", false, "patient-history.view.own", null },
                    { new Guid("20000000-0000-0000-0000-00000000000e"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "View medical history of patients in own exam cases (Doctor)", false, "patient-history.view.related", null },
                    { new Guid("20000000-0000-0000-0000-00000000000f"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "View any patient's medical history (Admin)", false, "patient-history.view.any", null },
                    { new Guid("2000000a-0000-0000-0000-000000000006"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Approve/reject doctor shift-change suggestions (Receptionist)", false, "shift.suggestion.manage", null }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("1000000a-0000-0000-0000-000000000002"), new Guid("00000000-0000-0000-0000-000000000002") },
                    { new Guid("20000000-0000-0000-0000-00000000000c"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("20000000-0000-0000-0000-00000000000f"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("20000000-0000-0000-0000-00000000000a"), new Guid("00000000-0000-0000-0000-000000000002") },
                    { new Guid("20000000-0000-0000-0000-00000000000d"), new Guid("00000000-0000-0000-0000-000000000002") },
                    { new Guid("20000000-0000-0000-0000-00000000000b"), new Guid("00000000-0000-0000-0000-000000000004") },
                    { new Guid("20000000-0000-0000-0000-00000000000e"), new Guid("00000000-0000-0000-0000-000000000004") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("2000000a-0000-0000-0000-000000000006"));

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("20000000-0000-0000-0000-00000000000c"), new Guid("00000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("20000000-0000-0000-0000-00000000000f"), new Guid("00000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("1000000a-0000-0000-0000-000000000002"), new Guid("00000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("20000000-0000-0000-0000-00000000000a"), new Guid("00000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("20000000-0000-0000-0000-00000000000d"), new Guid("00000000-0000-0000-0000-000000000002") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("20000000-0000-0000-0000-00000000000b"), new Guid("00000000-0000-0000-0000-000000000004") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("20000000-0000-0000-0000-00000000000e"), new Guid("00000000-0000-0000-0000-000000000004") });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-00000000000a"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-00000000000b"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-00000000000c"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-00000000000d"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-00000000000e"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-00000000000f"));

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000001"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.UpdateData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("20000000-0000-0000-0000-000000000003"),
                column: "IsDeleted",
                value: false);

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("20000000-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("20000000-0000-0000-0000-000000000003"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("20000000-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000004") },
                    { new Guid("20000000-0000-0000-0000-000000000003"), new Guid("00000000-0000-0000-0000-000000000004") }
                });
        }
    }
}
