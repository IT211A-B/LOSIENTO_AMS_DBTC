namespace MidtermTeno.AttendanceManagementSysttem.DTOs
{
    public class TeacherDTO
    {
        public int TeacherId { get; set; }
        public int DepartmentId { get; set; }
        public string? DepartmentName { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
    }
}
