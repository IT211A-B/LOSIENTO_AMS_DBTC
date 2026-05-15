namespace MidtermTeno.AttendanceManagementSysttem.Model
{
    public class Student
    {
        public int StudentId { get; set; }
        public int ProgramId { get; set; }
        public required string StudentNumber { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Year_Level { get; set; }
        public required string Email { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }

        public AcademicProgram Program { get; set; } = null!;
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
        public ICollection<AttendanceRecord> AttendanceRecords { get; set; } = new List<AttendanceRecord>();
        public UserAccount? UserAccount { get; set; }
    }
}
