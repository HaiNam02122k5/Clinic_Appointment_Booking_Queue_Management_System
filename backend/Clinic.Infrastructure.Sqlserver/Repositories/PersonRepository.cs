using Clinic.Domain.Entities;
using Clinic.Domain.Interfaces;
using Clinic.Infrastructure.Sqlserver.Models;
using Clinic.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Sqlserver.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public PersonRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<Person> AddAsync(Person person)
        {
            var model = MapToDataModel(person);
            await _dbContext.Persons.AddAsync(model);
            await _dbContext.SaveChangesAsync();
            return MapToDomain(model);
        }

        private Person? MapToDomain(PersonDataModel? model)
        {
            if (model == null)
            {
                return null;
            }

            return new Person(model.Id, model.FullName, model.PhoneNumber, model.Email, model.DateOfBirth, model.Gender, model.Address, model.IsDeleted, model.CreatedAt, model.UpdatedAt);
        }

        private PersonDataModel MapToDataModel(Person person)
        {
            UserDataModel? user = null;
            if (person.User != null)
            {
                user = new UserDataModel
                {
                    Id = person.User.Id,
                    Username = person.User.Username,
                    PasswordHash = person.User.PasswordHash,
                    CreatedAt = person.User.CreatedAt,
                    UpdatedAt = person.User.UpdatedAt,
                };
            }
            return new PersonDataModel
            {
                Id = person.Id,
                FullName = person.FullName,
                PhoneNumber = person.PhoneNumber,
                Email = person.Email,
                DateOfBirth = person.DateOfBirth,
                Gender = person.Gender,
                Address = person.Address,
                CreatedAt = person.CreatedAt,
                UpdatedAt = person.UpdatedAt,
                IsDeleted = person.IsDeleted,
                User = user
            };
        }

        public async Task<Person?> GetByEmailAsync(string email)
        {
            var model = await _dbContext.Persons.Include(p => p.User).FirstOrDefaultAsync(p => p.Email == email);
            return MapToDomain(model);
        }

        public async Task<Person?> GetByPhoneNumberAsync(string phoneNumber)
        {
            var model = await _dbContext.Persons.Include(p => p.User).FirstOrDefaultAsync(p => p.PhoneNumber == phoneNumber);
            return MapToDomain(model);
        }

        public async Task<Person> UpdateAsync(Person person)
        {
            var model = await _dbContext.Persons.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == person.Id);
            if (model == null)
            {
                throw new Exception($"Person with ID {person.Id} not found.");
            }
            model.Email = person.Email;
            model.Address = person.Address;
            model.DateOfBirth = person.DateOfBirth;
            model.Gender = person.Gender;
            model.IsDeleted = person.IsDeleted;
            await _dbContext.SaveChangesAsync();
            return MapToDomain(model);
        }
    }
}
