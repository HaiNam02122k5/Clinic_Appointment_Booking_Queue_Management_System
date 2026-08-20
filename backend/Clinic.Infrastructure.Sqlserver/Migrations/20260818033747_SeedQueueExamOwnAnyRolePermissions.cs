using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Clinic.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class SeedQueueExamOwnAnyRolePermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedAt", "Description", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("2000000b-0000-0000-0000-000000000001"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Start exam for own queue ticket (Doctor)", false, "queue.start-exam.own", null },
                    { new Guid("2000000b-0000-0000-0000-000000000002"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Start exam for any doctor's queue ticket (Admin/Receptionist)", false, "queue.start-exam.any", null },
                    { new Guid("2000000b-0000-0000-0000-000000000003"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Complete exam for own queue ticket (Doctor)", false, "queue.complete-exam.own", null },
                    { new Guid("2000000b-0000-0000-0000-000000000004"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Complete exam for any doctor's queue ticket (Admin/Receptionist)", false, "queue.complete-exam.any", null }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
                    { new Guid("2000000b-0000-0000-0000-000000000002"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("2000000b-0000-0000-0000-000000000004"), new Guid("00000000-0000-0000-0000-000000000001") },
                    { new Guid("2000000b-0000-0000-0000-000000000002"), new Guid("00000000-0000-0000-0000-000000000003") },
                    { new Guid("2000000b-0000-0000-0000-000000000004"), new Guid("00000000-0000-0000-0000-000000000003") },
                    { new Guid("2000000b-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000004") },
                    { new Guid("2000000b-0000-0000-0000-000000000003"), new Guid("00000000-0000-0000-0000-000000000004") }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2000000b-0000-0000-0000-000000000002"), new Guid("00000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2000000b-0000-0000-0000-000000000004"), new Guid("00000000-0000-0000-0000-000000000001") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2000000b-0000-0000-0000-000000000002"), new Guid("00000000-0000-0000-0000-000000000003") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2000000b-0000-0000-0000-000000000004"), new Guid("00000000-0000-0000-0000-000000000003") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2000000b-0000-0000-0000-000000000001"), new Guid("00000000-0000-0000-0000-000000000004") });

            migrationBuilder.DeleteData(
                table: "RolePermissions",
                keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2000000b-0000-0000-0000-000000000003"), new Guid("00000000-0000-0000-0000-000000000004") });

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("2000000b-0000-0000-0000-000000000001"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("2000000b-0000-0000-0000-000000000002"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("2000000b-0000-0000-0000-000000000003"));

            migrationBuilder.DeleteData(
                table: "Permissions",
                keyColumn: "Id",
                keyValue: new Guid("2000000b-0000-0000-0000-000000000004"));
        }
    }
}
