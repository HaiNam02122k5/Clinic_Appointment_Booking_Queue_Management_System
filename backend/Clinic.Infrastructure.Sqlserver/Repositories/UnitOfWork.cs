using Clinic.Application.Common.Exceptions;
using Clinic.Application.Interfaces;
using Clinic.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Sqlserver.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                return await _context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                // RowVersion không khớp - bản ghi đã bị request khác cập nhật trước.
                // Dịch sang exception riêng của Application để Infrastructure (EF Core)
                // không rò rỉ lên Handler, giữ đúng ranh giới Clean Architecture.
                throw new ConcurrencyConflictException(
                    "Dữ liệu đã bị thay đổi bởi thao tác khác, vui lòng thử lại.");
            }
        }
    }
}