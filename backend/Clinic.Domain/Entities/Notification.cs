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
        public Guid PersonId { get; protected set; }
        public Person Person { get; protected set; } = null!;

        public NotificationType Type { get; protected set; }

        public string Message { get; protected set; } = string.Empty;

        public bool? IsRead { get; protected set; } = null;

        public NotificationChannel Channel { get; protected set; }

        public DateTime? SendTime { get; protected set; }

        public NotificationStatus Status { get; protected set; } = NotificationStatus.Pending;

        public Notification(Guid personId, NotificationType type, string message, NotificationChannel channel)
        {
            PersonId = personId;
            Type = type;
            Message = message;
            Channel = channel;
        }

        /// Private constructor for EF Core
        private Notification() { }

        public void MarkAsRead()
        {
            IsRead = true;
        }

        public void MarkAsSent()
        {
            Status = NotificationStatus.Sent;
            SendTime = DateTime.UtcNow;
        }

        public void MarkAsFailed()
        {
            Status = NotificationStatus.Failed;
        }
    }
}
