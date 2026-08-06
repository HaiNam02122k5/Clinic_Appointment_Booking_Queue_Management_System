using Clinic.Domain.Entities;
using Clinic.Domain.Interfaces;
using Clinic.Infrastructure.Sqlserver.Persistence;

namespace Clinic.Infrastructure.Sqlserver.Repositories
{
    public class PersonRepository : IPersonRepository
    {
        private readonly ApplicationDbContext _dbContext;

        public PersonRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Task<Person> AddAsync(Person person)
        {
            throw new NotImplementedException();
        }

        public Task<Person?> GetByEmailAsync(string email)
        {
            throw new NotImplementedException();
        }

        public Task<Person?> GetByPhoneNumberAsync(string phoneNumber)
        {
            throw new NotImplementedException();
        }

        public Task<Person> UpdateAsync(Person person)
        {
            throw new NotImplementedException();
        }
    }
}
