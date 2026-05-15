namespace MidtermTeno.AttendanceManagementSysttem.DTOs
{
    public class MarkAttendanceResultDTO
    {
        public AttendanceRecordDTO Record { get; set; } = null!;
        public bool WasCreated { get; set; }
    }
}
