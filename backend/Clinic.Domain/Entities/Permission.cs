using Clinic.Domain.Common;
using System.Collections.Generic;

namespace Clinic.Domain.Entities
{
    public class Permission : BaseEntity
    {
        /// <summary>UNIQUE. Ví dụ: "appointment.cancel", "queue.call-next".</summary>
        public string Name { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }
}
