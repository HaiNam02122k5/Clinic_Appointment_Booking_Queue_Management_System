using Clinic.Domain.Entities;

namespace Clinic.Domain.Interfaces
{
    public interface IPersonRepository
    {
        Task<Person> AddAsync(Person person);
        Task<Person> UpdateAsync(Person person);
        Task<Person?> GetByEmailAsync(string email);
        Task<Person?> GetByPhoneNumberAsync(string phoneNumber);
    }
}