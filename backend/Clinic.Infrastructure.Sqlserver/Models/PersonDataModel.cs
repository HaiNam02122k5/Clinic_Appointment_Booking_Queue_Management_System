using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Infrastructure.Sqlserver.Models
{
    public class PersonDataModel
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string? Email { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public int Gender { get; set; }
        public string Address { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public bool IsDeleted { get; set; }
        public UserDataModel? User { get; set; }
    }
}
