using Clinic.Infrastructure.Sqlserver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Infrastructure.Sqlserver.Configurations
{
    public class RolePermissionConfiguration
    {
        public void Configure(EntityTypeBuilder<RolePermissionDataModel> builder)
        {
            builder.ToTable("RolePermissions");

            builder.HasKey(rp => new { rp.RoleId, rp.PermissionId });

            builder.Property(rp => rp.RoleId).IsRequired();
            builder.Property(rp => rp.PermissionId).IsRequired();
        }
    }
}
