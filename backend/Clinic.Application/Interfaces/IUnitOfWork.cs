using Microsoft.EntityFrameworkCore.Storage;

namespace Clinic.Application.Interfaces
{
    /// <summary>
    /// Use this to commit all changes to the database in a single transaction.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Submit button. Throws DbUpdateException if the commit fails.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Initialize a transaction lock to prevent concurrent modifications. This is useful for scenarios where you want to ensure that no other operations can modify the data while a critical operation is in progress.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task InitializeTransactionLockAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Commit the transaction. This should be called after all operations have been completed successfully. If any operation fails, the transaction should be rolled back instead of committed.
        /// Throws InvalidOperationException if no transaction has been initialized.
        /// </summary>
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Rollback the transaction. This should be called if any operation fails, to ensure that no partial changes are saved to the database.
        /// Throws InvalidOperationException if no transaction has been initialized.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
    }
}
