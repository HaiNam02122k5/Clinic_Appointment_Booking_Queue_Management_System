namespace Clinic.Application.Interfaces
{
    public interface INotificationService
    {
        Task SendAsync(
            INotificationJob job,
            CancellationToken cancellationToken = default);
    }
}
