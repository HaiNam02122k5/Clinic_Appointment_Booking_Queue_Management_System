using Clinic.Domain.Common;
using Clinic.Domain.Enums;
using System;

namespace Clinic.Domain.Entities
{
    /// <summary>
    /// Gắn theo PersonId (không phải UserId) để gửi được cho cả khách vãng lai
    /// chưa từng tạo tài khoản đăng nhập.
    /// </summary>
    public class Notification : BaseEntity
    {
        public Guid PersonId { get; set; }
        public Person Person { get; set; } = null!;

        public NotificationType Type { get; set; }

        public string Message { get; set; } = string.Empty;

        public NotificationChannel Channel { get; set; }

        public DateTime? SendTime { get; set; }

        public NotificationStatus Status { get; set; } = NotificationStatus.Pending;
    }
}
