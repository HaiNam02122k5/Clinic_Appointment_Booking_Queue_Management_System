using Clinic.Domain.Entities;
using System;
using System.Collections.Generic;
namespace Clinic.Application.Interfaces
{
    public interface IRefreshTokenRepository
    {
        Task AddAsync(RefreshToken refreshToken);
        Task UpdateAsync(RefreshToken refreshToken);
        Task<RefreshToken?> GetByTokenHashAsync(string tokenHash);
    }
}
