using Clinic.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Domain.Entities
{
    public class Permission : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }

        public List<RolePermission> RolePermissions { get; set; }
    }
}
