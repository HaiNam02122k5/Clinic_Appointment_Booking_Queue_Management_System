using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class ChangeAppointmentAndSnapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentSnapshots_Users_CreatedByUserId",
                table: "AppointmentSnapshots");

            migrationBuilder.DropColumn(
                name: "CancelledByUserId",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "AppointmentSnapshots",
                newName: "UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "AppointmentSnapshots",
                newName: "UpdatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentSnapshots_CreatedByUserId",
                table: "AppointmentSnapshots",
                newName: "IX_AppointmentSnapshots_UpdatedByUserId");

            migrationBuilder.RenameColumn(
                name: "CreatedByUserId",
                table: "Appointments",
                newName: "UpdatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Appointments_UpdatedByUserId",
                table: "Appointments",
                column: "UpdatedByUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appointments_Users_UpdatedByUserId",
                table: "Appointments",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentSnapshots_Users_UpdatedByUserId",
                table: "AppointmentSnapshots",
                column: "UpdatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appointments_Users_UpdatedByUserId",
                table: "Appointments");

            migrationBuilder.DropForeignKey(
                name: "FK_AppointmentSnapshots_Users_UpdatedByUserId",
                table: "AppointmentSnapshots");

            migrationBuilder.DropIndex(
                name: "IX_Appointments_UpdatedByUserId",
                table: "Appointments");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "AppointmentSnapshots",
                newName: "CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "AppointmentSnapshots",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_AppointmentSnapshots_UpdatedByUserId",
                table: "AppointmentSnapshots",
                newName: "IX_AppointmentSnapshots_CreatedByUserId");

            migrationBuilder.RenameColumn(
                name: "UpdatedByUserId",
                table: "Appointments",
                newName: "CreatedByUserId");

            migrationBuilder.AddColumn<Guid>(
                name: "CancelledByUserId",
                table: "Appointments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AppointmentSnapshots_Users_CreatedByUserId",
                table: "AppointmentSnapshots",
                column: "CreatedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
