using Clinic.Application.Common.Exceptions;
using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using Clinic.Domain.Common.Exceptions;
using MediatR;

namespace Clinic.Application.Features.Appointments.Commands
{
    public record CreateAppointmentCommand(Guid WorkScheduleId, DateTime TimeSlot, string? Reason) : IRequest<Guid>;

    public class CreateAppointmentHandler : IRequestHandler<CreateAppointmentCommand, Guid>
    {
        private readonly IWorkScheduleRepository _workScheduleRepository;
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IUnitOfWork _unitOfWork;

        public CreateAppointmentHandler(
            IWorkScheduleRepository workScheduleRepository,
            IAppointmentRepository appointmentRepository,
            ICurrentUser currentUser,
            IUnitOfWork unitOfWork)
        {
            _workScheduleRepository = workScheduleRepository;
            _appointmentRepository = appointmentRepository;
            _currentUser = currentUser;
            _unitOfWork = unitOfWork;
        }

        public async Task<Guid> Handle(CreateAppointmentCommand command, CancellationToken cancellationToken)
        {
            // Chỉ tài khoản đã có hồ sơ Patient mới được tự đặt lịch online.
            if (_currentUser.PatientId is null)
            {
                throw new ForbiddenException("Only a patient account can book an appointment.");
            }

            var workSchedule = await _workScheduleRepository.GetByIdAsync(command.WorkScheduleId)
                ?? throw new NotFoundException($"Work schedule '{command.WorkScheduleId}' not found.");

            if (workSchedule.Status != WorkScheduleStatus.Active)
            {
                throw new ConflictException("This shift is no longer accepting appointments.");
            }

            if (command.TimeSlot < workSchedule.ShiftStart || command.TimeSlot > workSchedule.ShiftEnd)
            {
                throw new ArgumentException("The selected time slot is outside the doctor's shift.");
            }

            if (command.TimeSlot <= DateTime.UtcNow)
            {
                throw new ArgumentException("Time slot must be in the future.");
            }

            // Số lịch hẹn còn "sống" (chưa hủy) trong khung giờ này không được vượt quá giới hạn.
            var bookedCount = workSchedule.Appointments.Count(a => a.Status != AppointmentStatus.Cancelled);
            if (bookedCount >= workSchedule.PatientLimitPerSlot)
            {
                throw new ConflictException("This shift is already fully booked.");
            }

            var appointment = new Appointment
            {
                PatientId = _currentUser.PatientId.Value,
                WorkScheduleId = workSchedule.Id,
                TimeSlot = command.TimeSlot,
                Reason = command.Reason,
                IsWalkIn = false
            };

            await _appointmentRepository.AddAsync(appointment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return appointment.Id;
        }
    }
}