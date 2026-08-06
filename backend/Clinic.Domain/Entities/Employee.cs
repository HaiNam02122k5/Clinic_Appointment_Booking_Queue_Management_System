using Clinic.Domain.Common;
using Clinic.Domain.Enums;
using System;

namespace Clinic.Domain.Entities
{
    /// <summary>
    /// Thông tin nghiệp vụ của nhân viên (Doctor/Receptionist/Admin).
    /// Vai trò cụ thể được xác định qua Role của User, không phải qua subtype riêng
    /// (đã bỏ bảng Admin/Receptionist thừa vì không có thuộc tính riêng).
    /// </summary>
    public class Employee : BaseEntity
    {
        /// <summary>FK, UNIQUE.</summary>
        public Guid PersonId { get; set; }
        public Person Person { get; set; } = null!;

        public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

        public DateOnly HireDate { get; set; }

        /// <summary>Tự tham chiếu, optional - quản lý cấp trên (nếu nhóm có dùng tính năng này).</summary>
        public Guid? ManagerId { get; set; }
        public Employee? Manager { get; set; }

        /// <summary>0..1 - chỉ có giá trị nếu nhân viên này là bác sĩ.</summary>
        public Doctor? Doctor { get; set; }
    }
}
