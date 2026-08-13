using Clinic.Application.Common.Exceptions;
using Clinic.Application.Interfaces;
using Clinic.Domain.Enums;
using MediatR;

namespace Clinic.Application.Features.WorkSchedules.Commands
{
    // Use-case: Doctor updates an existing shift request
    public record UpdateShiftRequestCommand(
        Guid Id,
        Guid DoctorId,
        DateOnly Date,
        TimeOnly StartTime,
        TimeOnly EndTime,
        int PatientLimit,
        string Reason
    ) : IRequest<Guid>;
    public class UpdateShiftRequestCommandHandler : IRequestHandler<UpdateShiftRequestCommand, Guid>
    {
        private readonly IWorkScheduleRepository _workScheduleRepository;
        private readonly IUnitOfWork _unitOfWork;
        public UpdateShiftRequestCommandHandler(IWorkScheduleRepository workScheduleRepository, IUnitOfWork unitOfWork)
        {
            _workScheduleRepository = workScheduleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(UpdateShiftRequestCommand request, CancellationToken cancellationToken)
        {
            var shiftRequest = await _workScheduleRepository.GetShiftRequestByIdAsync(request.Id);
            if (shiftRequest == null)
            {
                throw new NotFoundException("Shift request not found");
            }
            if (shiftRequest.DoctorId != request.DoctorId)
            {
                throw new UnauthorizedAccessException("You are not authorized to update this shift request");
            }
            shiftRequest.UpdateShift(request.Date, request.StartTime, request.EndTime, request.PatientLimit, request.Reason);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return shiftRequest.Id;
        }
    }
}
