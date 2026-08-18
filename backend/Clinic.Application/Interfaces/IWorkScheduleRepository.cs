using Clinic.Domain.Entities;
using System;
using System.Collections.Generic;

namespace Clinic.Application.Interfaces
{
    public interface IWorkScheduleRepository
    {
        /// <summary>
        /// Lấy 1 WorkSchedule theo Id, kèm Appointments (để tính số chỗ còn trống) và
        /// Doctor -> Employee -> Person (để hiển thị/kiểm tra khi cần).
        /// </summary>
        Task<WorkSchedule?> GetByIdAsync(Guid id);

        /// <summary>
        /// Lấy các WorkSchedule (khung giờ) còn chỗ trống, chưa kết thúc, có thể lọc theo bác sĩ,
        /// chuyên khoa hiện tại của bác sĩ, và khoảng ngày. Dùng cho màn "chọn khung giờ trống để đặt lịch".
        /// </summary>
        Task<List<WorkSchedule>> GetAvailableSlotsAsync(Guid? doctorId, Guid? specialtyId, DateTime? fromDate, DateTime? toDate);

        // ===== Nhóm chức năng: quản lý ca trực bác sĩ (shift request - của dev/cuong) =====

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
        /// Retrieves a work schedule by its unique identifier asynchronously (không kèm include, dùng cho nghiệp vụ ca trực).
        /// </summary>
        Task<WorkSchedule?> GetWorkScheduleByIdAsync(Guid scheduleId);

        /// <summary>
        /// Retrieves a requested shift by its unique identifier asynchronously.
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
        Task<bool> HasOverlappingWorkSchedule(Guid doctorId, DateTime startTime, DateTime endTime);

        /// <summary>
        /// Checks if a doctor has already requested a shift that has exact same time range asynchronously.
        /// </summary>
        Task<bool> HasDuplicateShiftRequest(Guid doctorId, DateTime startTime, DateTime endTime);
    }
}