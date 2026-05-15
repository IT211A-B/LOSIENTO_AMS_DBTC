using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MidtermTeno.AttendanceManagementSysttem.Auth;
using MidtermTeno.AttendanceManagementSysttem.Configuration;
using MidtermTeno.AttendanceManagementSysttem.Constants;
using MidtermTeno.AttendanceManagementSysttem.DTOs;
using MidtermTeno.AttendanceManagementSysttem.DTOs.Auth;
using MidtermTeno.AttendanceManagementSysttem.Interface;
using MidtermTeno.AttendanceManagementSysttem.Interface.ServiceInterface;
using MidtermTeno.AttendanceManagementSysttem.Model;

namespace MidtermTeno.AttendanceManagementSysttem.Service
{
    public class AuthService : IAuthService
    {
        private readonly IUserAccountRepository _userRepo;
        private readonly IPasswordHasher _passwordHasher;
        private readonly JwtSettings _jwtSettings;

        public AuthService(
            IUserAccountRepository userRepo,
            IPasswordHasher passwordHasher,
            IOptions<JwtSettings> jwtSettings)
        {
            _userRepo = userRepo;
            _passwordHasher = passwordHasher;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<ServiceResult<AuthResponseDTO>> LoginAsync(LoginRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return ServiceResult<AuthResponseDTO>.ValidationError("Username and password are required.");

            var user = await _userRepo.GetByUsernameAsync(request.Username.Trim());
            if (user is null || !user.IsActive || !_passwordHasher.Verify(request.Password, user.PasswordHash))
                return ServiceResult<AuthResponseDTO>.ValidationError("Invalid username or password.");

            return ServiceResult<AuthResponseDTO>.Ok(BuildAuthResponse(user));
        }

        public async Task<ServiceResult<AuthResponseDTO>> RegisterAsync(RegisterRequestDTO request)
        {
            if (string.IsNullOrWhiteSpace(request.Username) || string.IsNullOrWhiteSpace(request.Password))
                return ServiceResult<AuthResponseDTO>.ValidationError("Username and password are required.");

            if (request.Password.Length < 8)
                return ServiceResult<AuthResponseDTO>.ValidationError("Password must be at least 8 characters.");

            if (await _userRepo.UsernameExistsAsync(request.Username.Trim()))
                return ServiceResult<AuthResponseDTO>.ValidationError("Username already exists.");

            if (request.Role == UserRole.Teacher && !request.TeacherId.HasValue)
                return ServiceResult<AuthResponseDTO>.ValidationError("TeacherId is required for Teacher accounts.");

            if (request.Role == UserRole.Student && !request.StudentId.HasValue)
                return ServiceResult<AuthResponseDTO>.ValidationError("StudentId is required for Student accounts.");

            if (request.Role == UserRole.Admin && (request.TeacherId.HasValue || request.StudentId.HasValue))
                return ServiceResult<AuthResponseDTO>.ValidationError("Admin accounts cannot be linked to Teacher or Student records.");

            var now = DateTime.UtcNow;
            var user = new UserAccount
            {
                Username = request.Username.Trim(),
                PasswordHash = _passwordHasher.Hash(request.Password),
                Role = request.Role,
                TeacherId = request.TeacherId,
                StudentId = request.StudentId,
                IsActive = true,
                CreatedAt = now,
                LastUpdatedAt = now
            };

            var created = await _userRepo.AddAsync(user);
            return ServiceResult<AuthResponseDTO>.Ok(BuildAuthResponse(created));
        }

        private AuthResponseDTO BuildAuthResponse(UserAccount user)
        {
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);
            var roleName = user.Role.ToString();

            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Role, roleName)
            };

            if (user.TeacherId.HasValue)
                claims.Add(new Claim("teacherId", user.TeacherId.Value.ToString()));

            if (user.StudentId.HasValue)
                claims.Add(new Claim("studentId", user.StudentId.Value.ToString()));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: expiresAt,
                signingCredentials: credentials);

            return new AuthResponseDTO
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                ExpiresAt = expiresAt,
                Username = user.Username,
                Role = roleName,
                TeacherId = user.TeacherId,
                StudentId = user.StudentId
            };
        }
    }
}
