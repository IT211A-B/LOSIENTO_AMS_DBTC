namespace MidtermTeno.AttendanceManagementSysttem.DTOs.Auth
{
    public class AuthResponseDTO
    {
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public string Username { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public int? TeacherId { get; set; }
        public int? StudentId { get; set; }
    }
}
