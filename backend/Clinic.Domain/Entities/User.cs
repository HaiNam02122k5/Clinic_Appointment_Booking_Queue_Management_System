using Clinic.Domain.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Domain.Entities
{
    public class User : BaseEntity
    {
        public string Username { get; init; }
        public string PasswordHash { get; protected set; }
        public bool IsActive { get; protected set; } = true;
        public Guid PersonId { get; protected set; }

        // Navigation
        public Person Person { get; protected set; }
        public ICollection<UserRole> UserRoles { get; protected set; } = [];
        public ICollection<RefreshToken> RefreshTokens { get; protected set; } = [];

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

        /// <summary>
        /// Updates the user's password hash.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public void UpdatePassword(string newPasswordHash)
        {
            if (string.IsNullOrWhiteSpace(newPasswordHash))
                throw new ArgumentException("New password hash cannot be null or empty.", nameof(newPasswordHash));
            PasswordHash = newPasswordHash;
            MarkUpdated();
        }

        /// <summary>
        /// Changes the user's active status.
        /// </summary>
        public void ChangeStatus(bool isActive)
        {
            IsActive = isActive;
            MarkUpdated();
        }

        /// <summary>
        /// Assigns a role to the user. If the user already has the role, an exception is thrown.
        /// </summary>
        /// <param name="role"></param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
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

