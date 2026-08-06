using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;

namespace Clinic.Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly IPasswordHasher _passwordHasher;

        public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher)
        {
            _userRepository = userRepository;
            _passwordHasher = passwordHasher;
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _userRepository.GetByIdAsync(id);
        }

        public async Task<User> CreateUserAsync(string username, string password, Guid personId)
        {
            var hashedPassword = _passwordHasher.HashPassword(password);
            var user = new User(username, hashedPassword, personId);
            await _userRepository.AddAsync(user);
            return user;
        }

        public async Task<User?> VerifyUser(string username, string password)
        {
            var user = await _userRepository.GetByUsernameAsync(username);
            if (user == null
                || !_passwordHasher.VerifyPassword(password, user.PasswordHash))
            {
                throw new ArgumentException("Username or password is incorrect.");
            }

            return user;
        }
    }
}
