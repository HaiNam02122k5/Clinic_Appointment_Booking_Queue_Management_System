using Clinic.Application.Common.Exceptions;
using Clinic.Application.Contracts;
using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using MediatR;
using Clinic.Domain.Common.Exceptions;

namespace Clinic.Application.Features.Appointments.Commands
{
    // Use-case: Patient creates an appointment
    public record PatientCreateAppointmentCommand(
        Guid UserId,
        Guid WorkScheduleId,
        TimeOnly TimeSlot
    ) : IRequest<AppointmentDto>;
    public class PatientCreateAppointmentCommandHandler : IRequestHandler<PatientCreateAppointmentCommand, AppointmentDto>
    {
        private readonly IWorkScheduleRepository _workScheduleRepository;
        private readonly IPatientRepository _petientRepository;
        private readonly IUnitOfWork _unitOfWork;
        public PatientCreateAppointmentCommandHandler(IWorkScheduleRepository workScheduleRepository, IPatientRepository patientRepository, IUnitOfWork unitOfWork)
        {
            _workScheduleRepository = workScheduleRepository;
            _petientRepository = patientRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<AppointmentDto> Handle(PatientCreateAppointmentCommand request, CancellationToken cancellationToken)
        {
            var patient = await _petientRepository.GetPatientByUserIdAsync(request.UserId);
            if (patient == null) {
                throw new NotFoundException($"Patient not found.");
            }
            // Begin transaction lock to prevent race conditions (active appointment count > patientLimit) when creating appointments
            await _unitOfWork.InitializeTransactionLockAsync(cancellationToken);
            try
            {
                var workSchedule = await _workScheduleRepository.GetWorkScheduleByIdAsync(request.WorkScheduleId);
                if (workSchedule == null)
                {
                    throw new NotFoundException($"Work schedule not found.");
                }
                var appointment = new Appointment(patient, workSchedule, request.TimeSlot);
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
