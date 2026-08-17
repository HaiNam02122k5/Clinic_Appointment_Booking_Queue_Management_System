using Clinic.Application.Interfaces;
using Clinic.Application.Services;

namespace Clinic.API.Workers
{
    public class NotificationWorker : BackgroundService
    {
        private readonly NotificationQueue _queue;
        private readonly IServiceScopeFactory _scopeFactory;

        public NotificationWorker(
            NotificationQueue queue,
            IServiceScopeFactory scopeFactory)
        {
            _queue = queue;
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            await foreach (
                var job in _queue.ReadAllAsync(stoppingToken))
            {
                using var scope = _scopeFactory.CreateScope();

                var notificationService =
                    scope.ServiceProvider
                        .GetRequiredService<INotificationService>();

                await notificationService.SendAsync(
                    job,
                    stoppingToken);
            }
        }
    }
}
