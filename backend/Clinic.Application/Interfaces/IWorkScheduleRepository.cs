using Clinic.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.Interfaces
{
    public interface IWorkScheduleRepository
    {
        /// <summary>
        /// Retrieves work schedules for a doctor in the specified time range.
        /// The range should be less than 1 month.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the time range exceeds 1 month.</exception>
        Task<IEnumerable<WorkSchedule>> GetPlannedSchedulesByDoctorIdAsync(Guid doctorId, DateOnly startDate, DateOnly endDate);

        /// <summary>
        /// Retrieves requested shifts for a doctor in the specified time range.
        /// The range should be less than 1 month.
        /// </summary>
        /// <exception cref="ArgumentException">Thrown when the time range exceeds 1 month.</exception>
        Task<IEnumerable<ShiftRequest>> GetRequestedSchedulesByDoctorIdAsync(Guid doctorId, DateOnly startDate, DateOnly endDate);

        /// <summary>
        /// Retrieves a work schedule by its unique identifier asynchronously, including its associated doctor-person info.
        /// </summary>
        Task<WorkSchedule?> GetWorkScheduleByIdAsync(Guid scheduleId);

        /// <summary>
        /// Retrieves a requested shift by its unique identifier asynchronously, including its associated doctor and active appointments.
        /// </summary>
        Task<ShiftRequest?> GetShiftRequestByIdAsync(Guid scheduleId);

        /// <summary>
        /// Adds a new work schedule to the repository asynchronously.
        /// </summary>
        Task AddWorkScheduleAsync(WorkSchedule workSchedule);

        /// <summary>
        /// Adds a new shift request to the repository asynchronously.
        /// </summary>
        Task AddShiftRequestAsync(ShiftRequest shiftRequest);

        /// <summary>
        /// Checks if a doctor has overlapping work schedules within the specified time range asynchronously.
        /// </summary>
        Task<bool> HasOverlappingWorkSchedule(Guid doctorId, DateOnly date, TimeOnly startTime, TimeOnly endTime);

        /// <summary>
        /// Checks if a doctor has already requested a shift that has exact same time range asynchronously.
        /// </summary>
        Task<bool> HasDuplicateShiftRequest(Guid doctorId, DateOnly date, TimeOnly startTime, TimeOnly endTime);

        /// <summary>
        /// Retrieves work schedules for a doctor on a specific date, including their associated appointments asynchronously.
        /// </summary>
        Task<IEnumerable<WorkSchedule>> GetDoctorSchedulesWithAppointmentByDateAsync(Guid doctorId, DateOnly date);
    }
}
