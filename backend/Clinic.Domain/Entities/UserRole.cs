using Clinic.Domain.Common;
using System;

namespace Clinic.Domain.Entities
{
    /// <summary>
    /// Bảng trung gian N-N giữa User và Role.
    /// Dùng Id riêng (thay vì khóa chính kép UserId+RoleId) để đơn giản hóa
    /// truy vấn/EF Core; ràng buộc duy nhất (UserId, RoleId) được cấu hình
    /// bằng Fluent API ở tầng Infrastructure (unique index), không phải PK kép.
    /// </summary>
    public class UserRole : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid RoleId { get; set; }
        public Role Role { get; set; } = null!;
    }
}
