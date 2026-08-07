using Clinic.Domain.Entities;
using Clinic.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;
using Clinic.Application.Interfaces;

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
            await _dbContext.Persons.AddAsync(person);
            await _dbContext.SaveChangesAsync();
            var addedPerson = await _dbContext.Persons.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == person.Id);
            return addedPerson;
        }

        //private Person? MapToDomain(PersonDataModel? model)
        //{
        //    if (model == null)
        //    {
        //        return null;
        //    }

        //    return new Person(model.Id, model.FullName, model.PhoneNumber, model.Email, model.DateOfBirth, model.Gender, model.Address, model.IsDeleted, model.CreatedAt, model.UpdatedAt);
        //}

        //private PersonDataModel MapToDataModel(Person person)
        //{
        //    UserDataModel? user = null;
        //    if (person.User != null)
        //    {
        //        user = new UserDataModel
        //        {
        //            Id = person.User.Id,
        //            Username = person.User.Username,
        //            PasswordHash = person.User.PasswordHash,
        //            CreatedAt = person.User.CreatedAt,
        //            UpdatedAt = person.User.UpdatedAt,
        //        };
        //    }
        //    return new PersonDataModel
        //    {
        //        Id = person.Id,
        //        FullName = person.FullName,
        //        PhoneNumber = person.PhoneNumber,
        //        Email = person.Email,
        //        DateOfBirth = person.DateOfBirth,
        //        Gender = person.Gender,
        //        Address = person.Address,
        //        CreatedAt = person.CreatedAt,
        //        UpdatedAt = person.UpdatedAt,
        //        IsDeleted = person.IsDeleted,
        //        User = user
        //    };
        //}

        public async Task<Person?> GetByEmailAsync(string email)
        {
            return await _dbContext.Persons.Include(p => p.User).FirstOrDefaultAsync(p => p.Email == email);
        }

        public async Task<Person?> GetByPhoneNumberAsync(string phoneNumber)
        {
            return await _dbContext.Persons.Include(p => p.User).FirstOrDefaultAsync(p => p.PhoneNumber == phoneNumber);
        }

        public async Task<Person?> GetByIdAsync(Guid id)
        {
            return await _dbContext.Persons.Include(p => p.User).FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task DeleteAsync(Guid id)
        {
            var person = await _dbContext.Persons.FindAsync(id);
            if (person != null)
            {
                person.Delete();
                _dbContext.Persons.Update(person);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException($"Person with id {id} not found.");
            }
        }
        public async Task UpdateAsync(Person person)
        {
            _dbContext.Persons.Update(person);
            await _dbContext.SaveChangesAsync();
        }
    }
}
