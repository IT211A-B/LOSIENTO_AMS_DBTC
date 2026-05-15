namespace MidtermTeno.AttendanceManagementSysttem.DTOs
{
    public class AcademicProgramDTO
    {
        public int ProgramId { get; set; }
        public int DepartmentId { get; set; }
        public string ProgramCode { get; set; } = string.Empty;
        public string ProgramName { get; set; } = string.Empty;
        public string? DepartmentName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
