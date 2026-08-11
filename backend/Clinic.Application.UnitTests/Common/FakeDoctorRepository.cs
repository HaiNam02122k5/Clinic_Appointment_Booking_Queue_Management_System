using Clinic.Application.Common.Models;
using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;

namespace Clinic.Application.UnitTests.Common
{
    public class FakeDoctorRepository : IDoctorRepository
    {
        private readonly List<Doctor> _doctors = [];
        public async Task AddAsync(Doctor doctor)
        {
            _doctors.Add(doctor);
        }

        public async Task<IEnumerable<WorkSchedule>> GetPlannedSchedulesByDoctorIdAsync(Guid doctorId, DateOnly startDate, DateOnly endDate)
        {
            // Check if the time range exceeds 1 month
            if (startDate.AddMonths(1) < endDate)
            {
                throw new ArgumentException("The time range cannot exceed 1 month.");
            }

            var doc = _doctors.FirstOrDefault(d => d.Id == doctorId);
            return doc?.WorkSchedules.Where(ws =>
                (DateOnly.FromDateTime(ws.ShiftStart) >= startDate && DateOnly.FromDateTime(ws.ShiftStart) <= endDate) ||
                (DateOnly.FromDateTime(ws.ShiftEnd) >= startDate && DateOnly.FromDateTime(ws.ShiftEnd) <= endDate)
            ).ToList();
        }

        public async Task<IEnumerable<ShiftRequest>> GetRequestedSchedulesByDoctorIdAsync(Guid doctorId, DateOnly startDate, DateOnly endDate)
        {
            // Check if the time range exceeds 1 month
            if (startDate.AddMonths(1) < endDate)
            {
                throw new ArgumentException("The time range cannot exceed 1 month.");
            }

            var doc = _doctors.FirstOrDefault(d => d.Id == doctorId);
            return doc?.ShiftRequests.Where(sr =>
                (DateOnly.FromDateTime(sr.ShiftStart) >= startDate && DateOnly.FromDateTime(sr.ShiftStart) <= endDate) ||
                (DateOnly.FromDateTime(sr.ShiftEnd) >= startDate && DateOnly.FromDateTime(sr.ShiftEnd) <= endDate)
            ).ToList();
        }

        public async Task<Doctor?> GetInfoByIdAsync(Guid doctorId)
        {
            return _doctors.FirstOrDefault(d => d.Id == doctorId);
        }

        public async Task<PagedResult<Doctor>> GetPagedAsync(string? searchTerm, string? sortBy, string? qualification, DoctorStatus? status, Guid? specialtyId, bool descending, int pageNumber, int pageSize)
        {
            var query = _doctors
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
                "name" => !descending ? query.OrderBy(d => d.Employee.Person.FullName) : query.OrderByDescending(d => d.Employee.Person.FullName),
                "experienceyears" => !descending ? query.OrderBy(d => d.ExperienceYears) : query.OrderByDescending(d => d.ExperienceYears),
                _ => !descending ? query.OrderBy(d => d.Employee.Person.FullName) : query.OrderByDescending(d => d.Employee.Person.FullName), // Default sorting by Name
            };

            var totalCount = query.Count();

            var items = query.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();
            return new PagedResult<Doctor>(items, totalCount);
        }
    }
}
