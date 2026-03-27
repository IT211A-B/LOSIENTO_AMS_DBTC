namespace MidtermTeno.AttendanceManagementSysttem.Model
{
    public enum AttendanceStatus
    {
        Present = 1,
        Absent = 2,
        Late = 3
    }

    public class AttendanceRecord
    {
        public int AttendanceRecordId { get; set; }

        public int StudentId { get; set; }
        public int CourseId { get; set; }

        // Store as a date (controller will normalize to midnight with Date).
        public DateTime AttendanceDate { get; set; }

        public AttendanceStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
    }
}

