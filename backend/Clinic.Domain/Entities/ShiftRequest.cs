using Clinic.Domain.Common;
using Clinic.Domain.Enums;
using System;

namespace Clinic.Domain.Entities
{
    /// <summary>
    /// Yêu cầu ca làm việc do bác sĩ đề xuất, chờ Admin duyệt trước khi trở
    /// thành một WorkSchedule chính thức. Đây là tính năng mở rộng ngoài yêu
    /// cầu gốc của đề tài - cân nhắc mức độ ưu tiên so với phần lõi
    /// (Queue/Appointment) trước khi đầu tư nhiều thời gian vào luồng duyệt này.
    /// </summary>
    public class ShiftRequest : BaseEntity
    {
        public Guid DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        public DateTime ShiftStart { get; set; }

        public DateTime ShiftEnd { get; set; }

        public string? Reason { get; set; }

        public ShiftRequestStatus Status { get; set; } = ShiftRequestStatus.Pending;

    }
}
