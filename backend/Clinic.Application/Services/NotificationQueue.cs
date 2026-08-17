using Clinic.Application.Interfaces;
using System.Threading.Channels;

namespace Clinic.Application.Services
{
    public class NotificationQueue : INotificationQueue
    {
        private readonly Channel<INotificationJob> _queue =
            Channel.CreateUnbounded<INotificationJob>();

        public ValueTask EnqueueAsync(
            INotificationJob job,
            CancellationToken cancellationToken = default)
        {
            return _queue.Writer.WriteAsync(job, cancellationToken);
        }

        public IAsyncEnumerable<INotificationJob> ReadAllAsync(
            CancellationToken cancellationToken)
        {
            return _queue.Reader.ReadAllAsync(cancellationToken);
        }
    }
}
