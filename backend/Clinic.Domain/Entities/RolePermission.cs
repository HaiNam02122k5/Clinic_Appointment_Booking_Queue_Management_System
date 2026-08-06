using Clinic.Domain.Common;
using System;

namespace Clinic.Domain.Entities
{
    /// <summary>Bảng trung gian N-N giữa Role và Permission. Xem ghi chú ở UserRole.</summary>
    public class RolePermission : BaseEntity
    {
        public Guid RoleId { get; set; }
        public Role Role { get; set; } = null!;

        public Guid PermissionId { get; set; }
        public Permission Permission { get; set; } = null!;
    }
}
