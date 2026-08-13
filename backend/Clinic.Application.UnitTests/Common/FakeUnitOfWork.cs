using Clinic.Application.Interfaces;

namespace Clinic.Application.UnitTests.Common
{
    public class FakeUnitOfWork : IUnitOfWork
    {
        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            return;
        }

        public async Task InitializeTransactionLockAsync(CancellationToken cancellationToken = default)
        {
            return;
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            return;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return 0;
        }
    }
}
