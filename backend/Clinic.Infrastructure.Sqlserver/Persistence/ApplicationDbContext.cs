using Clinic.Infrastructure.Sqlserver.Models;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Sqlserver.Persistence
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserDataModel> Users => Set<UserDataModel>();
        public DbSet<PersonDataModel> Persons => Set<PersonDataModel>();
        public DbSet<RoleDataModel> Roles => Set<RoleDataModel>();
        public DbSet<PermissionDataModel> Permissions => Set<PermissionDataModel>();
        public DbSet<UserRoleDataModel> UserRoles => Set<UserRoleDataModel>();
        public DbSet<RolePermissionDataModel> RolePermissions => Set<RolePermissionDataModel>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
        }
    }
}
