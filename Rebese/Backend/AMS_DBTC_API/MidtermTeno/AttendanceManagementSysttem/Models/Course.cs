namespace MidtermTeno.AttendanceManagementSysttem.Model
{
    public class Course
    {
        public int CourseId { get; set; }   

        public required string CourseName { get; set; }

        public required string CourseCode { get; set; }

        public string? Description { get; set; }

        public required int TeacherId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        public string? CreatedBy { get; set; } = null;

        public string? LastUpdatedBy { get; set; }


    }
}
