using Clinic.Domain.Common;
using System;
using System.Collections.Generic;

namespace Clinic.Domain.Entities
{
    public class Doctor : BaseEntity
    {
        /// <summary>FK, UNIQUE - mỗi Doctor gắn với đúng 1 Employee.</summary>
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;

        /// <summary>
        /// Chuyên khoa hiện tại - đặt trực tiếp trên Doctor để tra cứu nhanh
        /// (không phải đi qua bảng lịch sử chuyển khoa) vì đây là truy vấn được
        /// gọi liên tục ("tìm bác sĩ theo chuyên khoa").
        /// </summary>
        public Guid SpecialtyId { get; set; }
        public Specialty Specialty { get; set; } = null!;

        public string LicenseNumber { get; set; } = string.Empty;

        public int? ExperienceYears { get; set; }

        public string? Qualification { get; set; }

        public string? Biography { get; set; }

        public ICollection<WorkSchedule> WorkSchedules { get; set; } = new List<WorkSchedule>();
    }
}
