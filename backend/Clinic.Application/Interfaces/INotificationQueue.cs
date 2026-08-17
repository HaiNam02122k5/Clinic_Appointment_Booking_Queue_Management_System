using Clinic.Domain.Enums;

namespace Clinic.Application.Interfaces
{
    public record NotificationJob(
        Guid PersonId,
        string? PhoneNumber,
        string? Email,
        NotificationType NotificationType,
        string Title,
        string Content,
        bool SendEmail,
        bool SendInApp,
        bool SendSms
    );

    public interface INotificationQueue
    {
        ValueTask EnqueueAsync(
            NotificationJob job,
            CancellationToken cancellationToken = default);
    }
}
