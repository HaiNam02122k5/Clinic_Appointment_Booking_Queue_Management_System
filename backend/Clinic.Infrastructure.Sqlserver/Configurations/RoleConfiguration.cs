using Clinic.Infrastructure.Sqlserver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Infrastructure.Sqlserver.Configurations
{
    public class RoleConfiguration
    {
        public void Configure(EntityTypeBuilder<RoleDataModel> builder)
        {
            builder.ToTable("Roles");

            builder.HasKey(r => r.Id);

            builder.Property(r => r.Name).IsRequired().HasMaxLength(50);
            builder.Property(r => r.Description).HasMaxLength(300);
        }
    }
}
