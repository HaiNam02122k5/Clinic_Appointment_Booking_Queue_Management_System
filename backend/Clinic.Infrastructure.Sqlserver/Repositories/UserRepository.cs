using Clinic.Domain.Entities;
using Clinic.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;
using Clinic.Application.Interfaces;

namespace Clinic.Infrastructure.Sqlserver.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> AddAsync(User user)
        {
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            var addedUser = await _context.Users
                .Include(u => u.Person)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == user.Id);
            return addedUser;
        }

        public async Task DeleteAsync(Guid id)
        {
            var model = await _context.Users.FindAsync(id);
            if (model == null)
            {
                throw new InvalidOperationException("User not found");
            }
            _context.Users.Remove(model);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users
                .AsNoTracking()
                .Include(u => u.Person)
                .ToListAsync();
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            return await _context.Users.
                Include(u => u.Person)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            return await _context.Users
                .Include(u => u.Person)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        //private User? MapToDomain(UserDataModel? data)
        //{
        //    if (data == null)
        //    {
        //        return null;
        //    }

        //    Person person = new Person(
        //        id: data.Person.Id,
        //        fullName: data.Person.FullName,
        //        email: data.Person.Email,
        //        phoneNumber: data.Person.PhoneNumber,
        //        dateOfBirth: data.Person.DateOfBirth,
        //        gender: data.Person.Gender,
        //        address: data.Person.Address,
        //        createdAt: data.Person.CreatedAt,
        //        updatedAt: data.Person.UpdatedAt,
        //        isDeleted: data.Person.IsDeleted
        //    );


        //    return new User(
        //        id: data.Id,
        //        username: data.Username,
        //        passwordHash: data.PasswordHash,
        //        isActive: data.IsActive,
        //        personId: data.PersonId,
        //        createdAt: data.CreatedAt,
        //        updatedAt: data.UpdatedAt,
        //        person: person,
        //        userRoles: data.UserRoles.Select(ur => new UserRole(
        //            userId: ur.UserId,
        //            roleId: ur.RoleId
        //        )).ToList()
        //    );
        //}

        //private UserDataModel MapToDataModel(User user)
        //{
        //    return new UserDataModel
        //    {
        //        Id = user.Id,
        //        Username = user.Username,
        //        PasswordHash = user.PasswordHash,
        //        IsActive = user.IsActive,
        //        PersonId = user.PersonId,
        //        CreatedAt = user.CreatedAt,
        //        UpdatedAt = user.UpdatedAt
        //    };
        //}

        public async Task<User?> GetByPersonIdAsync(Guid id)
        {
            return await _context.Users
                .Include(u => u.Person)
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .FirstOrDefaultAsync(u => u.PersonId == id);
        }
    }
}
