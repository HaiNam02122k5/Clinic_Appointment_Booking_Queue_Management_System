using Clinic.Domain.Common;
using Clinic.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Domain.Entities
{
    public class Person : BaseEntity
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string? Email { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public Gender Gender { get; set; }
        public string Address { get; set; }
        public bool IsDeleted { get; set; } = false;

        // Navigation
        public User? User { get; set; }
        public Person(string fullName, string phoneNumber, string? email, DateOnly dateOfBirth, Gender gender, string address)
        {
            FullName = fullName;
            PhoneNumber = phoneNumber;
            Email = email;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            Address = address;
            IsDeleted = false;
        }

        public Person(Guid id, string fullName, string phoneNumber, string? email, DateOnly dateOfBirth, Gender gender, string address, bool isDeleted, DateTime createdAt, DateTime? updatedAt)
            : base(id, createdAt, updatedAt)
        {
            FullName = fullName;
            PhoneNumber = phoneNumber;
            Email = email;
            DateOfBirth = dateOfBirth;
            Gender = gender;
            Address = address;
            IsDeleted = isDeleted;
        }
    }
}
