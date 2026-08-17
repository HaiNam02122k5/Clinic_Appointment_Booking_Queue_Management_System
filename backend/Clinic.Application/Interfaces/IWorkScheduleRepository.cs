using Clinic.Domain.Entities;

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
    }
}