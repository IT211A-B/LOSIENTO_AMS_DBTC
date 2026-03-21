namespace MidtermTeno.AttendanceManagementSysttem.DTOs
{
    /// <summary>
    /// Data transfer object for student API requests and responses.
    /// </summary>
    public class StudentDTO
    {
        public int StudentId { get; set; }
        public int ProgramId { get; set; }
        public string Year_Level { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
    }
}
