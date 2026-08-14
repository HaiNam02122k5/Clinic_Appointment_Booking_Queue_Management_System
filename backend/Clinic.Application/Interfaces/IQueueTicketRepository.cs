using Clinic.Domain.Entities;
using System;

namespace Clinic.Application.Interfaces
{
    public interface IQueueTicketRepository
    {
        /// <summary>
        /// Tính số thứ tự hàng đợi tiếp theo cho 1 bác sĩ trong 1 ngày cụ thể
        /// (dựa trên CheckInTime.Date của các QueueTicket đã tồn tại cho bác sĩ đó).
        /// Trả về 1 nếu bác sĩ chưa có bệnh nhân nào check-in trong ngày.
        /// </summary>
        Task<int> GetNextQueueNumberAsync(Guid doctorId, DateTime date);

        /// <summary>
        /// Thêm mới 1 số thứ tự hàng đợi, sinh ra khi bệnh nhân check-in
        /// (tại quầy hoặc online).
        /// </summary>
        Task AddAsync(QueueTicket queueTicket);

        /// <summary>
        /// Lấy 1 QueueTicket theo Id, kèm Appointment -> WorkSchedule (Doctor) và Appointment -> Patient -> Person
        /// để phục vụ authorization theo scope và hiển thị tên bệnh nhân.
        /// </summary>
        Task<QueueTicket?> GetByIdAsync(Guid id);

        /// <summary>
        /// Lấy toàn bộ hàng đợi của 1 bác sĩ trong 1 ngày cụ thể, sắp theo Priority (ưu tiên trước)
        /// rồi theo QueueNumber tăng dần - dùng cho dashboard xem hàng đợi (queue.view/doctor.queue.view).
        /// </summary>
        Task<List<QueueTicket>> GetByDoctorAsync(Guid doctorId, DateTime date);

        /// <summary>
        /// Lấy vé đang chờ (Waiting) tiếp theo cần gọi của 1 bác sĩ trong ngày, ưu tiên vé Priority = true,
        /// sau đó theo QueueNumber tăng dần. Trả về null nếu không còn ai đang chờ.
        /// </summary>
        Task<QueueTicket?> GetNextWaitingAsync(Guid doctorId, DateTime date);

        /// <summary>
        /// Cập nhật 1 QueueTicket đã tồn tại (gọi số, bỏ qua lượt, đổi ưu tiên).
        /// </summary>
        Task UpdateAsync(QueueTicket queueTicket);
    }
}