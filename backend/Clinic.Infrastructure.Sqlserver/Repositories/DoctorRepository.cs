using Clinic.Application.Common.Models;
using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using Clinic.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Sqlserver.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly ApplicationDbContext _context;

        public DoctorRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Doctor?> GetByIdAsync(Guid id)
        {
            return await _context.Doctors
                .Include(d => d.Employee)
                    .ThenInclude(e => e.Person)
                .Include(d => d.WorkHistories)
                    .ThenInclude(wh => wh.Specialty)
                .FirstOrDefaultAsync(d => d.Id == id && d.IsDeleted == false);
        }

        public async Task<PagedResult<Doctor>> GetPagedAsync(string? search, Guid? specialtyId, int page, int pageSize)
        {
            var query = _context.Doctors
                .Include(d => d.Employee)
                    .ThenInclude(e => e.Person)
                .Include(d => d.WorkHistories)
                    .ThenInclude(wh => wh.Specialty)
                .Where(d => d.IsDeleted == false && d.Status == DoctorStatus.Active);

            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(d => d.Employee.Person.FullName.Contains(search));
            }

            if (specialtyId.HasValue)
            {
                query = query.Where(d => d.WorkHistories.Any(wh =>
                    wh.SpecialtyId == specialtyId.Value && wh.Status == WorkHistoryStatus.Active));
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(d => d.Employee.Person.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PagedResult<Doctor>(items, totalCount);
        }
    }
}