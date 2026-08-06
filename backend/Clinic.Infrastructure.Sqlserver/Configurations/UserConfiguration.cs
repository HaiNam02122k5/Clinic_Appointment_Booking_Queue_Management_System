using Clinic.Infrastructure.Sqlserver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Infrastructure.Sqlserver.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<UserDataModel>
    {
        public void Configure(EntityTypeBuilder<UserDataModel> builder)
        {
            builder.ToTable("Users");

            builder.HasKey(u => u.Id);

            builder.Property(u => u.Username).IsRequired().HasMaxLength(30);
            builder.Property(u => u.PasswordHash).IsRequired();
            builder.Property(u => u.CreatedAt).IsRequired();
            builder.Property(u => u.IsActive).IsRequired();
            builder.HasIndex(u => u.Username).IsUnique();

            builder.HasOne(u => u.Person)
                .WithOne(p => p.User)
                .HasForeignKey<UserDataModel>(u => u.PersonId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(u => u.PersonId).IsUnique();
        }
    }
}
