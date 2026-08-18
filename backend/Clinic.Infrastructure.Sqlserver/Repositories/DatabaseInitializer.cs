using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using Clinic.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Sqlserver.Repositories
{
    public class DatabaseInitializer
    {
        private readonly ApplicationDbContext _context;
        private readonly IUserService _userService;
        private readonly IRoleRepository _roleRepository;
        private readonly IPersonService _personService;
        
        public DatabaseInitializer(ApplicationDbContext context, IUserService userService, IRoleRepository roleRepository, IPersonService personService)
        {
            _context = context;
            _userService = userService;
            _roleRepository = roleRepository;
            _personService = personService;
        }
        public async Task CreateInitialAdminAsync(string username, string password)
        {
            if (await _context.Users.Include(u => u.UserRoles).ThenInclude(ur => ur.Role).FirstOrDefaultAsync(u => u.UserRoles.Any(ur => ur.Role.Name == "Admin")) != null)
            {
                return;
            }
            var person = await _personService.CreateOrGetPersonAsync("Admin Name", "0379143825", "admin@thiscorp.com", new DateOnly(1990, 1, 1), Domain.Enums.Gender.Male, "123 Abc St.");
            var user = await _userService.CreateUserAsync(username, password, person);
            var employee = new Employee(person, new DateOnly(2025, 1, 1));

            var adminRole = await _roleRepository.GetByNameAsync("Admin");
            if (adminRole == null)
            {
                throw new InvalidOperationException("Admin role not found");
            }
            user.AssignRole(adminRole);
            await _context.AddAsync(employee);
            await _context.SaveChangesAsync();
        }
    }
}
