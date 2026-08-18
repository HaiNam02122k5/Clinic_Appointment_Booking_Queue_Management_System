using Clinic.Application.Common.Models;
using Clinic.Application.Contracts;
using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using Clinic.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Sqlserver.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly ApplicationDbContext _context;
        public PatientRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public Task AddAsync(Patient patient)
        {
            throw new NotImplementedException();
        }

        public async Task<Patient?> GetByIdAsync(Guid? patientId)
        {
            return await _context.Patients.Include(p => p.Person).ThenInclude(p => p.User).FirstOrDefaultAsync(p => p.Id == patientId);
        }

        public async Task<PagedResult<Patient>> GetPagedAsync(string? search, string? sortBy, bool descending, int pageNumber, int pageSize)
        {
            var query = _context.Patients.Include(p => p.Person).ThenInclude(p => p.User).Where(p => !p.IsDeleted);

            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(p => p.Person.FullName.Contains(search) || p.Person.Email.Contains(search));
            }

            query = sortBy.ToLower() switch
            {
                "fullname" => descending ? query.OrderByDescending(p => p.Person.FullName) : query.OrderBy(p => p.Person.FullName),
                "email" => descending ? query.OrderByDescending(p => p.Person.Email) : query.OrderBy(p => p.Person.Email),
                "dateofbirth" => descending ? query.OrderByDescending(p => p.Person.DateOfBirth) : query.OrderBy(p => p.Person.DateOfBirth),
                _ => query.OrderBy(p => p.Person.FullName)
            };

            var totalCount = await query.CountAsync();
            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).AsNoTracking().ToListAsync();
            return new PagedResult<Patient>(items, totalCount);
        }

        public async Task<Patient?> GetPatientByUserIdAsync(Guid userId)
        {
            return await _context.Patients.Include(p => p.Person).ThenInclude(p => p.User).FirstOrDefaultAsync(p => p.Person.User.Id == userId);
        }
    }
}
