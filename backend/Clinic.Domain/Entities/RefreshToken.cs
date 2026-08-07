using Clinic.Domain.Common;
using System;

namespace Clinic.Domain.Entities
{
    /// <summary>
    /// Lưu refresh token dạng hash (không lưu token gốc) để có thể thu hồi
    /// (RevokedAt) khi đăng xuất, và phát hiện token bị đánh cắp nếu dùng lại
    /// một token đã bị thu hồi/hết hạn.
    /// </summary>
    public class RefreshToken
    {
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public Guid Id { get; init; }

        /// <summary>Hash của refresh token, không lưu giá trị gốc.</summary>
        public string TokenHash { get; set; } = string.Empty;

        public DateTime IssuedAt { get; set; } = DateTime.UtcNow;

        public DateTime ExpiresAt { get; set; }

        /// <summary>NULL nếu token vẫn còn hiệu lực; có giá trị khi đã bị thu hồi (logout, refresh 1 lần...).</summary>
        public DateTime? RevokedAt { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="RefreshToken"/> class.
        /// </summary>
        public RefreshToken(string tokenHash, DateTime expiresAt, Guid userId)
        {
            Id = Guid.NewGuid();
            TokenHash = tokenHash;
            IssuedAt = DateTime.UtcNow;
            ExpiresAt = expiresAt;
            UserId = userId;
        }

        /// <summary>
        /// Rebuild the RefreshToken entity from the database
        /// </summary>
        public RefreshToken(Guid id, string tokenHash, DateTime issuedAt, DateTime expiresAt, DateTime? revokedAt, Guid userId)
        {
            Id = id;
            TokenHash = tokenHash;
            IssuedAt = issuedAt;
            ExpiresAt = expiresAt;
            RevokedAt = revokedAt;
            UserId = userId;
        }

        /// <summary>
        /// Revokes the refresh token 
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        public void Revoke()
        {
            if (RevokedAt != null)
            {
                throw new InvalidOperationException("This token has already revoked");
            }
            RevokedAt = DateTime.UtcNow;
        }
    }
}
