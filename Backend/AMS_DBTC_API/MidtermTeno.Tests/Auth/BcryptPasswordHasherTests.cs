using FluentAssertions;
using MidtermTeno.AttendanceManagementSysttem.Auth;

namespace AMS_DBTC_API.Tests.Auth
{
    public class BcryptPasswordHasherTests
    {
        private readonly BcryptPasswordHasher _hasher = new();

        [Fact]
        public void Hash_ShouldProduceDifferentValueThanPlainText()
        {
            var hash = _hasher.Hash("TestPassword123!");
            hash.Should().NotBe("TestPassword123!");
            hash.Should().StartWith("$2");
        }

        [Fact]
        public void Verify_ShouldReturnTrue_ForCorrectPassword()
        {
            const string password = "SecurePass99!";
            var hash = _hasher.Hash(password);
            _hasher.Verify(password, hash).Should().BeTrue();
        }

        [Fact]
        public void Verify_ShouldReturnFalse_ForWrongPassword()
        {
            var hash = _hasher.Hash("CorrectPassword");
            _hasher.Verify("WrongPassword", hash).Should().BeFalse();
        }
    }
}
