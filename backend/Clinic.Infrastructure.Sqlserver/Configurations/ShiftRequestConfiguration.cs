using Clinic.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Infrastructure.Sqlserver.Configurations
{
    public class ShiftRequestConfiguration : IEntityTypeConfiguration<ShiftRequest>
    {
        public void Configure(EntityTypeBuilder<ShiftRequest> builder)
        {
            builder.ToTable("ShiftRequests");

            builder.HasKey(sr => sr.Id);

            builder.HasOne(sr => sr.Doctor)
                .WithMany(d => d.ShiftRequests)
                .HasForeignKey(sr => sr.DoctorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Property(sr => sr.Reason).HasMaxLength(500);

            builder.Property(sr => sr.Status)
                .HasConversion<string>()
                .HasMaxLength(20);

            // Liên kết tới WorkSchedule được tạo ra sau khi duyệt - optional,
            // 1 WorkSchedule chỉ được tạo từ tối đa 1 ShiftRequest.
            builder.HasOne(sr => sr.ApprovedWorkSchedule)
                .WithOne()
                .HasForeignKey<ShiftRequest>(sr => sr.ApprovedWorkScheduleId)
                .OnDelete(DeleteBehavior.Restrict);

            // Lưu ý: SQL Server chỉ cho phép TỐI ĐA 1 dòng NULL trong unique index
            // thông thường, nên phải dùng filtered index để bỏ qua NULL - nếu
            // không, chỉ 1 ShiftRequest được phép ở trạng thái Pending/Rejected
            // (ApprovedWorkScheduleId = NULL) trong toàn bộ bảng, các dòng còn
            // lại sẽ báo lỗi trùng khóa khi insert.
            builder.HasIndex(sr => sr.ApprovedWorkScheduleId)
                .IsUnique()
                .HasFilter("[ApprovedWorkScheduleId] IS NOT NULL");
        }
    }
}
