using Clinic.Domain.Entities;

namespace Clinic.Application.Interfaces
{
    public interface ITokenProvider
    {
        string GenerateAccessToken(User user);
        string GenerateRefreshToken();
        string HashToken(string refreshToken);
    }
}
