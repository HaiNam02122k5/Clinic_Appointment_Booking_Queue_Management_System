using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Domain.Entities
{
    public class UserRole
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }

        // Navigation
        public User User { get; set; }
        public Role Role { get; set; }

        public UserRole(Guid userId, Guid roleId)
        {
            UserId = userId;
            RoleId = roleId;
        }
    }
}
