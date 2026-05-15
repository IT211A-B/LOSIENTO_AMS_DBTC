using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using MidtermTeno.AttendanceManagementSysttem.Constants;
using MidtermTeno.AttendanceManagementSysttem.DTOs.Auth;
using MidtermTeno.AttendanceManagementSysttem.Interface.ServiceInterface;
using Swashbuckle.AspNetCore.Annotations;

namespace MidtermTeno.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService) => _authService = authService;

        [HttpPost("login")]
        [AllowAnonymous]
        [EnableRateLimiting("auth")]
        [SwaggerOperation(Summary = "Login", Description = "Authenticates a user and returns a JWT bearer token.")]
        [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDTO>> Login(LoginRequestDTO request)
        {
            var result = await _authService.LoginAsync(request);
            if (result.ErrorMessage is not null) return BadRequest(result.ErrorMessage);
            return Ok(result.Data);
        }

        [HttpPost("register")]
        [Authorize(Roles = AppRoles.Admin)]
        [EnableRateLimiting("auth")]
        [SwaggerOperation(Summary = "Register user", Description = "Creates a new user account with a BCrypt-hashed password. Admin only.")]
        [ProducesResponseType(typeof(AuthResponseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AuthResponseDTO>> Register(RegisterRequestDTO request)
        {
            var result = await _authService.RegisterAsync(request);
            if (result.ErrorMessage is not null) return BadRequest(result.ErrorMessage);
            return Created(string.Empty, result.Data);
        }
    }
}
