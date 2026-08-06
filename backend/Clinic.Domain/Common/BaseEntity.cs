using System;

namespace Clinic.Domain.Common
{
    /// <summary>
    /// Lớp cơ sở cho mọi entity trong Domain.
    /// Cung cấp Id, thời gian tạo/cập nhật và soft-delete dùng chung.
    /// </summary>
    public abstract class BaseEntity
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime? UpdatedAt { get; set; }

        /// <summary>
        /// Soft-delete: API GET mặc định lọc IsDeleted = false.
        /// </summary>
        public bool IsDeleted { get; set; } = false;
    }
}
