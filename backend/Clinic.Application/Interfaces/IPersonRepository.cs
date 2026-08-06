using Clinic.Domain.Entities;

namespace Clinic.Application.Interfaces
{
    public interface IPersonRepository
    {
        Task<Person> AddAsync(Person person);
        Task UpdateAsync(Person person);
        Task<Person?> GetByEmailAsync(string email);
        Task<Person?> GetByPhoneNumberAsync(string phoneNumber);
        Task<Person?> GetByIdAsync(Guid id);
        Task DeleteAsync(Guid id);
    }
}