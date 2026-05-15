namespace MidtermTeno.AttendanceManagementSysttem.Model
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public required string DepartmentCode { get; set; }
        public required string DepartmentName { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public ICollection<AcademicProgram> Programs { get; set; } = new List<AcademicProgram>();
        public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
    }
}
