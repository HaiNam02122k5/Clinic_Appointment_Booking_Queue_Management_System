using Clinic.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid PersonId { get; set; }

        // Navigation
        public Person Person { get; set; }
        public Role Role { get; set; }
        public ICollection<RefreshToken> RefreshTokens { get; set; }

        public User(string  username, string passwordHash, Guid personId)
        {
            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be null or empty.", nameof(username));

            if (string.IsNullOrWhiteSpace(passwordHash))
                throw new ArgumentException("Password hash cannot be null or empty.", nameof(passwordHash));

            Username = username;
            PasswordHash = passwordHash;
            PersonId = personId;
            IsActive = true;
        }

        public User(Guid id, string username, string passwordHash, bool isActive, Guid personId, DateTime createdAt, DateTime? updatedAt, Person person)
            : base(id, createdAt, updatedAt)
        {
            Username = username;
            PasswordHash = passwordHash;
            IsActive = isActive;
            PersonId = personId;
            Person = person;
        }
    }
}
