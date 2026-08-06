using Clinic.Application.Features.Auth.DTOs;
using Clinic.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace Clinic.Application.Features.Auth.Commands
{
    public record LoginCommand(string Username, string Password) : IRequest<LoginResponse>;

    public record LoginResponse(
        string AccessToken,
        string RefreshToken,
        DateTime AccessTokenExpiresAt,
        UserInfo User
    );

    public class LoginCommandHandler : IRequestHandler<LoginCommand, LoginResponse>
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserRepository _userRepository;
        private readonly ITokenProvider _tokenProvider;

        public LoginCommandHandler(IPasswordHasher passwordHasher, IUserRepository userRepository, ITokenProvider tokenProvider)
        {
            _passwordHasher = passwordHasher;
            _userRepository = userRepository;
            _tokenProvider = tokenProvider;
        }

        public async Task<LoginResponse> Handle(LoginCommand command, CancellationToken cancellationToken)
        {
            var user = await _userRepository.GetByUsernameAsync(command.Username);
            if (user == null)
            {
                throw new Exception("Invalid username or password.");
            }

            if (!_passwordHasher.VerifyPassword(command.Password, user.PasswordHash))
            {
                throw new Exception("Invalid username or password.");
            }

            var accessToken = _tokenProvider.GenerateAccessToken(user);
            var refreshToken = _tokenProvider.GenerateRefreshToken(user);

            return new LoginResponse(
                AccessToken: accessToken,
                RefreshToken: refreshToken,
                AccessTokenExpiresAt: DateTime.UtcNow.AddHours(1),
                User: new UserInfo(
                    Id: user.Id,
                    Username: user.Username,
                    FullName: user.Person.FullName
                )
            );
        }
    }
}
