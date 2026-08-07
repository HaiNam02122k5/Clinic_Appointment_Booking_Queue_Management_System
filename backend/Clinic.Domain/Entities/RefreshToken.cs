using Clinic.Domain.Common;
using System;

namespace Clinic.Domain.Entities
{
    /// <summary>
    /// Lưu refresh token dạng hash (không lưu token gốc) để có thể thu hồi
    /// (RevokedAt) khi đăng xuất, và phát hiện token bị đánh cắp nếu dùng lại
    /// một token đã bị thu hồi/hết hạn.
    /// </summary>
    public class RefreshToken : BaseEntity
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        /// <summary>Hash của refresh token, không lưu giá trị gốc.</summary>
        public string TokenHash { get; set; } = string.Empty;

        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAt { get; set; }

        /// <summary>NULL nếu token vẫn còn hiệu lực; có giá trị khi đã bị thu hồi (logout, refresh 1 lần...).</summary>
        public DateTime? RevokedAt { get; set; }
    }
}
