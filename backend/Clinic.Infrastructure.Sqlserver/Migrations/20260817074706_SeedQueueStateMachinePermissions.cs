using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class SeedQueueStateMachinePermissions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Permissions",
                columns: new[] { "Id", "CreatedAt", "Description", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
            { new Guid("2000000a-0000-0000-0000-000000000007"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Start exam for a called patient", false, "queue.start-exam", null },
            { new Guid("2000000a-0000-0000-0000-000000000008"), new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "Complete exam for a patient in progress", false, "queue.complete-exam", null }
                });

            migrationBuilder.InsertData(
                table: "RolePermissions",
                columns: new[] { "PermissionId", "RoleId" },
                values: new object[,]
                {
            { new Guid("2000000a-0000-0000-0000-000000000007"), new Guid("00000000-0000-0000-0000-000000000001") }, // Admin
            { new Guid("2000000a-0000-0000-0000-000000000007"), new Guid("00000000-0000-0000-0000-000000000003") }, // Receptionist
            { new Guid("2000000a-0000-0000-0000-000000000007"), new Guid("00000000-0000-0000-0000-000000000004") }, // Doctor
            { new Guid("2000000a-0000-0000-0000-000000000008"), new Guid("00000000-0000-0000-0000-000000000001") }, // Admin
            { new Guid("2000000a-0000-0000-0000-000000000008"), new Guid("00000000-0000-0000-0000-000000000003") }, // Receptionist
            { new Guid("2000000a-0000-0000-0000-000000000008"), new Guid("00000000-0000-0000-0000-000000000004") }  // Doctor
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(table: "RolePermissions", keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2000000a-0000-0000-0000-000000000007"), new Guid("00000000-0000-0000-0000-000000000001") });
            migrationBuilder.DeleteData(table: "RolePermissions", keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2000000a-0000-0000-0000-000000000007"), new Guid("00000000-0000-0000-0000-000000000003") });
            migrationBuilder.DeleteData(table: "RolePermissions", keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2000000a-0000-0000-0000-000000000007"), new Guid("00000000-0000-0000-0000-000000000004") });
            migrationBuilder.DeleteData(table: "RolePermissions", keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2000000a-0000-0000-0000-000000000008"), new Guid("00000000-0000-0000-0000-000000000001") });
            migrationBuilder.DeleteData(table: "RolePermissions", keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2000000a-0000-0000-0000-000000000008"), new Guid("00000000-0000-0000-0000-000000000003") });
            migrationBuilder.DeleteData(table: "RolePermissions", keyColumns: new[] { "PermissionId", "RoleId" },
                keyValues: new object[] { new Guid("2000000a-0000-0000-0000-000000000008"), new Guid("00000000-0000-0000-0000-000000000004") });

            migrationBuilder.DeleteData(table: "Permissions", keyColumn: "Id", keyValue: new Guid("2000000a-0000-0000-0000-000000000007"));
            migrationBuilder.DeleteData(table: "Permissions", keyColumn: "Id", keyValue: new Guid("2000000a-0000-0000-0000-000000000008"));
        }
    }
}
