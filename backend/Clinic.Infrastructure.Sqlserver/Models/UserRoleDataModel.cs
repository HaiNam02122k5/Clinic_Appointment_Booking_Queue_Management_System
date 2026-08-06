using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Infrastructure.Sqlserver.Models
{
    public class UserRoleDataModel
    {
        public Guid UserId { get; set; }
        public Guid RoleId { get; set; }

        // Navigation
        public UserDataModel User { get; set; }
        public RoleDataModel Role { get; set; }
    }
}
