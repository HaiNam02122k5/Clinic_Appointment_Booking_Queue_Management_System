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
        Guid UserId
    ) : IRequest<Guid>;
    public class CancelAppointmentCommandHandler : IRequestHandler<CancelAppointmentCommand, Guid>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CancelAppointmentCommandHandler(IAppointmentRepository appointmentRepository, IUserRepository userRepository, IUnitOfWork unitOfWork)
        {
            _appointmentRepository = appointmentRepository;
            _userRepository = userRepository;
            _unitOfWork = unitOfWork;
        }
        public async Task<Guid> Handle(CancelAppointmentCommand request, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                throw new UnauthorizedAccessException("User not found.");
            }
            var appointment = await _appointmentRepository.GetByIdAsync(request.AppointmentId);
            if (appointment == null)
            {
                throw new NotFoundException("Appointment not found.");
            }
            if (!(user.UserRoles.Any(ur => ur.Role.Name == "Receptionist") || user.Person.Patient?.Id == appointment.PatientId))
            {
                throw new ForbiddenException("You are not authorized to cancel this appointment.");
            }
            appointment.Cancel(request.UserId);
            await _unitOfWork.SaveChangesAsync();
            return request.AppointmentId;
        }
    }
}
