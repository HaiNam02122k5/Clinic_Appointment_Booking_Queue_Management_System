using Clinic.Application.Common.Exceptions;
using Clinic.Application.Contracts;
using Clinic.Application.Interfaces;
using Clinic.Domain.Common.Exceptions;
using Clinic.Domain.Entities;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands
{
    // Use-case: Patient updates an existing appointment
    public record PatientUpdateAppointmentCommand(
        Guid UserId,
        Guid AppointmentId,
        Guid NewWorkScheduleId,
        TimeOnly TimeSlot
    ) : IRequest<AppointmentDto>;
    public class PatientUpdateAppointmentCommandHandler : IRequestHandler<PatientUpdateAppointmentCommand, AppointmentDto>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IWorkScheduleRepository _workScheduleRepository;
        private readonly IUnitOfWork _unitOfWork;
        public PatientUpdateAppointmentCommandHandler(IAppointmentRepository appointmentRepository, IPatientRepository patientRepository, IWorkScheduleRepository workScheduleRepository, IUnitOfWork unitOfWork)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _workScheduleRepository = workScheduleRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<AppointmentDto> Handle(PatientUpdateAppointmentCommand request, CancellationToken cancellationToken)
        {
            var patient = await _patientRepository.GetPatientByUserIdAsync(request.UserId);
            if (patient == null)
            {
                throw new NotFoundException($"Patient not found.");
            }
            var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId);
            if (appointment == null)
            {
                throw new NotFoundException($"Appointment not found.");
            }
            if (appointment.PatientId != patient.Id)
            {
                throw new UnauthorizedAccessException($"Patient is not authorized to update this appointment.");
            }
            // Begin transaction
            await _unitOfWork.InitializeTransactionLockAsync(cancellationToken);
            try
            {
                var workSchedule = await _workScheduleRepository.GetWorkScheduleByIdAsync(request.NewWorkScheduleId);
                if (workSchedule == null)
                {
                    throw new NotFoundException($"Work schedule not found.");
                }
                appointment.Update(workSchedule, request.TimeSlot);
                workSchedule.AddAppointment(appointment);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
                await _unitOfWork.CommitTransactionAsync(cancellationToken);
                return new AppointmentDto
                {
                    Id = appointment.Id,
                    PatientId = patient.Id,
                    DoctorId = workSchedule.DoctorId,
                    PatientName = patient.Person.FullName,
                    DoctorName = workSchedule.Doctor.Employee.Person.FullName,
                    TimeSlot = request.TimeSlot,
                    Date = workSchedule.Date,
                    Status = appointment.Status,
                    CreatedAt = appointment.CreatedAt
                };
            }
            catch (Exception ex)
            {
                // For unique constraint violations as 2 appointments cannot be created for the same time slot
                await _unitOfWork.RollbackTransactionAsync(cancellationToken);
                if (ex is ConflictException)
                {
                    throw new ConflictException("An appointment already exists for this time slot.");
                }
                throw;
            }
        }
    }
}
