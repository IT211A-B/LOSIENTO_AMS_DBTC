namespace MidtermTeno.AttendanceManagementSysttem.DTOs
{
    public class StudentDTO
    {
        public int StudentId { get; set; }
        public int ProgramId { get; set; }
        public string? ProgramName { get; set; }
        public string StudentNumber { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Year_Level { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;

        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
    }
}
