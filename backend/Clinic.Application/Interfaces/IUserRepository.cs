using Clinic.Domain.Entities;

namespace Clinic.Application.Interfaces
{
    public interface IUserRepository
    {
        // ... Define method here
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(Guid id);
        Task<IEnumerable<User>> GetAllAsync();
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeleteAsync(Guid id);
        Task<User?> GetByPersonIdAsync(Guid id);
    }
}
