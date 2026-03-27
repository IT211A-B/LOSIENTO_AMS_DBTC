namespace MidtermTeno.AttendanceManagementSysttem.Model
{
    public class Student
    {
        public int StudentId { get; set; }

        public int ProgramId { get; set; } 

        public required string Year_Level { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime LastUpdatedAt { get; set; }

        public string? CreatedBy { get; set; }  

        public string? LastUpdatedBy { get; set; }  

    }
}
