using Clinic.Domain.Common;
using Clinic.Domain.Enums;
using System;

namespace Clinic.Domain.Entities
{
    public class Appointment : BaseEntity
    {
        public Guid PatientId { get; set; }
        public Patient Patient { get; set; } = null!;

        public Guid WorkScheduleId { get; set; }
        public WorkSchedule WorkSchedule { get; set; } = null!;

        public string? Reason { get; set; }

        public DateTime TimeSlot { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;

        /// <summary>
        /// True nếu lịch hẹn do Lễ tân tạo tại quầy cho khách vãng lai (Cách B).
        /// Dùng để loại các bản ghi này ra khỏi thống kê tỷ lệ hủy lịch / hành vi
        /// đặt lịch trước, vì chúng không phản ánh hành vi đặt lịch online thật.
        /// </summary>
        public bool IsWalkIn { get; set; } = false;

        /// <summary>0..1 - chỉ có sau khi bệnh nhân check-in.</summary>
        public QueueTicket? QueueTicket { get; set; }
    }
}
