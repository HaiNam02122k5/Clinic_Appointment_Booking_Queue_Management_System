using Clinic.Application.Common.Models;
using Clinic.Domain.Entities;

namespace Clinic.Application.Interfaces
{
    public interface IDoctorRepository
    {
        /// <summary>
        /// Lấy 1 Doctor theo Id, kèm Employee -> Person (tên) và WorkHistories -> Specialty
        /// (để xác định chuyên khoa hiện tại).
        /// </summary>
        Task<Doctor?> GetByIdAsync(Guid id);

        /// <summary>
        /// Lấy danh sách bác sĩ (chỉ Status = Active) có phân trang, lọc theo tên và/hoặc chuyên khoa hiện tại.
        /// Dùng cho màn "tìm bác sĩ/chuyên khoa" của bệnh nhân.
        /// </summary>
        Task<PagedResult<Doctor>> GetPagedAsync(string? search, Guid? specialtyId, int page, int pageSize);
    }
}