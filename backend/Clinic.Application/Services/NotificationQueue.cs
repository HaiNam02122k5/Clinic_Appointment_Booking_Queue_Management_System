using Clinic.Application.Interfaces;
using System.Threading.Channels;

namespace Clinic.Application.Services
{
    public class NotificationQueue : INotificationQueue
    {
        private readonly Channel<NotificationJob> _queue =
            Channel.CreateUnbounded<NotificationJob>();

        public ValueTask EnqueueAsync(
            NotificationJob job,
            CancellationToken cancellationToken = default)
        {
            return _queue.Writer.WriteAsync(job, cancellationToken);
        }

        public IAsyncEnumerable<NotificationJob> ReadAllAsync(
            CancellationToken cancellationToken)
        {
            return _queue.Reader.ReadAllAsync(cancellationToken);
        }
    }
}
