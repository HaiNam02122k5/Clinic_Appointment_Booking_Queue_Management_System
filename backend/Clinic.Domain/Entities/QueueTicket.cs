using Clinic.Domain.Common;
using Clinic.Domain.Enums;
using System;

namespace Clinic.Domain.Entities
{
    /// <summary>
    /// Số thứ tự hàng đợi khám bệnh.
    /// </summary>
    public class QueueTicket : BaseEntity
    {
        /// <summary>FK, NOT NULL, UNIQUE - 1 Appointment chỉ sinh tối đa 1 QueueTicket.</summary>
        public Guid AppointmentId { get; set; }
        public Appointment Appointment { get; set; } = null!;

        public int QueueNumber { get; set; }

        public bool Priority { get; private set; } = false;

        public QueueStatus Status { get; private set; } = QueueStatus.Waiting;

        /// <summary>Thời điểm bệnh nhân check-in, sinh số thứ tự.</summary>
        public DateTime CheckInTime { get; set; } = DateTime.UtcNow;

        public DateTime? CalledAt { get; set; }

        /// <summary>0..1 - chỉ có sau khi bác sĩ khám xong.</summary>
        public MedicalReport? MedicalReport { get; set; }

        /// <summary>
        /// Gọi bệnh nhân vào khám. Chỉ cho phép gọi khi đang ở trạng thái Waiting
        /// (chưa được gọi lần nào, hoặc đã reset lại từ 1 lượt gọi trước).
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void Call()
        {
            if (Status != QueueStatus.Waiting)
            {
                throw new ArgumentException($"Cannot call a queue ticket with status '{Status}'.");
            }

            Status = QueueStatus.Called;
            CalledAt = DateTime.UtcNow;
            MarkUpdated();
        }

        /// <summary>
        /// Bỏ qua lượt khám (bệnh nhân không có mặt khi được gọi, hoặc lễ tân chủ động bỏ qua
        /// trong khi vẫn đang chờ). Không cho phép bỏ qua khi đã khám xong hoặc đã bị bỏ qua trước đó.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void Skip()
        {
            if (Status is QueueStatus.Completed or QueueStatus.Skipped)
            {
                throw new ArgumentException($"Cannot skip a queue ticket with status '{Status}'.");
            }

            Status = QueueStatus.Skipped;
            MarkUpdated();
        }

        /// <summary>
        /// Đánh dấu (hoặc gỡ đánh dấu) ưu tiên khẩn cấp. Chỉ áp dụng khi vé còn đang chờ (Waiting) -
        /// ưu tiên chỉ có ý nghĩa thay đổi thứ tự của lượt gọi tiếp theo, không còn tác dụng
        /// khi đã được gọi/đang khám/khám xong/bị bỏ qua.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void SetPriority(bool priority)
        {
            if (Status != QueueStatus.Waiting)
            {
                throw new ArgumentException($"Cannot change priority of a queue ticket with status '{Status}'.");
            }

            Priority = priority;
            MarkUpdated();
        }
    }
}