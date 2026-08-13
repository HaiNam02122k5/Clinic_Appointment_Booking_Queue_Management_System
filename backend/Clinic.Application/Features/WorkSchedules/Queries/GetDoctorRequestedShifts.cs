using Clinic.Application.Common.Exceptions;
using Clinic.Application.Contracts;
using Clinic.Application.Interfaces;
using MediatR;

namespace Clinic.Application.Features.WorkSchedules.Queries
{
    // Use-case: Get a list of doctor schedules
    public record GetDoctorRequestedShiftsQuery(
        Guid DoctorId,
        DateOnly StartDate,
        DateOnly EndDate
    ) : IRequest<DoctorScheduleDto<RequestedShiftDto>>;
    public class GetDoctorRequestedShiftsHandler : IRequestHandler<GetDoctorRequestedShiftsQuery, DoctorScheduleDto<RequestedShiftDto>>
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IWorkScheduleRepository _workScheduleRepository;
        public GetDoctorRequestedShiftsHandler(IDoctorRepository doctorRepository, IWorkScheduleRepository workScheduleRepository)
        {
            _doctorRepository = doctorRepository;
            _workScheduleRepository = workScheduleRepository;
        }

        public async Task<DoctorScheduleDto<RequestedShiftDto>> Handle(GetDoctorRequestedShiftsQuery request, CancellationToken cancellationToken)
        {
            if (request.StartDate.AddMonths(1) < request.EndDate)
            {
                throw new ArgumentException("The date range cannot exceed one month");
            }
            var doctor = await _doctorRepository.GetInfoByIdAsync(request.DoctorId);
            if (doctor == null)
            {
                throw new NotFoundException("Doctor not found");
            }
            var schedules = await _workScheduleRepository.GetRequestedSchedulesByDoctorIdAsync(request.DoctorId, request.StartDate, request.EndDate);

            // Map the doctor's schedules to the DTO
            var scheduleDto = new DoctorScheduleDto<RequestedShiftDto>
            {
                DoctorId = doctor.Id,
                DoctorName = doctor.Employee.Person.FullName,
                Schedules = schedules.Select(sr => new RequestedShiftDto
                {
                    Id = sr.Id,
                    DoctorId = sr.DoctorId,
                    Date = sr.Date,
                    StartTime = sr.ShiftStart,
                    EndTime = sr.ShiftEnd,
                    PatientLimit = sr.PatientLimit,
                    Reason = sr.Reason,
                    Status = sr.Status
                }).ToList(),
                startDate = request.StartDate,
                endDate = request.EndDate
            };

            return scheduleDto;
        }
    }
}
