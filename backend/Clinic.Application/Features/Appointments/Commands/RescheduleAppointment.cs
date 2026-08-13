using Clinic.Application.Common.Exceptions;
using Clinic.Application.Interfaces;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands
{
    public record RescheduleAppointmentCommand(Guid AppointmentId, DateTime NewTimeSlot) : IRequest;

    public class RescheduleAppointmentHandler : IRequestHandler<RescheduleAppointmentCommand>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppointmentPolicySettings _policySettings;

        public RescheduleAppointmentHandler(
            IAppointmentRepository appointmentRepository,
            ICurrentUser currentUser,
            IUnitOfWork unitOfWork,
            IAppointmentPolicySettings policySettings)
        {
            _appointmentRepository = appointmentRepository;
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

            appointment.Reschedule(command.NewTimeSlot);

            await _appointmentRepository.UpdateAsync(appointment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}