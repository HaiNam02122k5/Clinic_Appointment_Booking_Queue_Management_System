using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using Clinic.Infrastructure.Sqlserver.Models;
using Clinic.Infrastructure.Sqlserver.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Clinic.Infrastructure.Sqlserver.Repositories
{
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationDbContext _context;
        public RefreshTokenRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<RefreshToken> AddAsync(RefreshToken refreshToken)
        {
            await _context.RefreshTokens.AddAsync(refreshToken);
            await _context.SaveChangesAsync();
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Id == refreshToken.Id);
        }

        //private RefreshToken? MapToDomain(RefreshTokenDataModel model)
        //{
        //    if (model == null)
        //    {
        //        return null;
        //    }
        //    return new RefreshToken(model.Id, model.TokenHash, model.IssuedAt, model.ExpiresAt, model.RevokedAt, model.UserId);
        //}

        //private RefreshTokenDataModel MapToDataModel(RefreshToken refreshToken)
        //{
        //    return new RefreshTokenDataModel
        //    {
        //        Id = refreshToken.Id,
        //        TokenHash = refreshToken.TokenHash,
        //        UserId = refreshToken.UserId,
        //        ExpiresAt = refreshToken.ExpiresAt,
        //        IssuedAt = refreshToken.IssuedAt,
        //        RevokedAt = refreshToken.RevokedAt
        //    };
        //}

        public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash)
        {
            return await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);
        }

        public async Task UpdateAsync(RefreshToken refreshToken)
        {

            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();
        }
    }
}
