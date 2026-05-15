namespace MidtermTeno.AttendanceManagementSysttem.Model
{
    public class Teacher
    {
        public int TeacherId { get; set; }
        public int DepartmentId { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required string Email { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }

        public Department Department { get; set; } = null!;
        public ICollection<Course> Courses { get; set; } = new List<Course>();
        public UserAccount? UserAccount { get; set; }
    }
}
