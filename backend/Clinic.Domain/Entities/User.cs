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
        public ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
        public ICollection<RefreshToken> RefreshTokens { get; set; } = new List<RefreshToken>();

        public User(string username, string passwordHash, Guid personId)
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

        public User(Guid id, string username, string passwordHash, bool isActive, Guid personId, DateTime createdAt, DateTime? updatedAt, Person person, ICollection<UserRole> userRoles)
            : base(id, createdAt, updatedAt)
        {
            Username = username;
            PasswordHash = passwordHash;
            IsActive = isActive;
            PersonId = personId;
            Person = person;
            UserRoles = userRoles;
        }

        public void UpdatePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("New password hash cannot be null or empty.", nameof(newPasswordHash));
            PasswordHash = newPasswordHash;
            MarkUpdated();
        }

        public void ChangeStatus(bool isActive)
        {
            IsActive = isActive;
            MarkUpdated();
        }

        public void AssignRole(Role role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role), "Role cannot be null.");
            if (UserRoles.Where(ur => ur.RoleId == role.Id).Any())
            {
                throw new ArgumentException($"User already has the role '{role.Name}' assigned.");
            }
            UserRoles.Add(new UserRole(this.Id, role.Id));
            MarkUpdated();
        }
    }
}

