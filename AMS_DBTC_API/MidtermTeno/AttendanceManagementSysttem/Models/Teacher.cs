namespace MidtermTeno.AttendanceManagementSysttem.Model
{
    public class Teacher
    {
        public int TeacherId { get; set; }

        public required string Department { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        public string? CreatedBy { get; set; }

        public string? LastUpdatedBy { get; set; }
    }
}
