using Clinic.Domain.Entities;

namespace Clinic.Application.Interfaces
{
    public interface IUserRepository
    {
        /// <summary>
        /// Gets a user by their username.
        /// </summary>
        Task<User?> GetByUsernameAsync(string username);

        /// <summary>
        /// Gets a user by their unique identifier (ID).
        /// </summary>
        Task<User?> GetByIdAsync(Guid? id);

        /// <summary>
        /// Gets all users in the system.
        /// </summary>
        Task<IEnumerable<User>> GetAllAsync();

        /// <summary>
        /// Adds a new user to the system.
        /// </summary>
        Task AddAsync(User user);

        /// <summary>
        /// Updates an existing user's information in the system.
        /// </summary>
        Task UpdateAsync(User user);

        /// <summary>
        /// Deletes a user from the system by their unique identifier (ID).
        /// </summary>
        Task DeleteAsync(Guid id);

        /// <summary>
        /// Gets a user by their associated person's unique identifier (PersonId).
        /// </summary>
        Task<User?> GetByPersonIdAsync(Guid id);
    }
}
