using Clinic.Application.Interfaces;

namespace Clinic.Application.UnitTests.Common
{
    public class FakeUnitOfWork : IUnitOfWork
    {
        public Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task InitializeTransactionLockAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return 0;
        }
    }
}
