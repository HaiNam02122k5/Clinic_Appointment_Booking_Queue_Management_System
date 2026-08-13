using Clinic.Application.Common.Exceptions;
using Clinic.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.Features.Appointments.Commands
{
    public record PatientCancelAppointmentCommand(
        Guid AppointmentId,
        Guid UserId
    ) : IRequest<Guid>;
    public class PatientCancelAppointmentCommandHandler : IRequestHandler<PatientCancelAppointmentCommand, Guid>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IUnitOfWork _unitOfWork;

        public PatientCancelAppointmentCommandHandler(IAppointmentRepository appointmentRepository, IPatientRepository patientRepository, IUnitOfWork unitOfWork)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Guid> Handle(PatientCancelAppointmentCommand request, CancellationToken cancellationToken)
        {
            var patient = await _patientRepository.GetPatientByUserIdAsync(request.UserId);
            if (patient == null)
            {
                throw new NotFoundException("Patient not found.");
            }
            var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId);
            if (appointment == null)
            {
                throw new NotFoundException("Appointment not found.");
            }
            if (appointment.PatientId != patient.Id)
            {
                throw new UnauthorizedAccessException("You are not authorized to cancel this appointment.");
            }
            appointment.Cancel();
            await _unitOfWork.SaveChangesAsync();
            return request.AppointmentId;
        }
    }
}
