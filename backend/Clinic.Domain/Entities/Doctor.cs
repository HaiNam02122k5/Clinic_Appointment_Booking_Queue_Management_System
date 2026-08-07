using Clinic.Domain.Common;
using Clinic.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Clinic.Domain.Entities
{
    public class Doctor : BaseEntity
    {
        /// <summary>FK, UNIQUE - mỗi Doctor gắn với đúng 1 Employee.</summary>
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        public string LicenseNumber { get; set; } = string.Empty;

        public int? ExperienceYears { get; set; }

        public string? Qualification { get; set; }

        public string? Biography { get; set; }
        public DoctorStatus Status { get; set; } = DoctorStatus.Active;

        public ICollection<WorkSchedule> WorkSchedules { get; set; } = new List<WorkSchedule>();

        public ICollection<WorkHistory> WorkHistories { get; set; } = new List<WorkHistory>();

        public ICollection<ShiftRequest> ShiftRequests { get; set; } = new List<ShiftRequest>();
    }
}
