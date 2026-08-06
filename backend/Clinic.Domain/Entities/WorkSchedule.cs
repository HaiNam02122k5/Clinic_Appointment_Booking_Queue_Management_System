using Clinic.Domain.Common;
using Clinic.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Clinic.Domain.Entities
{
    /// <summary>Một khung giờ làm việc của bác sĩ, giới hạn số bệnh nhân tối đa.</summary>
    public class WorkSchedule : BaseEntity
    {
        public Guid DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        public DateTime ShiftStart { get; set; }

        public DateTime ShiftEnd { get; set; }

        public int PatientLimit { get; set; }

        public WorkScheduleStatus Status { get; set; } = WorkScheduleStatus.Active;

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}
