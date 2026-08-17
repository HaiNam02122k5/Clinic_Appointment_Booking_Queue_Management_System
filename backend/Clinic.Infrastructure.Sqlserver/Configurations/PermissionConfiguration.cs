using Clinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Infrastructure.Sqlserver.Configurations
{
    public class PermissionConfiguration : IEntityTypeConfiguration<Permission>
    {
        public void Configure(EntityTypeBuilder<Permission> builder)
        {
            builder.ToTable("Permissions");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.HasIndex(p => p.Name).IsUnique();

            builder.Property(p => p.Description).HasMaxLength(300);

            builder.HasData(
                new Permission(Guid.Parse("10000000-0000-0000-0000-000000000001"), "appointment.create", "Create appointments", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("10000000-0000-0000-0000-000000000002"), "appointment.view", "View appointments", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("10000000-0000-0000-0000-000000000003"), "appointment.cancel", "Cancel appointments (deprecated - dùng appointment.cancel.own/.any)", new DateTime(2026, 1, 1), null, true),
                new Permission(Guid.Parse("10000000-0000-0000-0000-00000000000E"), "appointment.cancel.own", "Cancel own appointment (Patient)", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("10000000-0000-0000-0000-00000000000F"), "appointment.cancel.any", "Cancel any appointment (Admin/Receptionist)", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("10000000-0000-0000-0000-000000000004"), "queue.call-next", "Call next in queue", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("10000000-0000-0000-0000-000000000005"), "doctor.view", "View doctor profile", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("10000000-0000-0000-0000-000000000006"), "doctor.edit", "Edit doctor profile (deprecated - dùng doctor.edit.own/.any)", new DateTime(2026, 1, 1), null, true),
                new Permission(Guid.Parse("10000000-0000-0000-0000-000000000007"), "user.manage", "Manage users", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("10000000-0000-0000-0000-000000000008"), "role.manage", "Manage roles and permissions", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("10000000-0000-0000-0000-000000000009"), "appointment.update", "Update appointments", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("1000000A-0000-0000-0000-000000000001"), "appointment.confirm", "Confirm appointments", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("1000000A-0000-0000-0000-000000000002"), "appointment.reschedule", "Reschedule appointments", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("10000000-0000-0000-0000-00000000000C"), "doctor.queue.view", "View doctor's queue", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("10000000-0000-0000-0000-00000000000D"), "doctor.create", "Create doctor profile", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("10000000-0000-0000-0000-000000000013"), "doctor.edit.own", "Edit own doctor profile (Doctor)", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("10000000-0000-0000-0000-000000000014"), "doctor.edit.any", "Edit any doctor profile (Admin)", new DateTime(2026, 1, 1), null, false),

                new Permission(Guid.Parse("20000000-0000-0000-0000-000000000001"), "medical-report.view", "View medical reports", new DateTime(2026, 1, 1), null, true),
                new Permission(Guid.Parse("20000000-0000-0000-0000-000000000002"), "medical-report.create", "Create medical reports", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("20000000-0000-0000-0000-000000000003"), "patient-history.view", "View patient medical history", new DateTime(2026, 1, 1), null, true),
                new Permission(Guid.Parse("20000000-0000-0000-0000-000000000004"), "shift.manage", "Manage any doctor's shifts", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("20000000-0000-0000-0000-000000000005"), "shift.self-manage", "Manage own shift requests", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("20000000-0000-0000-0000-000000000006"), "queue.view", "View queue", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("20000000-0000-0000-0000-000000000007"), "queue.check-in", "Check in a patient into queue", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("20000000-0000-0000-0000-000000000008"), "queue.skip", "Skip a patient in queue", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("20000000-0000-0000-0000-000000000009"), "queue.priority", "Set priority in queue", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("20000000-0000-0000-0000-00000000000A"), "medical-report.view.own", "View own medical reports (Patient)", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("20000000-0000-0000-0000-00000000000B"), "medical-report.view.related", "View medical reports of patients in own exam cases (Doctor)", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("20000000-0000-0000-0000-00000000000C"), "medical-report.view.any", "View any medical report (Admin)", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("20000000-0000-0000-0000-00000000000D"), "patient-history.view.own", "View own medical history (Patient)", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("20000000-0000-0000-0000-00000000000E"), "patient-history.view.related", "View medical history of patients in own exam cases (Doctor)", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("20000000-0000-0000-0000-00000000000F"), "patient-history.view.any", "View any patient's medical history (Admin)", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("2000000A-0000-0000-0000-000000000001"), "notification.manage", "Manage notifications", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("2000000A-0000-0000-0000-000000000002"), "report.view", "View clinic-wide reports", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("2000000A-0000-0000-0000-000000000003"), "specialty.manage", "Manage medical specialties", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("2000000A-0000-0000-0000-000000000004"), "slot.view", "View appointment slots", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("2000000A-0000-0000-0000-000000000005"), "slot.manage", "Manage appointment slots", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("2000000A-0000-0000-0000-000000000006"), "shift.suggestion.manage", "Approve/reject doctor shift-change suggestions (Receptionist)", new DateTime(2026, 1, 1), null, false),

                // queue.start-exam / queue.complete-exam (own/any split - trước đó bị leader phát hiện là
                // permission cấp theo Role "Doctor" chung, không phân biệt DoctorId, khiến 1 bác sĩ có thể
                // thao tác lên hàng đợi của bác sĩ khác. Dùng GUID mới (2000000B-...) thay vì tái sử dụng
                // "queue.start-exam"/"queue.complete-exam" cũ, vì 2 permission cũ đó chỉ được insert bằng
                // raw migration (SeedQueueStateMachinePermissions) và CHƯA từng có trong file cấu hình này -
                // tái dùng GUID cũ sẽ khiến EF cố INSERT trùng khóa chính khi migrate trên DB đã tồn tại.
                new Permission(Guid.Parse("2000000B-0000-0000-0000-000000000001"), "queue.start-exam.own", "Start exam for own queue ticket (Doctor)", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("2000000B-0000-0000-0000-000000000002"), "queue.start-exam.any", "Start exam for any doctor's queue ticket (Admin/Receptionist)", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("2000000B-0000-0000-0000-000000000003"), "queue.complete-exam.own", "Complete exam for own queue ticket (Doctor)", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("2000000B-0000-0000-0000-000000000004"), "queue.complete-exam.any", "Complete exam for any doctor's queue ticket (Admin/Receptionist)", new DateTime(2026, 1, 1), null, false)
            );
        }
    }
}