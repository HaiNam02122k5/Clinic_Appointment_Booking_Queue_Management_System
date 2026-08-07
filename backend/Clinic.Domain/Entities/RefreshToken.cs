using Clinic.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Domain.Entities
{
    public class RefreshToken
    {
        public Guid Id { get; init; }
        public string TokenHash { get; init; }
        public DateTime IssuedAt { get; init; } = DateTime.UtcNow;
        public DateTime ExpiresAt { get; init; }
        public DateTime? RevokedAt { get; protected set; }
        public Guid UserId { get; protected set; }

        // Navigation
        public User User { get; protected set; }

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
