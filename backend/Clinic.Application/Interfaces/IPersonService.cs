using Clinic.Domain.Entities;
using Clinic.Domain.Enums;

namespace Clinic.Application.Interfaces
{
    public interface IPersonService
    {
        Task<Person?> GetByPhoneNumberAsync(string phoneNumber);
        Task<Person?> GetByEmailAsync(string email);
        Task<Person?> GetByIdAsync(Guid id);
        Task<ICollection<Person>> GetAllAsync();

        /// <summary>
        /// Creates a new person profile or links to an existing one based on the provided phone number.
        /// If the phone number belongs to an existing person, but the email belongs to a different person, then throw an exception.
        /// Else, override the existing person's details with the provided information.
        /// </summary>
        /// <returns>The created or linked person entity.</returns>
        /// <exception cref="ArgumentException"></exception>
        Task<Person> CreateOrGetPersonAsync(string fullName, string phoneNumber, string email, DateOnly dateOfBirth, Gender gender, string address);
        Task UpdatePersonAsync(Guid id, string fullName, string phoneNumber, string email, DateOnly dateOfBirth, Gender gender, string address);
        Task DeletePersonAsync(Guid id);
    }
}
