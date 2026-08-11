using Clinic.Application.Common.Models;
using Clinic.Domain.Entities;
using Clinic.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Sqlserver.Repositories
{
    public class EmployeeRepository
    {
        private readonly ApplicationDbContext _context;
        public EmployeeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Employee employee)
        {
            _context.Employees.Add(employee);
        }

        public async Task<Employee?> GetByPersonIdAsync(Guid personId)
        {
            return await _context.Employees.FirstOrDefaultAsync(e => e.PersonId == personId && e.IsDeleted == false);
        }

        public async Task<PagedResult<Employee>> GetPagedAsync(string? searchTerm, string sortBy, bool sortDescending, int page, int pageSize)
        {
            var query = _context.Employees.Where(e => e.IsDeleted == false);

            // TODO: Implement filter
            //if (!string.IsNullOrEmpty(searchTerm))
            //{
            //    query = query.Where(s => s.Name.Contains(searchTerm) || s.Description.Contains(searchTerm));
            //}

            //query = sortBy.ToLower() switch
            //{
            //    "name" => sortDescending ? query.OrderByDescending(s => s.Name) : query.OrderBy(s => s.Name),
            //    "description" => sortDescending ? query.OrderByDescending(s => s.Description) : query.OrderBy(s => s.Description),
            //    "establisheddate" => sortDescending ? query.OrderByDescending(s => s.EstablishedDate) : query.OrderBy(s => s.EstablishedDate),
            //    _ => query.OrderBy(s => s.Name), // Default sorting by Name
            //};

            var totalCount = await query.CountAsync();

            var items = await query.Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<Employee>(items, totalCount);
        }
    }
}
