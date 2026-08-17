using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using Clinic.Domain.Enums;

namespace Clinic.Application.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IEmailSender _emailSender;
        private readonly ISmsSender _smsSender;
        private readonly IInAppSender _inAppSender;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPersonRepository _personRepository;

        public NotificationService(
            IEmailSender emailSender,
            ISmsSender smsSender,
            IInAppSender inAppSender,
            INotificationRepository notificationRepository,
            IUnitOfWork unitOfWork,
            IPersonRepository personRepository)
        {
            _emailSender = emailSender;
            _smsSender = smsSender;
            _inAppSender = inAppSender;
            _notificationRepository = notificationRepository;
            _unitOfWork = unitOfWork;
            _personRepository = personRepository;
        }

        public async Task SendAsync(
            NotificationJob job,
            CancellationToken cancellationToken = default)
        {
            var person = await _personRepository.GetByIdAsync(job.PersonId);
            if (person == null)
            {
                Console.WriteLine("Person with ID {0} not found.", job.PersonId);
                return;
            }
            if (job.SendInApp && person.User != null)
            {
                var notification = new Notification(
                    job.PersonId,
                    job.NotificationType,
                    job.Content,
                    NotificationChannel.InApp);

                await _notificationRepository.AddAsync(
                    notification,
                    cancellationToken);

                notification.MarkAsSent();

                await _inAppSender.SendAsync(
                    person.User.Id,
                    job.Content,
                    cancellationToken);
            }

            if (job.SendEmail && job.Email is not null)
            {
                var notification = new Notification(
                    job.PersonId,
                    job.NotificationType,
                    job.Content,
                    NotificationChannel.Email);

                await _notificationRepository.AddAsync(
                    notification,
                    cancellationToken);

                try
                {
                    await _emailSender.SendAsync(
                        job.Email,
                        job.Title,
                        job.Content,
                        cancellationToken);
                    notification.MarkAsSent();
                }
                catch
                {
                    // Log the exception or handle it as needed
                    Console.WriteLine("Failed to send email to {0}.", job.Email);
                    notification.MarkAsFailed();
                }
            }

            if (job.SendSms && job.PhoneNumber is not null)
            {
                var notification = new Notification(
                    job.PersonId,
                    job.NotificationType,
                    job.Content,
                    NotificationChannel.Sms);
                await _notificationRepository.AddAsync(
                    notification,
                    cancellationToken);

                try
                {
                    await _smsSender.SendAsync(
                        job.PhoneNumber,
                        job.Content,
                        cancellationToken);
                    notification.MarkAsSent();
                }
                catch
                {
                    // Log the exception or handle it as needed
                    Console.WriteLine("Failed to send SMS to {0}.", job.PhoneNumber);
                    notification.MarkAsFailed();
                }
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }
}
