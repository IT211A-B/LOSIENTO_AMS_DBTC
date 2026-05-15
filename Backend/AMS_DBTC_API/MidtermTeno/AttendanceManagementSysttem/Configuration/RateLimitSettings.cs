namespace MidtermTeno.AttendanceManagementSysttem.Configuration
{
    public class RateLimitSettings
    {
        public const string SectionName = "RateLimiting";

        public int ApiPermitLimit { get; set; } = 100;
        public int AuthPermitLimit { get; set; } = 10;
        public int WindowSeconds { get; set; } = 60;
    }
}
