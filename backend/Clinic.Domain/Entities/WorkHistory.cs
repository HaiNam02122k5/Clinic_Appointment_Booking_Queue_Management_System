using Clinic.Domain.Common;
using Clinic.Domain.Enums;
using System;

namespace Clinic.Domain.Entities
{
    /// <summary>
    /// Sổ nhật ký lịch sử chuyển khoa của bác sĩ. Đây KHÔNG phải nguồn dữ liệu
    /// để tra "chuyên khoa hiện tại" - việc đó vẫn đọc trực tiếp từ
    /// Doctor.SpecialtyId (denormalize có chủ đích để tra cứu nhanh, vì
    /// "tìm bác sĩ theo chuyên khoa" là truy vấn được gọi liên tục).
    /// WorkHistory chỉ dùng để xem lại quá trình công tác qua các chuyên khoa.
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
