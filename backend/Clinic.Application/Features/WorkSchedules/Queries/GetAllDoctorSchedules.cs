using Clinic.Application.Contracts;
using Clinic.Application.Interfaces;
using MediatR;

namespace Clinic.Application.Features.WorkSchedules.Queries
{
    public record GetAllDoctorSchedulesQuery(
        DateOnly StartDate,
        DateOnly EndDate
    ) : IRequest<IEnumerable<DoctorScheduleDto<WorkScheduleDto>>>;

    public class GetAllDoctorSchedulesHandler
        : IRequestHandler<
            GetAllDoctorSchedulesQuery,
            IEnumerable<DoctorScheduleDto<WorkScheduleDto>>
        >
    {
        private readonly IDoctorRepository _doctorRepository;
        private readonly IWorkScheduleRepository _workScheduleRepository;

        public GetAllDoctorSchedulesHandler(
            IDoctorRepository doctorRepository,
            IWorkScheduleRepository workScheduleRepository
        )
        {
            _doctorRepository = doctorRepository;
            _workScheduleRepository = workScheduleRepository;
        }

        public async Task<IEnumerable<DoctorScheduleDto<WorkScheduleDto>>> Handle(
            GetAllDoctorSchedulesQuery request,
            CancellationToken cancellationToken
        )
        {
            if (request.StartDate.AddMonths(1) < request.EndDate)
            {
                throw new ArgumentException(
                    "The date range cannot exceed one month"
                );
            }

            // Lấy tất cả bác sĩ đang Active
            var doctors = await _doctorRepository
                .GetActiveDoctorsBySpecialty(null);

            var result =
                new List<DoctorScheduleDto<WorkScheduleDto>>();

            foreach (var doctor in doctors)
            {
                var schedules = await _workScheduleRepository
                    .GetPlannedSchedulesByDoctorIdAsync(
                        doctor.Id,
                        request.StartDate,
                        request.EndDate
                    );

                var scheduleDto =
                    new DoctorScheduleDto<WorkScheduleDto>
                    {
                        DoctorId = doctor.Id,

                        DoctorName =
                            doctor.Employee.Person.FullName,

                        Schedules = schedules
                            .Select(ws => new WorkScheduleDto
                            {
                                Id = ws.Id,
                                DoctorId = ws.DoctorId,
                                Date = ws.Date,
                                StartTime = ws.ShiftStart,
                                EndTime = ws.ShiftEnd,
                                PatientLimit = ws.PatientLimit,
                                Status = ws.Status
                            })
                            .ToList(),

                        startDate = request.StartDate,
                        endDate = request.EndDate
                    };

                result.Add(scheduleDto);
            }

            return result;
        }
    }
}