using MidtermTeno.AttendanceManagementSysttem.DTOs;
using MidtermTeno.AttendanceManagementSysttem.DTOs.Auth;

namespace MidtermTeno.AttendanceManagementSysttem.Interface.ServiceInterface
{
    public interface IAuthService
    {
        Task<ServiceResult<AuthResponseDTO>> LoginAsync(LoginRequestDTO request);
        Task<ServiceResult<AuthResponseDTO>> RegisterAsync(RegisterRequestDTO request);
    }
}
