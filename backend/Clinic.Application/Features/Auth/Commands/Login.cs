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
        private readonly UserService _userService;
        private readonly ITokenProvider _tokenProvider;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUnitOfWork _unitOfWork;

        public LoginCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher, ITokenProvider tokenProvider, IRefreshTokenRepository refreshTokenRepository, IUnitOfWork unitOfWork)
        {
            _userService = new UserService(userRepository, passwordHasher);
            _tokenProvider = tokenProvider;
            _refreshTokenRepository = refreshTokenRepository;
            _unitOfWork = unitOfWork;
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

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new LoginResponse(
                AccessToken: accessToken,
                RefreshToken: refreshToken
            );
        }
    }
}
