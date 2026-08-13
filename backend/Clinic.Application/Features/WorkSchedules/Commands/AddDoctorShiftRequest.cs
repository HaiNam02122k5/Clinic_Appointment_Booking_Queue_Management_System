using Clinic.Application.Common.Exceptions;
using Clinic.Application.Contracts;
using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using MediatR;

namespace Clinic.Application.Features.WorkSchedules.Commands
{
    // Use-case: Doctor adds a new shift request
    public record AddDoctorShiftRequestCommand(Guid DoctorId, DateOnly Date, TimeOnly StartTime, TimeOnly EndTime, int PatientLimit, string reason) : IRequest<RequestedShiftDto>;

    public class AddDoctorShiftRequestCommandHandler : IRequestHandler<AddDoctorShiftRequestCommand, RequestedShiftDto>
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IWorkScheduleRepository _workScheduleRepository;
        private readonly IUnitOfWork _unitOfWork;
        public AddDoctorShiftRequestCommandHandler(IDoctorRepository doctorRepository, IWorkScheduleRepository workScheduleRepository, IUnitOfWork unitOfWork)
        {
            _doctorRepository = doctorRepository;
            _workScheduleRepository = workScheduleRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<RequestedShiftDto> Handle(AddDoctorShiftRequestCommand request, CancellationToken cancellationToken)
        {
            var doctor = await _doctorRepository.GetInfoByIdAsync(request.DoctorId);
            if (doctor == null)
            {
                throw new NotFoundException("Doctor not found.");
            }
            if (await _workScheduleRepository.HasDuplicateShiftRequest(request.DoctorId, request.Date, request.StartTime, request.EndTime))
            {
                throw new InvalidOperationException("The doctor has a duplicate shift request in the specified time range.");
            }
            var shiftRequest = new ShiftRequest
            (
                doctor: doctor,
                date: request.Date,
                shiftStart: request.StartTime,
                shiftEnd: request.EndTime,
                patientLimit: request.PatientLimit,
                reason: request.reason
            );
            doctor.AddShiftRequest(shiftRequest);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new RequestedShiftDto
            {
                Id = shiftRequest.Id,
                DoctorId = doctor.Id,
                Date = request.Date,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                PatientLimit = request.PatientLimit,
                Reason = request.reason,
                Status = shiftRequest.Status
            };
        }
    }
}
