using Clinic.Domain.Entities;
using Clinic.Domain.Interfaces;
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
            var model = MapToDataModel(refreshToken);
            await _context.RefreshTokens.AddAsync(model);
            await _context.SaveChangesAsync();
            return MapToDomain(model);
        }

        private RefreshToken? MapToDomain(RefreshTokenDataModel model)
        {
            if (model == null)
            {
                return null;
            }
            return new RefreshToken(model.Id, model.TokenHash, model.IssuedAt, model.ExpiresAt, model.RevokedAt, model.UserId);
        }

        private RefreshTokenDataModel MapToDataModel(RefreshToken refreshToken)
        {
            return new RefreshTokenDataModel
            {
                Id = refreshToken.Id,
                TokenHash = refreshToken.TokenHash,
                UserId = refreshToken.UserId,
                ExpiresAt = refreshToken.ExpiresAt,
                IssuedAt = refreshToken.IssuedAt,
                RevokedAt = refreshToken.RevokedAt
            };
        }

        public async Task<RefreshToken?> GetByTokenHashAsync(string tokenHash)
        {
            var model = await _context.RefreshTokens.Include(rt => rt.User).FirstOrDefaultAsync(rt => rt.TokenHash == tokenHash);
            return MapToDomain(model);
        }

        public async Task<RefreshToken> UpdateAsync(RefreshToken refreshToken)
        {
            var model = await _context.RefreshTokens.FindAsync(refreshToken.Id);
            if (model == null)
            {
                throw new InvalidOperationException("Refresh token not found");
            }

            model.RevokedAt = refreshToken.RevokedAt;

            _context.RefreshTokens.Update(model);
            await _context.SaveChangesAsync();
            return MapToDomain(model);
        }
    }
}
