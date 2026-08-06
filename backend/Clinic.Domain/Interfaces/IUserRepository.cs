using Clinic.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Domain.Interfaces
{
    public interface IUserRepository
    {
        // ... Define method here
        Task<User?> GetByUsernameAsync(string username);
        Task<User?> GetByIdAsync(Guid id);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User> AddAsync(User user);
        Task<User> UpdateAsync(User user);
        Task<User> DeleteAsync(Guid id);
        Task<User?> GetByPersonIdAsync(Guid id);
    }
}
