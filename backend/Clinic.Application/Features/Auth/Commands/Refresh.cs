using Clinic.Application.Interfaces;
using Clinic.Domain.Entities;
using MediatR;

namespace Clinic.Application.Features.Auth.Commands
{
    public record RefreshCommand(string RefreshToken) : IRequest<RefreshResponse>;
    public record RefreshResponse(
        string AccessToken,
        string RefreshToken
    );
    public class RefreshCommandHandler : IRequestHandler<RefreshCommand, RefreshResponse>
    {
        private readonly ITokenService _tokenService;
        private readonly IUserService _userService;

        public RefreshCommandHandler(IUserService userService, ITokenService tokenService)
        {
            _userService = userService;
            _tokenService = tokenService;
        }

        public async Task<RefreshResponse> Handle(RefreshCommand request, CancellationToken cancellationToken)
        {
            var user = await _tokenService.ValidateRefreshTokenAsync(request.RefreshToken);
            var newTokens = await _tokenService.GenerateTokensAsync(user);
            return new RefreshResponse(newTokens.AccessToken, newTokens.RefreshToken);
        }
    }
}
