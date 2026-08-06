using Clinic.Domain.Entities;

namespace Clinic.Application.Interfaces
{
    public interface IRoleRepository
    {
        Task<Role?> GetByNameAsync(string roleName);
    }
}