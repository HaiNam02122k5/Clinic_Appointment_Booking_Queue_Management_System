using Clinic.Infrastructure.Sqlserver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Infrastructure.Sqlserver.Configurations
{
    public class PersonConfiguration : IEntityTypeConfiguration<PersonDataModel>
    {
        public void Configure(EntityTypeBuilder<PersonDataModel> builder)
        {
            builder.ToTable("Persons");

            builder.HasKey(p => p.Id);

            builder.Property(p => p.FullName).IsRequired().HasMaxLength(100);
            builder.Property(p => p.PhoneNumber).IsRequired().HasMaxLength(20);
            builder.Property(p => p.Email).HasMaxLength(100);
            builder.Property(p => p.DateOfBirth).IsRequired();
            builder.Property(p => p.Gender).IsRequired();
            builder.Property(p => p.Address).IsRequired().HasMaxLength(200);
            builder.Property(p => p.CreatedAt).IsRequired();
            builder.Property(p => p.UpdatedAt).IsRequired();
            builder.Property(p => p.IsDeleted).IsRequired();
        }
    }
}
