namespace MidtermTeno.AttendanceManagementSysttem.Model
{
    public class UserAccount
    {
        public int UserId { get; set; }
        public required string Username { get; set; }
        public required string PasswordHash { get; set; }
        public UserRole Role { get; set; }
        public bool IsActive { get; set; } = true;

        public int? TeacherId { get; set; }
        public int? StudentId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public Teacher? Teacher { get; set; }
        public Student? Student { get; set; }
    }
}
