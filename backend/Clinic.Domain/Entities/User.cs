using Clinic.Domain.Common;
using System;
using System.Collections.Generic;

namespace Clinic.Domain.Entities
{
    /// <summary>
    /// Tài khoản đăng nhập. Không phải mọi Person đều có User (VD: khách vãng lai).
    /// </summary>
    public class User : BaseEntity
    {
        /// <summary>FK, UNIQUE - 1 Person chỉ có tối đa 1 tài khoản.</summary>
        public Guid PersonId { get; set; }
        public Person Person { get; set; } = null!;

        /// <summary>UNIQUE.</summary>
        public string Username { get; set; } = string.Empty;

        public string PasswordHash { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();

        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();
    }
}
