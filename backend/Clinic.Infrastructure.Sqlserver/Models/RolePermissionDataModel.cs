namespace Clinic.Infrastructure.Sqlserver.Models
{
    public class RolePermissionDataModel
    {
        public Guid RoleId { get; set; }
        public Guid PermissionId { get; set; }

        public RoleDataModel Role { get; set; }
        public PermissionDataModel Permission { get; set; }
    }
}
