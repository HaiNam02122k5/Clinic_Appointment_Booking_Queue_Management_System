using Clinic.Domain.Common;
using Clinic.Domain.Enums;
using System;

namespace Clinic.Domain.Entities
{
    /// <summary>
    /// Sổ nhật ký lịch sử chuyển khoa của bác sĩ
    /// </summary>
    public class WorkHistory : BaseEntity
    {
        public Guid DoctorId { get; set; }
        public Doctor Doctor { get; set; } = null!;

        public Guid SpecialtyId { get; set; }
        public Specialty Specialty { get; set; } = null!;

        public DateOnly StartDate { get; set; }

        /// <summary>NULL nếu đây là giai đoạn hiện tại (chưa kết thúc).</summary>
        public DateOnly? EndDate { get; set; }

        public WorkHistoryStatus Status { get; set; } = WorkHistoryStatus.Active;
    }
}
