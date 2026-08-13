using Clinic.Application.Common.Exceptions;
using Clinic.Application.Contracts;
using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using MediatR;

namespace Clinic.Application.Features.Appointments.Queries
{
    // Use-case: Patient gets their appointments
    public record GetPatientAppointmentsQuery(
        Guid UserId,
        string Category = "upcoming"
    ) : IRequest<List<AppointmentDto>>;
    public class GetPatientAppointmentsQueryHandler : IRequestHandler<GetPatientAppointmentsQuery, List<AppointmentDto>>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;

        public GetPatientAppointmentsQueryHandler(IAppointmentRepository appointmentRepository, IPatientRepository patientRepository)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
        }

        public async Task<List<AppointmentDto>> Handle(GetPatientAppointmentsQuery request, CancellationToken cancellationToken)
        {
            var patient = await _patientRepository.GetPatientByUserIdAsync(request.UserId);
            if (patient == null)
            {
                throw new NotFoundException("Patient not found.");
            }

            var appointments = await _appointmentRepository.GetAppointmentsByPatientIdAsync(patient.Id, request.Category);
            return appointments.Items.Select(a => new AppointmentDto
                {
                    Id = a.Id,
                    PatientId = a.PatientId,
                    DoctorId = a.WorkSchedule.DoctorId,
                    PatientName = a.Patient.Person.FullName,
                    DoctorName = a.WorkSchedule.Doctor.Employee.Person.FullName,
                    TimeSlot = a.TimeSlot,
                    Date = a.WorkSchedule.Date,
                    Status = a.Status,
                    CreatedAt = a.CreatedAt
                }).ToList();
        }
    }
}