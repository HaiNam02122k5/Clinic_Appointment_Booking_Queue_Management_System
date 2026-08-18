using Clinic.Application.Common.Exceptions;
using Clinic.Application.Interfaces;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands
{
    public record CancelAppointmentCommand(Guid AppointmentId) : IRequest;

    public class CancelAppointmentHandler : IRequestHandler<CancelAppointmentCommand>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public CancelAppointmentHandler(
            IAppointmentRepository appointmentRepository,
            ICurrentUser currentUser,
            IUnitOfWork unitOfWork)
        {
            _appointmentRepository = appointmentRepository;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CancelAppointmentCommand command, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(command.AppointmentId)
                ?? throw new NotFoundException($"Appointment '{command.AppointmentId}' not found.");

            // "any" scope (Admin/Receptionist): được hủy bất kỳ appointment nào.
            // Nếu không có "any", user chỉ được hủy appointment của chính mình ("own" scope, Patient).
            var hasAnyScope = _currentUser.HasPermission("appointment.cancel.any");
            if (!hasAnyScope && appointment.PatientId != _currentUser.PatientId)
            {
                throw new ForbiddenException("You are not allowed to cancel this appointment.");
            }

            appointment.Cancel();

            await _appointmentRepository.UpdateAsync(appointment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}