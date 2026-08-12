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

        public AppointmentStatus Status { get; private set; } = AppointmentStatus.Pending;

        public bool IsWalkIn { get; set; } = false;

        /// <summary>0..1 - chỉ có sau khi bệnh nhân check-in.</summary>
        public QueueTicket? QueueTicket { get; set; }

        /// <summary>
        /// Hủy lịch hẹn. Không cho phép hủy lịch đã ở trạng thái kết thúc
        /// (Completed, Cancelled, NoShow).
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void Cancel()
        {
            if (Status is AppointmentStatus.Completed or AppointmentStatus.Cancelled or AppointmentStatus.NoShow)
            {
                throw new ArgumentException($"Cannot cancel an appointment with status '{Status}'.");
            }

            Status = AppointmentStatus.Cancelled;
            MarkUpdated();
        }
    }
}