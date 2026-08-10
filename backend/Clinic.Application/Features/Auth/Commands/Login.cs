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
        private readonly ITokenService _tokenService;
        private readonly IUnitOfWork _unitOfWork;

        public LoginCommandHandler(IUserService userService, ITokenService tokenService, IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
        }

        public async Task<LoginResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            var user = await _userService.VerifyUser(command.Username, command.Password);
            var tokens = await _tokenService.GenerateTokensAsync(user);

            await _unitOfWork.SaveChangesAsync(cancellationToken);

            return new LoginResponse(
                AccessToken: tokens.AccessToken,
                RefreshToken: tokens.RefreshToken
            );
        }
    }
}
