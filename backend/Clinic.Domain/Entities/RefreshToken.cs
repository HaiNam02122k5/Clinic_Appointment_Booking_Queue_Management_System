using Clinic.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Domain.Entities
{
    public class RefreshToken : BaseEntity
    {
        public string TokenHash { get; init; }
        public DateTime ExpiresAt { get; init; }
        public DateTime? RevokedAt { get; set; }
        public Guid UserId { get; set; }

        // Navigation
        public User User { get; set; }
    }

}
