using Clinic.Domain.Entities;
using Clinic.Domain.Interfaces;
using Clinic.Infrastructure.Sqlserver.Models;
using Clinic.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

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
            var model = MapToDataModel(user);
            await _context.Users.AddAsync(model);
            await _context.SaveChangesAsync();
            return MapToDomain(model);
        }

        public async Task<User> DeleteAsync(Guid id)
        {
            var model = await _context.Users.FindAsync(id);
            if (model == null)
            {
                throw new InvalidOperationException("User not found");
            }
            _context.Users.Remove(model);
            await _context.SaveChangesAsync();
            return MapToDomain(model);
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            var data = await _context.Users.AsNoTracking().Include(u => u.Person).ToListAsync();
            return data.Select(MapToDomain).Where(u => u != null).ToList();
        }

        public async Task<User?> GetByUsernameAsync(string username)
        {
            var data = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
            return MapToDomain(data);
        }

        public async Task<User?> GetByIdAsync(Guid id)
        {
            var data = await _context.Users.FindAsync(id);
            return MapToDomain(data);
        }

        public async Task<User> UpdateAsync(User user)
        {
            var model = await _context.Users.Include(u => u.Person).FirstOrDefaultAsync(u => u.Id == user.Id);
            if (model == null)
            {
                throw new InvalidOperationException("User not found");
            }

            // Update the properties of the existing model with the new values
            model.Username = user.Username;
            model.PasswordHash = user.PasswordHash;
            model.IsActive = user.IsActive;
            model.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToDomain(model);
        }

        private User MapToDomain(UserDataModel? data)
        {
            if (data == null)
            {
                return null;
            }

            Person person = new Person(
                id: data.Person.Id,
                fullName: data.Person.FullName,
                email: data.Person.Email,
                phoneNumber: data.Person.PhoneNumber,
                dateOfBirth: data.Person.DateOfBirth,
                gender: data.Person.Gender,
                address: data.Person.Address,
                createdAt: data.Person.CreatedAt,
                updatedAt: data.Person.UpdatedAt,
                isDeleted: data.Person.IsDeleted
            );


            return new User(
                id: data.Id,
                username: data.Username,
                passwordHash: data.PasswordHash,
                isActive: data.IsActive,
                personId: data.PersonId,
                createdAt: data.CreatedAt,
                updatedAt: data.UpdatedAt,
                person: person,
                userRoles: data.UserRoles.Select(ur => new UserRole(
                    userId: ur.UserId,
                    roleId: ur.RoleId
                )).ToList()
            );
        }

        private UserDataModel MapToDataModel(User user)
        {
            return new UserDataModel
            {
                Id = user.Id,
                Username = user.Username,
                PasswordHash = user.PasswordHash,
                IsActive = user.IsActive,
                PersonId = user.PersonId,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt
            };
        }
    }
}
