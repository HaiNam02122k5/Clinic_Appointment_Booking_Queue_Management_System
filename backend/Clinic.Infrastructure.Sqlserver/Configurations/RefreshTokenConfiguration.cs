using Clinic.Infrastructure.Sqlserver.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Clinic.Infrastructure.Sqlserver.Configurations
{
    public class RefreshTokenConfiguration
    {
        public void Configure(EntityTypeBuilder<RefreshTokenDataModel> builder)
        {
            builder.ToTable("RefreshTokens");

            builder.HasKey(rt => rt.Id);
            builder.Property(rt => rt.TokenHash).IsRequired().HasMaxLength(256);
            builder.Property(rt => rt.UserId).IsRequired();
            builder.Property(rt => rt.IssuedAt).IsRequired();
            builder.Property(rt => rt.ExpiresAt).IsRequired();
            builder.Property(rt => rt.RevokedAt).IsRequired(false);

            builder.HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
