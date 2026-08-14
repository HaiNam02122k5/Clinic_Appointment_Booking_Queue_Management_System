using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;

namespace Clinic.Application.UnitTests.Common
{
    public class FakeUserRepository : IUserRepository
    {
        private readonly List<User> _users = [];
        public async Task AddAsync(User user)
        {
            _users.Add(user);
        }

        public async Task DeleteAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<User?> GetByIdAsync(Guid? id)
        {
            return _users.FirstOrDefault(u => u.Id == id);
        }

        public async Task<User?> GetByPersonIdAsync(Guid id)
        {
            return _users.FirstOrDefault(u => u.PersonId == id);
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return _users.FirstOrDefault(u => u.Username == username);
        }

        public async Task UpdateAsync(User user)
        {
            return;
        }
    }
}
