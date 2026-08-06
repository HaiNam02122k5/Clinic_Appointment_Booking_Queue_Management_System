using Clinic.Domain.Entities;
using Clinic.Domain.Interfaces;
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
        private readonly IPasswordHasher _passwordHasher;
        private readonly ITokenHasher _tokenHasher;
        private readonly IUserRepository _userRepository;
        private readonly ITokenProvider _tokenProvider;
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public LoginCommandHandler(IPasswordHasher passwordHasher, ITokenHasher tokenHasher, IUserRepository userRepository, ITokenProvider tokenProvider, IRefreshTokenRepository refreshTokenRepository)
        {
            _passwordHasher = passwordHasher;
            _tokenHasher = tokenHasher;
            _userRepository = userRepository;
            _tokenProvider = tokenProvider;
            _refreshTokenRepository = refreshTokenRepository;
        }

        public async Task<LoginResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUsernameAsync(command.Username);
            if (user == null)
            {
                throw new ArgumentException("Invalid username or password.");
            }
            if (!_passwordHasher.VerifyPassword(command.Password, user.PasswordHash))
            {
                throw new ArgumentException("Invalid username or password.");
            }

            var accessToken = _tokenProvider.GenerateAccessToken(user);

            var refreshToken = _tokenProvider.GenerateRefreshToken();
            while (await _refreshTokenRepository.GetByTokenHashAsync(_tokenHasher.Hash(refreshToken)) != null)
            {
                refreshToken = _tokenProvider.GenerateRefreshToken();
            }
            await _refreshTokenRepository.AddAsync(
                new RefreshToken
                (
                    userId: user.Id,
                    tokenHash: _tokenHasher.Hash(refreshToken),
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
