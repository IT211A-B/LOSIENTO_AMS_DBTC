namespace MidtermTeno.AttendanceManagementSysttem.Configuration
{
    public class JwtSettings
    {
        public const string SectionName = "Jwt";

        public string Issuer { get; set; } = "AMS_DBTC_API";
        public string Audience { get; set; } = "AMS_DBTC_Client";
        public string SecretKey { get; set; } = string.Empty;
        public int ExpirationMinutes { get; set; } = 60;
    }
}
