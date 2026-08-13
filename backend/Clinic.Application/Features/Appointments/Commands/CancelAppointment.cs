using Clinic.Application.Common.Exceptions;
using Clinic.Application.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.Features.Appointments.Commands
{
    // Use-case: Patient or Receptionist cancels an appointment
    public record CancelAppointmentCommand(
        Guid AppointmentId,
        Guid UserId,
        Guid? PatientId = null
    ) : IRequest<Guid>;
    public class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand, Guid>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IPatientRepository _patientRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CancelAppointmentCommandHandler(IAppointmentRepository appointmentRepository, IPatientRepository patientRepository, IUnitOfWork unitOfWork)
        {
            _appointmentRepository = appointmentRepository;
            _patientRepository = patientRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Guid> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
        {
            var patient = request.PatientId == null ? await _patientRepository.GetPatientByUserIdAsync(request.UserId) : await _patientRepository.GetByIdAsync(request.PatientId);
            if (patient == null)
            {
                throw new NotFoundException("Patient not found.");
            }
            var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId);
            if (appointment == null)
            {
                throw new NotFoundException("Appointment not found.");
            }
            if (request.PatientId == null && appointment.PatientId != patient.Id)
            {
                throw new UnauthorizedAccessException("You are not authorized to cancel this appointment.");
            }
            appointment.Cancel(request.UserId);
            await _unitOfWork.SaveChangesAsync();
            return request.AppointmentId;
        }
    }
}
