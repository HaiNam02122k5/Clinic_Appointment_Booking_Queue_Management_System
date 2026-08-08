using Clinic.Application.Interfaces;
using MediatR;

namespace Clinic.Application.Features.Auth.Commands
{
    public record LogoutCommand(string RefreshToken) : IRequest;
    public class LogoutCommandHandler : IRequestHandler<LogoutCommand>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly ITokenProvider _tokenProvider;
        public LogoutCommandHandler(IRefreshTokenRepository refreshTokenRepository, ITokenProvider tokenProvider)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _tokenProvider = tokenProvider;
        }

        public async Task Handle(LogoutCommand request, CancellationToken cancellationToken)
        {
            var tokenHash = _tokenProvider.HashToken(request.RefreshToken);
            var token = await _refreshTokenRepository.GetByTokenHashAsync(tokenHash);
            if (token !=  null)
            {
                token.Revoke();
                await _refreshTokenRepository.UpdateAsync(token);
            }
        }
    }
}
