using Clinic.Application.Common.Exceptions;
using Clinic.Application.Interfaces;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands
{
    public record CancelAppointmentCommand(Guid AppointmentId) : IRequest;

    public class CancelAppointmentHandler : IRequestHandler<CancelAppointmentCommand>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IQueueTicketRepository _queueTicketRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public CancelAppointmentHandler(
            IAppointmentRepository appointmentRepository,
            IQueueTicketRepository queueTicketRepository,
            ICurrentUser currentUser,
            IUnitOfWork unitOfWork)
        {
            _appointmentRepository = appointmentRepository;
            _queueTicketRepository = queueTicketRepository;
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

            // appointment.Cancel() tự kiểm tra trạng thái QueueTicket (nếu CheckedIn) và cascade
            // hủy vé khi hợp lệ (Waiting/Called), hoặc ném lỗi khi vé đang InProgress/Completed.
            appointment.Cancel();

            await _appointmentRepository.UpdateAsync(appointment);

            // Nếu Cancel() vừa cascade-hủy QueueTicket, lưu rõ ràng thay đổi đó qua repository
            // riêng của nó, tránh phụ thuộc ngầm vào việc EF change-tracker tự phát hiện.
            if (appointment.QueueTicket is not null)
            {
                await _queueTicketRepository.UpdateAsync(appointment.QueueTicket);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}