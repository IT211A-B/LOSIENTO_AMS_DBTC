namespace MidtermTeno.AttendanceManagementSysttem.DTOs
{
    /// <summary>
    /// Data transfer object for course API requests and responses.
    /// </summary>
    public class CourseDTO
    {
        public int CourseId { get; set; }
        public string CourseName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int TeacherId { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
    }
}
