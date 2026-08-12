using Azure;
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
        public async Task AddAsync(Doctor doctor)
        {
            _context.Doctors.Add(doctor);
        }

        public async Task<Doctor?> GetInfoByIdAsync(Guid doctorId)
        {
            return await _context.Doctors
                .Include(d => d.Employee)
                    .ThenInclude(e => e.Person)
                .Include(d => d.WorkHistories)
                    .ThenInclude(wh => wh.Specialty)
                .Include(d => d.WorkSchedules)
                .Include(d => d.ShiftRequests)
                .FirstOrDefaultAsync(d => d.Id == doctorId && !d.IsDeleted);
        }

        public async Task<PagedResult<Doctor>> GetPagedAsync(string? searchTerm, string? sortBy, string? qualification, DoctorStatus? status, Guid? specialtyId, bool descending, int pageNumber, int pageSize)
        {
            var query = _context.Doctors
                .Include(d => d.Employee)
                    .ThenInclude(e => e.Person)
                .Include(d => d.WorkHistories)
                    .ThenInclude(wh => wh.Specialty)
                .Where(d => d.IsDeleted == false);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(d => d.Employee.Person.FullName.Contains(searchTerm) ||
                    d.LicenseNumber.Contains(searchTerm) ||
                    d.Qualification.Contains(searchTerm) ||
                    (d.Biography != null && d.Biography.Contains(searchTerm)) ||
                    (d.WorkHistories.Any(wh => wh.EndDate == null && wh.Specialty.Name.Contains(searchTerm)))
                );
            }

            if (!string.IsNullOrEmpty(qualification))
            {
                query = query.Where(d => d.Qualification == qualification);
            }

            if (status != null)
            {
                query = query.Where(d => d.Status == status);
            }

            if (specialtyId != null)
            {
                query = query.Where(d => d.WorkHistories.Any(wh => wh.EndDate == null && wh.SpecialtyId == specialtyId));
            }

            query = sortBy.ToLower() switch
            {
                "fullname" => !descending ? query.OrderBy(d => d.Employee.Person.FullName) : query.OrderByDescending(d => d.Employee.Person.FullName),
                "experienceyears" => !descending ? query.OrderBy(d => d.ExperienceYears) : query.OrderByDescending(d => d.ExperienceYears),
                _ => !descending ? query.OrderBy(d => d.Employee.Person.FullName) : query.OrderByDescending(d => d.Employee.Person.FullName), // Default sorting by Name
            };

            var totalCount = await query.CountAsync();

            var items = await query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToListAsync();
            return new PagedResult<Doctor>(items, totalCount);
        }
    }
}
