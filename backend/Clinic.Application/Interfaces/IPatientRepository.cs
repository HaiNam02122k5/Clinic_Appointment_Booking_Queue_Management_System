using Clinic.Domain.Entities;

namespace Clinic.Application.Interfaces
{
    public interface IPatientRepository
    {
        /// <summary>
        /// Lấy Patient theo PersonId. Dùng để kiểm tra 1 Person đã có hồ sơ Patient hay chưa
        /// trước khi tạo mới (tránh vi phạm UNIQUE index PersonId).
        /// </summary>
        Task<Patient?> GetByPersonIdAsync(Guid personId);

        /// <summary>
        /// Lấy Patient theo Id.
        /// </summary>
        Task<Patient?> GetByIdAsync(Guid id);

        /// <summary>
        /// Thêm mới 1 hồ sơ Patient.
        /// </summary>
        Task AddAsync(Patient patient);
    }
}