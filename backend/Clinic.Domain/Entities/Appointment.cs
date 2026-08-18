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
        /// Xác nhận lịch hẹn. Chỉ cho phép xác nhận khi đang ở trạng thái Pending
        /// (lịch mới đặt, chưa được lễ tân/hệ thống xác nhận).
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void Confirm()
        {
            if (Status != AppointmentStatus.Pending)
            {
                throw new ArgumentException($"Cannot confirm an appointment with status '{Status}'.");
            }

            Status = AppointmentStatus.Confirmed;
            MarkUpdated();
        }

        /// <summary>
        /// Check-in bệnh nhân tại quầy/vào hàng đợi.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void CheckIn()
        {
            if (Status != AppointmentStatus.Confirmed)
            {
                throw new ArgumentException($"Cannot check in an appointment with status '{Status}'. Appointment must be confirmed first.");
            }

            Status = AppointmentStatus.CheckedIn;
            MarkUpdated();
        }

        /// <summary>
        /// Hủy lịch hẹn. Không cho phép hủy lịch đã ở trạng thái kết thúc
        /// (Completed, Cancelled, NoShow).
        ///
        /// Trường hợp Status = CheckedIn: bệnh nhân đã check-in nên chắc chắn có 1 QueueTicket
        /// đi kèm. Để tránh việc Appointment chuyển sang Cancelled trong khi QueueTicket vẫn
        /// còn Waiting/Called/InProgress (khiến lễ tân/bác sĩ vẫn gọi một bệnh nhân "ảo" đã hủy),
        /// việc hủy được xử lý như sau:
        /// - Vé đang Waiting hoặc Called (bệnh nhân chưa vào phòng khám): hủy luôn vé kèm theo,
        ///   để hàng đợi phản ánh đúng thực tế.
        /// - Vé đang InProgress (bác sĩ đang khám dở) hoặc Completed (đã khám xong): hủy lúc này
        ///   không còn ý nghĩa và sẽ để lại dữ liệu không nhất quán -> chặn hẳn, ném ArgumentException.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// Trạng thái hiện tại không cho phép hủy (đã kết thúc, hoặc đang/đã khám xong).
        /// </exception>
        /// <exception cref="InvalidOperationException">
        /// Status = CheckedIn nhưng QueueTicket chưa được load/gán - vi phạm bất biến của
        /// entity (mọi Appointment CheckedIn phải có đúng 1 QueueTicket). Đây là lỗi lập trình/
        /// truy vấn dữ liệu (thiếu Include), không phải lỗi nghiệp vụ của người dùng.
        /// </exception>
        public void Cancel()
        {
            if (Status is AppointmentStatus.Completed or AppointmentStatus.Cancelled or AppointmentStatus.NoShow)
            {
                throw new ArgumentException($"Cannot cancel an appointment with status '{Status}'.");
            }

            if (Status == AppointmentStatus.CheckedIn)
            {
                if (QueueTicket is null)
                {
                    throw new InvalidOperationException(
                        "Appointment is CheckedIn but its QueueTicket is not loaded. " +
                        "Make sure the repository includes QueueTicket before calling Cancel().");
                }

                if (QueueTicket.Status is QueueStatus.InProgress or QueueStatus.Completed)
                {
                    throw new ArgumentException(
                        $"Cannot cancel an appointment whose queue ticket is '{QueueTicket.Status}'.");
                }

                // Vé còn Waiting/Called -> hủy đồng thời để hàng đợi không còn giữ vé "ảo".
                QueueTicket.Cancel();
            }

            Status = AppointmentStatus.Cancelled;
            MarkUpdated();
        }

        /// <summary>
        /// Đổi lịch hẹn sang thời điểm mới. Không cho phép đổi lịch đã ở trạng thái kết thúc
        /// (Completed, Cancelled, NoShow) hoặc đã CheckedIn (bệnh nhân đã có mặt, không còn ý nghĩa đổi lịch).
        /// Lưu ý: rule "phải đổi trước hạn X giờ" phụ thuộc vào actor
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