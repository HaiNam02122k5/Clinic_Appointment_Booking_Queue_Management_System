using Clinic.Domain.Entities;

namespace Clinic.Application.Interfaces
{
    public interface IUserService
    {
        /// <summary>
        /// Creates a new user with the specified username, password, and associated person ID.
        /// </summary>
        /// <returns></returns>
        Task<User> CreateUserAsync(string username, string password, Guid personId);

        /// <summary>
        /// Gets a user by their username. Returns null if the user does not exist.
        /// </summary>
        /// <returns></returns>
        Task<User?> GetByIdAsync(Guid id);

        /// <summary>
        /// Verifies the user's credentials and returns the user if valid, otherwise returns null.
        /// </summary>
        /// <returns></returns>
        Task<User?> VerifyUser(string username, string password);
    }
}
