using Clinic.Domain.Common;
using Clinic.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Domain.Entities
{
    public class Person : BaseEntity
    {
        public string FullName { get; init; }
        public string PhoneNumber { get; init; }
        public string? Email { get; protected set; }
        public DateOnly DateOfBirth { get; init; }
        public Gender Gender { get; protected set; }
        public string Address { get; protected set; }
        public bool IsDeleted { get; protected set; } = false;

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

        /// <summary>
        /// Updates the details of the person. Limited to email, gender, and address.
        /// </summary>
        public void UpdateDetails(string? email, Gender gender, string address)
        {
            Email = email;
            Gender = gender;
            Address = address;
            MarkUpdated();
        }

        public void Delete()
        {
            IsDeleted = true;
            MarkUpdated();
        }
    }
}
