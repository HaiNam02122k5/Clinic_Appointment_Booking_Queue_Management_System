using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Clinic.Infrastructure.Sqlserver.Migrations
{
    /// <inheritdoc />
    public partial class SyncDoctorWorkScheduleEntityChanges : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Migration này ban đầu được generate từ một model snapshot đã lỗi thời (stale),
            // nên toàn bộ thao tác trong nó (DropColumn Status/WorkHistories, AddColumn
            // PatientLimitPerSlot/ShiftRequests, AlterColumn Qualification/ExperienceYears
            // của Doctors, AddColumn CancellationReason/WorkSchedules, insert/update các
            // permission employee.*/doctor.view.any/doctor.view.own) đều đã được áp dụng
            // bởi các migration trước đó:
            //   - 20260811052530_AddShiftRequestColumnRemoveWorkHistoryColumn
            //   - 20260812052216_AddWSColumn
            //   - 20260817104708_AddPermissions
            // Không còn thay đổi thực sự nào mới, nên để migration này là no-op để giữ đúng
            // thứ tự lịch sử migration mà không tác động lên schema/dữ liệu.
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
        }
    }
}