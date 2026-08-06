using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Infrastructure.Sqlserver.Models
{
    public class UserDataModel
    {
        public Guid Id { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public Guid PersonId { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public PersonDataModel Person { get; set; }
        public ICollection<RefreshTokenDataModel> RefreshTokens { get; set; }
        public ICollection<UserRoleDataModel> UserRoles { get; set; }
    }
}
