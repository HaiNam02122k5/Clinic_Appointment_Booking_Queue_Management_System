using Clinic.Domain.Common;
using Clinic.Domain.Enums;
using System;
using System.Collections.Generic;

namespace Clinic.Domain.Entities
{
    /// <summary>
    /// Thông tin cá nhân dùng chung cho mọi actor.
    /// Không bắt buộc phải có User -> cho phép lưu hồ sơ khách vãng lai (walk-in)
    /// không cần tài khoản đăng nhập.
    /// </summary>
    public class Person : BaseEntity
    {
        public string FullName { get; set; } = string.Empty;

        /// <summary>UNIQUE. Dùng để tra cứu tránh tạo trùng hồ sơ khi khách vãng lai.</summary>
        public string PhoneNumber { get; set; } = string.Empty;

        public string? Email { get; set; }

        public DateOnly? DateOfBirth { get; set; }

        public Gender? Gender { get; set; }

        public string? Address { get; set; }

        // Navigation - quan hệ 0..1: không phải Person nào cũng có tài khoản/là nhân viên/là bệnh nhân
        public User? User { get; set; }

        public Employee? Employee { get; set; }

        public Patient? Patient { get; set; }

        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
