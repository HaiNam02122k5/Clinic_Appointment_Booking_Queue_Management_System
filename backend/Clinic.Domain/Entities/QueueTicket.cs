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

        public bool Priority { get; set; } = false;

        public QueueStatus Status { get; set; } = QueueStatus.Waiting;

        /// <summary>Thời điểm bệnh nhân check-in, sinh số thứ tự.</summary>
        public DateTime CheckInTime { get; set; } = DateTime.UtcNow;

        public DateTime? CalledAt { get; set; }

        /// <summary>0..1 - chỉ có sau khi bác sĩ khám xong.</summary>
        public MedicalReport? MedicalReport { get; set; }
    }
}
