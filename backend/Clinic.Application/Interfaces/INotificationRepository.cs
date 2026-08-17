using Clinic.Domain.Entities;

namespace Clinic.Application.Interfaces
{
    public interface INotificationRepository
    {
        /// <summary>
        /// Adds a new notification to the repository.
        /// </summary>
        Task AddAsync(Notification notification, CancellationToken cancellationToken);

        /// <summary>
        /// Retrieves a notification by its unique identifier, including the associated person and user information for lookup and resend.
        /// </summary>
        Task<Notification?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    }
}
