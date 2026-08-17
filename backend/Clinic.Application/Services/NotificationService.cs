using Clinic.Application.Interfaces;
using Clinic.Application.Notifications.Interfaces;
using Clinic.Domain.Entities;

namespace Clinic.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppointmentNotificatinHandler _appointmentHandler;
        private readonly IDueNotificationHandler _dueHandler ;

        public NotificationService(
            IAppointmentNotificatinHandler appointmentHandler,
            IDueNotificationHandler dueHandler,
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
            _dueHandler = dueHandler;
            _appointmentHandler = appointmentHandler;
        }

        public async Task SendAsync(
            INotificationJob job,
            CancellationToken cancellationToken = default)
        {
            switch (job)
            {
                case NotificationJob<Appointment> appointmentJob:
                    await _appointmentHandler.HandleAsync(appointmentJob, cancellationToken);
                    break;
                default:
                    throw new NotSupportedException($"Notification job type '{job.GetType().Name}' is not supported.");
            }

            // Each dispatcher will handle the adding of the notification to the repository
            // Save the notification to the repository
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task ScheduleAsync(
            INotificationJob job,
            DateTimeOffset scheduledTime,
            CancellationToken cancellationToken = default)
        {
            switch (job)
            {
                case NotificationJob<Appointment> appointmentJob:
                    await _appointmentHandler.HandleAsync(appointmentJob, cancellationToken, scheduledTime);
                    break;
                default:
                    throw new NotSupportedException($"Notification job type '{job.GetType().Name}' is not supported.");
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        public async Task ProcessDueNotificationsAsync(CancellationToken stoppingToken)
        {
            await _dueHandler.ProcessAsync(stoppingToken);
            await _unitOfWork.SaveChangesAsync(stoppingToken);
        }
    }
}
