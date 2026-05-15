using FluentAssertions;
using Microsoft.Extensions.Options;
using MidtermTeno.AttendanceManagementSysttem.Auth;
using MidtermTeno.AttendanceManagementSysttem.Configuration;
using MidtermTeno.AttendanceManagementSysttem.DTOs.Auth;
using MidtermTeno.AttendanceManagementSysttem.Interface;
using MidtermTeno.AttendanceManagementSysttem.Model;
using MidtermTeno.AttendanceManagementSysttem.Service;
using Moq;

namespace AMS_DBTC_API.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly Mock<IUserAccountRepository> _userRepo = new();
        private readonly Mock<IPasswordHasher> _passwordHasher = new();
        private readonly AuthService _sut;

        public AuthServiceTests()
        {
            var jwt = Options.Create(new JwtSettings
            {
                SecretKey = "UnitTestSecretKey_Minimum32CharactersLong!",
                Issuer = "Test",
                Audience = "Test",
                ExpirationMinutes = 30
            });
            _sut = new AuthService(_userRepo.Object, _passwordHasher.Object, jwt);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnValidationError_WhenCredentialsEmpty()
        {
            var result = await _sut.LoginAsync(new LoginRequestDTO());
            result.ErrorMessage.Should().Be("Username and password are required.");
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenCredentialsValid()
        {
            var user = new UserAccount
            {
                UserId = 1,
                Username = "admin",
                PasswordHash = "hash",
                Role = UserRole.Admin,
                IsActive = true
            };

            _userRepo.Setup(r => r.GetByUsernameAsync("admin")).ReturnsAsync(user);
            _passwordHasher.Setup(h => h.Verify("pass", "hash")).Returns(true);

            var result = await _sut.LoginAsync(new LoginRequestDTO { Username = "admin", Password = "pass" });

            result.IsSuccess.Should().BeTrue();
            result.Data!.Token.Should().NotBeNullOrWhiteSpace();
            result.Data.Username.Should().Be("admin");
            result.Data.Role.Should().Be("Admin");
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnValidationError_WhenUserNotFound()
        {
            _userRepo.Setup(r => r.GetByUsernameAsync(It.IsAny<string>())).ReturnsAsync((UserAccount?)null);

            var result = await _sut.LoginAsync(new LoginRequestDTO { Username = "ghost", Password = "pass" });

            result.ErrorMessage.Should().Be("Invalid username or password.");
        }

        [Fact]
        public async Task RegisterAsync_ShouldRejectShortPassword()
        {
            var result = await _sut.RegisterAsync(new RegisterRequestDTO
            {
                Username = "newuser",
                Password = "short",
                Role = UserRole.Admin
            });

            result.ErrorMessage.Should().Be("Password must be at least 8 characters.");
        }
    }
}
