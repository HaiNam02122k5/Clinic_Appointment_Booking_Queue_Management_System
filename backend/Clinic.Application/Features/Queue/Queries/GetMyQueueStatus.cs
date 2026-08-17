using Clinic.Application.Common.Exceptions;
using Clinic.Application.Contracts;
using Clinic.Application.Interfaces;
using Clinic.Domain.Enums;
using MediatR;

namespace Clinic.Application.Features.Queue.Queries
{
    /// <summary>Trả về null nếu bệnh nhân chưa check-in trong ngày hôm nay (không có vé nào đang "sống").</summary>
    public record GetMyQueueStatusQuery : IRequest<MyQueueStatusDto?>;

    public class GetMyQueueStatusHandler : IRequestHandler<GetMyQueueStatusQuery, MyQueueStatusDto?>
    {
        // Thời gian khám trung bình ước lượng - dùng tạm cho tới khi làm thuật toán ước tính
        // dựa trên lịch sử thời gian khám thực tế (mục VIII - tính năng nâng cao, tuỳ chọn).
        private const int EstimatedMinutesPerPatient = 10;

        private readonly IQueueTicketRepository _queueTicketRepository;
        private readonly ICurrentUser _currentUser;

        public GetMyQueueStatusHandler(IQueueTicketRepository queueTicketRepository, ICurrentUser currentUser)
        {
            _queueTicketRepository = queueTicketRepository;
            _currentUser = currentUser;
        }

        public async Task<MyQueueStatusDto?> Handle(GetMyQueueStatusQuery request, CancellationToken cancellationToken)
        {
            if (_currentUser.PatientId is null)
            {
                throw new ForbiddenException("Only a patient account has a queue status to view.");
            }

            var today = DateTime.UtcNow;

            var myTicket = await _queueTicketRepository.GetActiveByPatientAsync(_currentUser.PatientId.Value, today);
            if (myTicket is null)
            {
                return null;
            }

            var doctorId = myTicket.Appointment.WorkSchedule.DoctorId;
            var dayQueue = await _queueTicketRepository.GetByDoctorAsync(doctorId, today);

            // dayQueue đã được sắp theo Priority (ưu tiên trước) rồi QueueNumber tăng dần -
            // đúng thứ tự sẽ được gọi. Đếm số vé đang chờ/đã gọi đứng trước vé của mình.
            var waitingAhead = 0;
            foreach (var ticket in dayQueue)
            {
                if (ticket.Id == myTicket.Id)
                {
                    break;
                }

                if (ticket.Status == QueueStatus.Waiting || ticket.Status == QueueStatus.Called)
                {
                    waitingAhead++;
                }
            }

            return new MyQueueStatusDto
            {
                QueueTicketId = myTicket.Id,
                QueueNumber = myTicket.QueueNumber,
                Status = myTicket.Status.ToString(),
                DoctorName = myTicket.Appointment.WorkSchedule.Doctor?.Employee?.Person?.FullName ?? string.Empty,
                PositionInQueue = waitingAhead + 1,
                EstimatedWaitMinutes = waitingAhead * EstimatedMinutesPerPatient
            };
        }
    }
}