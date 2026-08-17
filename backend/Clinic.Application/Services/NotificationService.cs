using Clinic.Application.Interfaces;
using Clinic.Application.Notifications;
using Clinic.Application.Notifications.Dispatchers;
using Clinic.Application.Notifications.Interfaces;
using Clinic.Application.Notifications.Templates;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;
using Microsoft.EntityFrameworkCore.Storage.Json;

namespace Clinic.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IAppointmentHandler _appointmentHandler;

        public NotificationService(
            IAppointmentHandler appointmentHandler,
            IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
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
    }
}
