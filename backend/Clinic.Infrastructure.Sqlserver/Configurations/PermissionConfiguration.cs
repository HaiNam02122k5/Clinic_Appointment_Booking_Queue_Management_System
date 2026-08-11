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
                new Permission(Guid.Parse("10000000-0000-0000-0000-000000000003"), "appointment.cancel", "Cancel appointments", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("10000000-0000-0000-0000-000000000004"), "queue.call-next", "Call next in queue", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("10000000-0000-0000-0000-000000000005"), "doctor.view", "View doctor profile", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("10000000-0000-0000-0000-000000000006"), "doctor.edit", "Edit doctor profile", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("10000000-0000-0000-0000-000000000007"), "user.manage", "Manage users", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("10000000-0000-0000-0000-000000000008"), "role.manage", "Manage roles and permissions", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("10000000-0000-0000-0000-000000000009"), "appointment.update", "Update appointments", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("1000000A-0000-0000-0000-000000000001"), "appointment.confirm", "Confirm appointments", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("1000000A-0000-0000-0000-000000000002"), "appointment.reschedule", "Reschedule appointments", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("10000000-0000-0000-0000-00000000000C"), "doctor.queue.view", "View doctor's queue", new DateTime(2026, 1, 1), null, false),
                new Permission(Guid.Parse("10000000-0000-0000-0000-00000000000D"), "doctor.create", "Create doctor profile", new DateTime(2026, 1, 1), null, false)
            );
        }
    }
}
