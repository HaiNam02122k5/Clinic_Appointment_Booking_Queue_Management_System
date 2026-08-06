using Clinic.Domain.Common;

namespace Clinic.Domain.Entities
{
    public class Permission : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; }
    }
}
