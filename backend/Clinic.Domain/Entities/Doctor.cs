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

        /// <summary>
        /// Nguồn duy nhất để biết chuyên khoa của bác sĩ (đã bỏ Doctor.SpecialtyId
        /// theo quyết định của nhóm - tránh 2 nơi lưu trùng thông tin dễ lệch dữ liệu).
        /// Chuyên khoa hiện tại = bản ghi có Status == WorkHistoryStatus.Active
        /// (tầng Service nên cung cấp helper method, ví dụ Doctor.GetCurrentSpecialty(),
        /// thay vì để Controller tự query LINQ mỗi lần).
        /// </summary>
        public ICollection<WorkHistory> WorkHistories { get; set; } = new List<WorkHistory>();

        public ICollection<ShiftRequest> ShiftRequests { get; set; } = new List<ShiftRequest>();
    }
}
