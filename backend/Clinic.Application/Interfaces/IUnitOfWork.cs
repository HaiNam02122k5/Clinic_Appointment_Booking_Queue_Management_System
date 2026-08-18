namespace Clinic.Application.Interfaces
{
    /// <summary>
    /// Use this to commit all changes to the database in a single transaction.
    /// </summary>
    public interface IUnitOfWork
    {
        /// <summary>
        /// Submit button.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
