using Clinic.Application.Common.Exceptions;
using Clinic.Application.Contracts;
using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using MediatR;

namespace Clinic.Application.Features.WorkSchedules.Commands
{
    // Use-case: Doctor adds a new shift request
    public record AddDoctorShiftRequestCommand(Guid? UserId, DateTime StartTime, DateTime EndTime, int PatientLimitPerSlot, string reason) : IRequest<RequestedShiftDto>;

    public class AddDoctorShiftRequestCommandHandler : IRequestHandler<AddDoctorShiftRequestCommand, RequestedShiftDto>
    {
        private readonly IUserRepository _userRepository;
        private readonly IWorkScheduleRepository _workScheduleRepository;
        private readonly IUnitOfWork _unitOfWork;
        public AddDoctorShiftRequestCommandHandler(IUserRepository userRepository, IWorkScheduleRepository workScheduleRepository, IUnitOfWork unitOfWork)
        {
            _userRepository = userRepository;
            _workScheduleRepository = workScheduleRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<RequestedShiftDto> Handle(AddDoctorShiftRequestCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }
            var doctor = user.Person.Employee?.Doctor;
            if (doctor == null)
            {
                throw new ForbiddenException("The user is not a doctor and cannot add shift requests.");
            }
            if (await _workScheduleRepository.HasDuplicateShiftRequest(doctor.Id, request.StartTime, request.EndTime))
            {
                throw new InvalidOperationException("The doctor has a duplicate shift request in the specified time range.");
            }
            var shiftRequest = new ShiftRequest
            (
                doctor: doctor,
                shiftStart: request.StartTime,
                shiftEnd: request.EndTime,
                patientLimitPerSlot: request.PatientLimitPerSlot,
                reason: request.reason
            );
            doctor.AddShiftRequest(shiftRequest);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new RequestedShiftDto
            {
                Id = shiftRequest.Id,
                DoctorId = doctor.Id,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                PatientLimitPerSlot = request.PatientLimitPerSlot,
                Reason = request.reason,
                Status = shiftRequest.Status
            };
        }
    }
}
