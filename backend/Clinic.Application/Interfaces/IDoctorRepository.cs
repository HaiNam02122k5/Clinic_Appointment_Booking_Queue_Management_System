using Clinic.Application.Common.Models;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;

namespace Clinic.Application.Interfaces
{
    public interface IDoctorRepository
    {
        /// <summary>
        /// Adds a new doctor to the repository asynchronously.
        /// </summary>
        Task AddAsync(Doctor doctor);

        /// <summary>
        /// Retrieves personal information for a doctor by their unique identifier asynchronously. Includes related entities Employee, Person, and WorkHistories.
        /// </summary>
        Task<Doctor?> GetInfoByIdAsync(Guid doctorId);

        /// <summary>
        /// Retrieves a doctor by their unique identifier asynchronously, including their associated work schedules in the specified time range.
        /// The range should be less than 1 month.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the time range exceeds 1 month.</exception>
        Task<IEnumerable<WorkSchedule>> GetPlannedSchedulesByDoctorIdAsync(Guid doctorId, DateOnly startDate, DateOnly endDate);

        /// <summary>
        /// Retrieves a doctor by their unique identifier asynchronously, including their associated requested shifts in the specified time range.
        /// The range should be less than 1 month.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the time range exceeds 1 month.</exception>
        Task<IEnumerable<ShiftRequest>> GetRequestedSchedulesByDoctorIdAsync(Guid doctorId, DateOnly startDate, DateOnly endDate);

        /// <summary>
        /// Retrieves a paginated list of doctors based on the provided search term, sorting options, status filter, specialty filter, and pagination parameters asynchronously.
        /// </summary>
        Task<PagedResult<Doctor>> GetPagedAsync(string? searchTerm, string? sortBy, string? qualification, DoctorStatus? status, Guid? specialtyId, bool descending, int pageNumber, int pageSize);
    }
}
