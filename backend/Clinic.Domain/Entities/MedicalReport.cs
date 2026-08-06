using Clinic.Domain.Common;
using System;

namespace Clinic.Domain.Entities
{
    /// <summary>
    /// Hồ sơ khám bệnh, gắn qua QueueTicketId (không phải AppointmentId) vì
    /// mọi ca khám đều phải đi qua hàng đợi trước khi bác sĩ ghi nhận kết quả.
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
    }
}
