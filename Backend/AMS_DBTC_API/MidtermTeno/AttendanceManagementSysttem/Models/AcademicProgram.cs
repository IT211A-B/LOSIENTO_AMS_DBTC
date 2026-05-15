namespace MidtermTeno.AttendanceManagementSysttem.Model
{
    public class AcademicProgram
    {
        public int ProgramId { get; set; }
        public int DepartmentId { get; set; }
        public required string ProgramCode { get; set; }
        public required string ProgramName { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public Department Department { get; set; } = null!;
        public ICollection<Student> Students { get; set; } = new List<Student>();
    }
}
