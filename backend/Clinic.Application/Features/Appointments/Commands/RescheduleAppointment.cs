using Clinic.Application.Common.Exceptions;
using Clinic.Application.Interfaces;
using Clinic.Domain.Common.Exceptions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands
{
    public record RescheduleAppointmentCommand(Guid AppointmentId, Guid NewWorkScheduleId, DateTime NewTimeSlot) : IRequest;

    public class RescheduleAppointmentHandler : IRequestHandler<RescheduleAppointmentCommand>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IWorkScheduleRepository _workScheduleRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppointmentPolicySettings _policySettings;

        public RescheduleAppointmentHandler(
            IAppointmentRepository appointmentRepository,
            IWorkScheduleRepository workScheduleRepository,
            ICurrentUser currentUser,
            IUnitOfWork unitOfWork,
            IAppointmentPolicySettings policySettings)
        {
            _appointmentRepository = appointmentRepository;
            _workScheduleRepository = workScheduleRepository;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
            _policySettings = policySettings;
        }

        public async Task Handle(RescheduleAppointmentCommand command, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(command.AppointmentId)
                ?? throw new NotFoundException($"Appointment '{command.AppointmentId}' not found.");

            if (command.NewTimeSlot <= DateTime.UtcNow)
            {
                throw new ArgumentException("New time slot must be in the future.");
            }

            // Permission "appointment.reschedule" hiện chưa tách own/any như "appointment.cancel".
            // Tạm dùng "appointment.update" (chỉ Admin/Receptionist/Doctor có) làm cờ nhận biết staff
            // để cho phép staff đổi lịch cho bất kỳ bệnh nhân nào mà không bị áp rule dưới đây.
            var isStaff = _currentUser.HasPermission("appointment.update");

            if (!isStaff)
            {
                // Patient chỉ được đổi lịch của chính mình.
                if (appointment.PatientId != _currentUser.PatientId)
                {
                    throw new ForbiddenException("You are not allowed to reschedule this appointment.");
                }

                // Rule "trước hạn": Patient phải đổi lịch trước ít nhất N giờ so với giờ hẹn hiện tại.
                var minNoticeHours = _policySettings.RescheduleMinNoticeHours;
                var deadline = appointment.TimeSlot.AddHours(-minNoticeHours);

                if (DateTime.UtcNow > deadline)
                {
                    throw new ConflictException(
                        $"Cannot reschedule within {minNoticeHours} hours of the appointment time. " +
                        "Please contact the front desk for assistance.");
                }
            }

            // === Fix bug: Reschedule chỉ đổi TimeSlot, giữ nguyên WorkScheduleId cũ ===
            // Trước đây handler chỉ nhận NewTimeSlot rồi gọi appointment.Reschedule(newTimeSlot),
            // KHÔNG đụng tới WorkScheduleId. Hệ quả: nếu bệnh nhân đổi sang khung giờ thuộc 1 ca
            // trực khác (vd. ca cũ của bác sĩ A 08:00-10:00, đổi sang 15:00), Appointment vẫn bị
            // gắn cứng với WorkScheduleId cũ dù giờ mới không còn nằm trong ca đó, đồng thời ca mới
            // (nơi bệnh nhân thực sự muốn khám) hoàn toàn không được kiểm tra tồn tại/bác sĩ/sức
            // chứa/trùng lịch.
            //
            // Fix: bắt buộc client gửi NewWorkScheduleId, handler load đúng ca mới rồi để
            // Appointment.Reschedule(newWorkSchedule, newTimeSlot) tự validate Active/thời gian
            // nằm trong ca/sức chứa trước khi cập nhật ĐỒNG THỜI cả WorkScheduleId lẫn TimeSlot.
            // Cũng dùng lại LockAsync + transaction tường minh giống CreateAppointmentHandler để
            // chống race condition "đếm rồi mới insert" khi 2 người cùng đổi lịch vào ca mới còn
            // đúng 1 chỗ trống tại cùng thời điểm.
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                await _workScheduleRepository.LockAsync(command.NewWorkScheduleId, cancellationToken);

                var newWorkSchedule = await _workScheduleRepository.GetByIdAsync(command.NewWorkScheduleId)
                    ?? throw new NotFoundException($"Work schedule '{command.NewWorkScheduleId}' not found.");

                appointment.Reschedule(newWorkSchedule, command.NewTimeSlot);

                await _appointmentRepository.UpdateAsync(appointment);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
            }
            catch
            {
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                throw;
            }
        }
    }
}