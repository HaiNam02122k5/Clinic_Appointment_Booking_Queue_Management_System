namespace Clinic.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendAsync(
            NotificationJob job,
            CancellationToken cancellationToken = default);
    }
}
