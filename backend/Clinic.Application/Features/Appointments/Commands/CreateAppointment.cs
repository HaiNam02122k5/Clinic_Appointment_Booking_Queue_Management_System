using Clinic.Application.Common.Exceptions;
using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using Clinic.Domain.Common.Exceptions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands
{
    public record CreateAppointmentCommand(Guid WorkScheduleId, DateTime TimeSlot, string? Reason) : IRequest<Guid>;

    public class CreateAppointmentHandler : IRequestHandler<CreateAppointmentCommand, Guid>
    {
        private readonly IWorkScheduleRepository _workScheduleRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public CreateAppointmentHandler(
            IWorkScheduleRepository workScheduleRepository,
            IAppointmentRepository appointmentRepository,
            ICurrentUser currentUser,
            IUnitOfWork unitOfWork)
        {
            _workScheduleRepository = workScheduleRepository;
            _appointmentRepository = appointmentRepository;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateAppointmentCommand command, CancellationToken cancellationToken)
        {
            // Chỉ tài khoản đã có hồ sơ Patient mới được tự đặt lịch online.
            if (_currentUser.PatientId is null)
            {
                throw new ForbiddenException("Only a patient account can book an appointment.");
            }

            // === Fix race condition "đếm rồi mới insert" ===
            // Trước đây: đọc workSchedule.Appointments.Count(...) rồi so với PatientLimitPerSlot,
            // sau đó mới AddAsync + SaveChangesAsync. Nếu slot chỉ còn đúng 1 chỗ và 2 request
            // đặt lịch chạy gần như cùng lúc, cả 2 đều đọc thấy Count < Limit (dữ liệu cũ chưa
            // ai kịp ghi) TRƯỚC KHI request nào insert xong, nên cả 2 đều vượt qua điều kiện và
            // đều tạo Appointment thành công -> vượt PatientLimitPerSlot (mất "count rồi insert"
            // không nguyên tử, phân loại race condition kiểu TOCTOU - time-of-check to
            // time-of-use).
            //
            // Fix: mở 1 transaction DB tường minh và khóa ghi (UPDLOCK, HOLDLOCK) đúng 1 dòng
            // WorkSchedule đang đặt NGAY TỪ ĐẦU (LockAsync), trước khi đọc + đếm. Nhờ vậy request
            // thứ 2 cho CÙNG WorkScheduleId sẽ bị chặn tại LockAsync cho tới khi request thứ 1
            // COMMIT (hoặc ROLLBACK) xong. Khi request thứ 2 được chạy tiếp, nó luôn đọc được số
            // Appointments mới nhất (đã bao gồm appointment mà request thứ 1 vừa tạo), nên nếu
            // slot đã đầy, request thứ 2 sẽ bị chặn đúng ở ConflictException thay vì insert thành
            // công. 2 request đặt lịch cho 2 slot KHÁC NHAU không ảnh hưởng nhau vì lock chỉ khóa
            // đúng 1 dòng WorkSchedule đang xử lý (row-level lock), không khóa cả bảng.
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                await _workScheduleRepository.LockAsync(command.WorkScheduleId, cancellationToken);

                var workSchedule = await _workScheduleRepository.GetByIdAsync(command.WorkScheduleId)
                    ?? throw new NotFoundException($"Work schedule '{command.WorkScheduleId}' not found.");

                if (workSchedule.Status != WorkScheduleStatus.Active)
                {
                    throw new ConflictException("This shift is no longer accepting appointments.");
                }

                if (command.TimeSlot < workSchedule.ShiftStart || command.TimeSlot > workSchedule.ShiftEnd)
                {
                    throw new ArgumentException("The selected time slot is outside the doctor's shift.");
                }

                if (command.TimeSlot <= DateTime.UtcNow)
                {
                    throw new ArgumentException("Time slot must be in the future.");
                }

                // Số lịch hẹn còn "sống" (chưa hủy) trong khung giờ này không được vượt quá
                // giới hạn. An toàn với race condition nhờ đã LockAsync ở trên: tại thời điểm
                // này chắc chắn không còn request nào khác đang đặt cùng slot chạy song song.
                var bookedCount = workSchedule.Appointments.Count(a => a.Status != AppointmentStatus.Cancelled);
                if (bookedCount >= workSchedule.PatientLimitPerSlot)
                {
                    throw new ConflictException("This shift is already fully booked.");
                }

                var appointment = new Appointment
                {
                    PatientId = _currentUser.PatientId.Value,
                    WorkScheduleId = workSchedule.Id,
                    TimeSlot = command.TimeSlot,
                    Reason = command.Reason,
                    IsWalkIn = false
                };

                await _appointmentRepository.AddAsync(appointment);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);

                return appointment.Id;
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}