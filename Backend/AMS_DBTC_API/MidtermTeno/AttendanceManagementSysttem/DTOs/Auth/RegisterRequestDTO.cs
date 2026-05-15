using MidtermTeno.AttendanceManagementSysttem.Model;

namespace MidtermTeno.AttendanceManagementSysttem.DTOs.Auth
{
    public class RegisterRequestDTO
    {
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public UserRole Role { get; set; }
        public int? TeacherId { get; set; }
        public int? StudentId { get; set; }
    }
}
