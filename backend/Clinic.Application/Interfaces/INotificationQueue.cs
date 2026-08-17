using Clinic.Domain.Entities;
using Clinic.Domain.Enums;

namespace Clinic.Application.Interfaces
{
    public interface INotificationJob
    {
        Person Person { get; }
        NotificationType NotificationType { get; }
        bool SendEmail { get; }
        bool SendInApp { get; }
        bool SendSms { get; }
    }
    public record NotificationJob<T>(
        Person Person,
        T Data,
        NotificationType NotificationType,
        bool SendEmail,
        bool SendInApp,
        bool SendSms
    ) : INotificationJob;

    public interface INotificationQueue
    {
        ValueTask EnqueueAsync(
            INotificationJob job,
            CancellationToken cancellationToken = default);
    }
}
