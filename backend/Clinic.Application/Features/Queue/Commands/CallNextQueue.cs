using Clinic.Application.Common.Exceptions;
using Clinic.Application.Contracts;
using Clinic.Application.Interfaces;
using MediatR;

namespace Clinic.Application.Features.Queue.Commands
{
    public record CallNextQueueCommand(Guid DoctorId) : IRequest<QueueTicketDto>;

    public class CallNextQueueHandler : IRequestHandler<CallNextQueueCommand, QueueTicketDto>
    {
        private readonly IQueueTicketRepository _queueTicketRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CallNextQueueHandler(
            IQueueTicketRepository queueTicketRepository,
            IUnitOfWork unitOfWork)
        {
            _queueTicketRepository = queueTicketRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<QueueTicketDto> Handle(CallNextQueueCommand command, CancellationToken cancellationToken)
        {
            var queueTicket = await _queueTicketRepository.GetNextWaitingAsync(command.DoctorId, DateTime.UtcNow)
                ?? throw new ConflictException("There are no patients waiting in the queue for this doctor.");

            queueTicket.Call();

            await _queueTicketRepository.UpdateAsync(queueTicket);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new QueueTicketDto
            {
                Id = queueTicket.Id,
                AppointmentId = queueTicket.AppointmentId,
                QueueNumber = queueTicket.QueueNumber,
                Priority = queueTicket.Priority,
                Status = queueTicket.Status.ToString(),
                CheckInTime = queueTicket.CheckInTime,
                CalledAt = queueTicket.CalledAt,
                PatientName = queueTicket.Appointment.Patient?.Person?.FullName
            };
        }
    }
}