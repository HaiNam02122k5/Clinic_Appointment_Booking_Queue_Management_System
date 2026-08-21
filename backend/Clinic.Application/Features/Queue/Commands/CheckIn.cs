using Clinic.Application.Common.Exceptions;
using Clinic.Application.Contracts;
using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using MediatR;

namespace Clinic.Application.Features.Queue.Commands
{
    public record CheckInCommand(Guid AppointmentId) : IRequest<QueueTicketBriefDto>;

    public class CheckInHandler : IRequestHandler<CheckInCommand, QueueTicketBriefDto>
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly IQueueTicketRepository _queueTicketRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CheckInHandler(
            IAppointmentRepository appointmentRepository,
            IQueueTicketRepository queueTicketRepository,
            IUnitOfWork unitOfWork)
        {
            _appointmentRepository = appointmentRepository;
            _queueTicketRepository = queueTicketRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<QueueTicketBriefDto> Handle(CheckInCommand command, CancellationToken cancellationToken)
        {
            var appointment = await _appointmentRepository.GetByIdAsync(command.AppointmentId)
                ?? throw new NotFoundException($"Appointment '{command.AppointmentId}' not found.");


            var doctorId = appointment.WorkSchedule.DoctorId;
            var checkInTime = DateTime.UtcNow;
            var queueNumber = await _queueTicketRepository.GetNextQueueNumberAsync(doctorId, checkInTime.Date, cancellationToken);
            var queueTicket = appointment.CheckIn(checkInTime, queueNumber);

            await _queueTicketRepository.AddAsync(queueTicket);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return new QueueTicketBriefDto
            {
                Id = queueTicket.Id,
                AppointmentId = appointment.Id,
                DoctorName = appointment.WorkSchedule.Doctor.Employee.Person.FullName,
                PatientName = appointment.Patient.Person.FullName,
                SpecialtyName = appointment.WorkSchedule.Doctor.WorkHistories.FirstOrDefault(wh => wh.EndDate == null)?.Specialty?.Name ?? "Unknown", // Ticket for now, so we can assume the current specialty is the one with no end date at that time
                QueueNumber = queueTicket.QueueNumber,
                CheckInTime = queueTicket.CheckInTime,
            };
        }
    }
}