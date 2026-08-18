using Clinic.Domain.Common;
using Clinic.Domain.Enums;
using System;

namespace Clinic.Domain.Entities
{
    /// <summary>
    /// Hồ sơ khám bệnh
    /// </summary>
    public class MedicalReport : BaseEntity
    {
        /// <summary>FK - đường dẫn duy nhất tới Doctor là QueueTicket.Appointment.WorkSchedule.Doctor.</summary>
        public Guid QueueTicketId { get; set; }
        public QueueTicket QueueTicket { get; set; } = null!;

        public string? Symptoms { get; set; }

        public string? Diagnosis { get; set; }

        public string? Prescription { get; set; }

        public string? Notes { get; set; }

        /// <summary>Thời điểm bác sĩ bắt đầu khám (khi bấm "bắt đầu" trên UI, khác với QueueTicket.CalledAt là lúc gọi số).</summary>
        public DateTime? ExamStartTime { get; set; }

        /// <summary>Thời điểm khám xong - dùng để tính thời gian khám thực tế cho báo cáo hiệu suất bác sĩ.</summary>
        public DateTime? ExamEndTime { get; set; }

        /// <summary>Draft trong lúc bác sĩ đang ghi, Finalized khi đã chốt (không cho sửa nữa qua PUT /medical-reports/{id}).</summary>
        public MedicalReportStatus Status { get; set; } = MedicalReportStatus.Draft;
    }
}
