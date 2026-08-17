using Clinic.Application.Common.Exceptions;
using Clinic.Application.Interfaces;
using MediatR;

namespace Clinic.Application.Features.Queue.Commands
{
    public record CompleteExamCommand(Guid QueueTicketId) : IRequest;

    /// <summary>
    /// Chuyển vé từ InProgress -> Completed khi bác sĩ khám xong. Giải phóng "chỗ" active
    /// của bác sĩ, cho phép CallNextQueue gọi vé tiếp theo.
    /// </summary>
    public class CompleteExamHandler : IRequestHandler<CompleteExamCommand>
    {
        private readonly IQueueTicketRepository _queueTicketRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CompleteExamHandler(
            IQueueTicketRepository queueTicketRepository,
            IUnitOfWork unitOfWork)
        {
            _queueTicketRepository = queueTicketRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(CompleteExamCommand command, CancellationToken cancellationToken)
        {
            var queueTicket = await _queueTicketRepository.GetByIdAsync(command.QueueTicketId)
                ?? throw new NotFoundException($"Queue ticket '{command.QueueTicketId}' not found.");

            queueTicket.Complete();

            await _queueTicketRepository.UpdateAsync(queueTicket);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}