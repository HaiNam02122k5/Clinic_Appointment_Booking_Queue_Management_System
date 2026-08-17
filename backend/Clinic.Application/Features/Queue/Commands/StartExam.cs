using Clinic.Application.Common.Exceptions;
using Clinic.Application.Interfaces;
using MediatR;

namespace Clinic.Application.Features.Queue.Commands
{
    public record StartExamCommand(Guid QueueTicketId) : IRequest;

    /// <summary>
    /// Chuyển vé từ Called -> InProgress khi bệnh nhân đã vào phòng khám và bác sĩ
    /// bắt đầu khám. Sau bước này, GetActiveTicketAsync vẫn coi vé là "active"
    /// (bác sĩ vẫn đang bận), nên CallNextQueue vẫn bị chặn cho tới khi Complete/Skip.
    /// </summary>
    public class StartExamHandler : IRequestHandler<StartExamCommand>
    {
        private readonly IQueueTicketRepository _queueTicketRepository;
        private readonly IUnitOfWork _unitOfWork;

        public StartExamHandler(
            IQueueTicketRepository queueTicketRepository,
            IUnitOfWork unitOfWork)
        {
            _queueTicketRepository = queueTicketRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task Handle(StartExamCommand command, CancellationToken cancellationToken)
        {
            var queueTicket = await _queueTicketRepository.GetByIdAsync(command.QueueTicketId)
                ?? throw new NotFoundException($"Queue ticket '{command.QueueTicketId}' not found.");

            queueTicket.StartExam();

            await _queueTicketRepository.UpdateAsync(queueTicket);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}