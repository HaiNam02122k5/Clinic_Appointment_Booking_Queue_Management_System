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

        /// <summary>
        /// Đổi lịch hẹn sang thời điểm mới. Không cho phép đổi lịch đã ở trạng thái kết thúc
        /// (Completed, Cancelled, NoShow) hoặc đã CheckedIn (bệnh nhân đã có mặt, không còn ý nghĩa đổi lịch).
        /// Lưu ý: rule "phải đổi trước hạn X giờ" phụ thuộc vào actor (Patient bị ràng buộc, Admin/Receptionist thì không)
        /// nên được kiểm tra ở Application layer (RescheduleAppointmentHandler), không đặt ở đây.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void Reschedule(DateTime newTimeSlot)
        {
            if (Status is AppointmentStatus.Completed or AppointmentStatus.Cancelled or AppointmentStatus.NoShow or AppointmentStatus.CheckedIn)
            {
                throw new ArgumentException($"Cannot reschedule an appointment with status '{Status}'.");
            }

            TimeSlot = newTimeSlot;
            // Lịch đổi giờ cần bác sĩ/lễ tân xác nhận lại, không giữ nguyên trạng thái Confirmed cũ.
            Status = AppointmentStatus.Pending;
            MarkUpdated();
        }
    }
}