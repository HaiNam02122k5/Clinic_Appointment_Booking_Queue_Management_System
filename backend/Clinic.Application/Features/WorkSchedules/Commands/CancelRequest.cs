using Clinic.Application.Common.Exceptions;
using Clinic.Application.Interfaces;
using MediatR;

namespace Clinic.Application.Features.WorkSchedules.Commands
{
    // Use-case: Doctor cancels a shift request
    public record CancelRequestCommand(
        Guid DoctorId,
        Guid Id
    ) : IRequest<Guid>;
    public class CancelRequestCommandHandler : IRequestHandler<CancelRequestCommand, Guid>
    {
        private readonly IWorkScheduleRepository _workScheduleRepository;
        private readonly IUnitOfWork _unitOfWork;
        public CancelRequestCommandHandler(IWorkScheduleRepository workScheduleRepository, IUnitOfWork unitOfWork)
        {
            _workScheduleRepository = workScheduleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CancelRequestCommand request, CancellationToken cancellationToken)
        {
            var shiftRequest = await _workScheduleRepository.GetShiftRequestByIdAsync(request.Id);
            if (shiftRequest == null)
            {
                throw new NotFoundException("Shift request not found");
            }
            if (shiftRequest.DoctorId != request.DoctorId)
            {
                throw new UnauthorizedAccessException("You are not authorized to cancel this shift request");
            }
            shiftRequest.Cancel();
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return shiftRequest.Id;
        }
    }
}
