using Clinic.Application.Features.Auth.Commands;
using Clinic.Application.Services;
using Clinic.Application.UnitTests.Common;

namespace Clinic.Application.UnitTests.Features.Auth.Commands
{
    public class LoginTests
    {
        [Fact]
        public async Task TestLogin()
        {
            FakeUserRepository userRepository = new FakeUserRepository();
            FakePasswordHasher hasher = new FakePasswordHasher();
            FakeTokenProvider tokenProvider = new FakeTokenProvider();
            FakeRefreshTokenRepository refreshTokenRepository = new FakeRefreshTokenRepository();
            LoginCommandHandler handler = new LoginCommandHandler(new UserService(userRepository, hasher), tokenProvider, refreshTokenRepository);
            var person = TestDataFactory.CreatePerson();
            var user = TestDataFactory.CreateUser("testuser", hasher.HashPassword("password"), person);
            await userRepository.AddAsync(user);

            LoginCommand command = new(
                Username: "testuser",
                Password: "password"
            );

            LoginResponse response = await handler.Handle(command, CancellationToken.None);
            Assert.NotEmpty(response.AccessToken);
            Assert.NotEmpty(response.RefreshToken);
            Assert.NotNull(await refreshTokenRepository.GetByTokenHashAsync(tokenProvider.HashToken(response.RefreshToken)));

            LoginCommand wrongCommand = new("testuser", hasher.HashPassword("wrongpassword"));
            await Assert.ThrowsAsync<ArgumentException>(async () =>
            {
                await handler.Handle(wrongCommand, CancellationToken.None);
            });
        }

        [Fact]
        public async Task TestLoginDuplicateToken()
        {
            FakeUserRepository userRepository = new FakeUserRepository();
            FakePasswordHasher hasher = new FakePasswordHasher();
            FakeTokenProvider tokenProvider = new FakeTokenProvider();
            FakeRefreshTokenRepository refreshTokenRepository = new FakeRefreshTokenRepository();
            LoginCommandHandler handler = new LoginCommandHandler(new UserService(userRepository, hasher), tokenProvider, refreshTokenRepository);
            var person = TestDataFactory.CreatePerson();
            var user = TestDataFactory.CreateUser("testuser", hasher.HashPassword("password"), person);
            await userRepository.AddAsync(user);

            LoginCommand command = new(
                Username: "testuser",
                Password: "password"
            );
            refreshTokenRepository.SetupDuplicateToken(3); // Simulate duplicate token scenario

            LoginResponse response = await handler.Handle(command, CancellationToken.None);
            Assert.NotEmpty(response.AccessToken);
            Assert.NotEmpty(response.RefreshToken);
            Assert.NotNull(await refreshTokenRepository.GetByTokenHashAsync(tokenProvider.HashToken(response.RefreshToken)));
            Assert.Equal(0, refreshTokenRepository.showRemainingDuplicateCount());
        }
    }
}
