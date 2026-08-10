using Clinic.Application.Interfaces;
using Clinic.Application.Services;
using Clinic.Domain.Entities;
using MediatR;

namespace Clinic.Application.Features.Auth.Commands
{
    public record LoginCommand(string Username, string Password) : IRequest<LoginResponse>;

    public record LoginResponse(
        string AccessToken,
        string RefreshToken
    );

    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IUserService _userService;
        private readonly ITokenProvider _tokenProvider;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public LoginCommandHandler(IUserService userService, ITokenProvider tokenProvider, IRefreshTokenRepository refreshTokenRepository)
        {
            _userService = userService;
            _tokenProvider = tokenProvider;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<LoginResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            var user = await _userService.VerifyUser(command.Username, command.Password);

            var accessToken = _tokenProvider.GenerateAccessToken(user);

            var refreshToken = _tokenProvider.GenerateRefreshToken();
            while (await _refreshTokenRepository.GetByTokenHashAsync(_tokenProvider.HashToken(refreshToken)) != null)
            {
                refreshToken = _tokenProvider.GenerateRefreshToken();
            }
            await _refreshTokenRepository.AddAsync(
                new RefreshToken
                (
                    userId: user.Id,
                    tokenHash: _tokenProvider.HashToken(refreshToken),
                    expiresAt: DateTime.UtcNow.AddDays(7)
                )
            );

            return new LoginResponse(
                AccessToken: accessToken,
                RefreshToken: refreshToken
            );
        }
    }
}
