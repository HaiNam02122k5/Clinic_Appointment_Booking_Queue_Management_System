using Clinic.Domain.Common;

namespace Clinic.Domain.Entities
{
    public class Role : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation
        public ICollection<UserRole> UserRoles { get; set; }
        public ICollection<RolePermission> RolePermissions { get; set; }

        private Role() { }
        public Role(Guid id, string name, string description, DateTime createdAt, DateTime updatedAt, bool isDeleted) : base(id, createdAt, updatedAt)
        {
            this.Name = name;
            this.Description = description;
            this.IsDeleted = isDeleted;
        }
    }
}
