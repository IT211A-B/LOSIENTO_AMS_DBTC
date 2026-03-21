namespace MidtermTeno.AttendanceManagementSysttem.DTOs
{
    /// <summary>
    /// Data transfer object for teacher API requests and responses.
    /// </summary>
    public class TeacherDTO
    {
        public int TeacherId { get; set; }
        public string Department { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
    }
}
