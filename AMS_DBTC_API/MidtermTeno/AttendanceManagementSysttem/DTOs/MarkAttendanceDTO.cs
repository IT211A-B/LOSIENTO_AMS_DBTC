using MidtermTeno.AttendanceManagementSysttem.Model;
using System.Text.Json.Serialization;

namespace MidtermTeno.AttendanceManagementSysttem.DTOs
{
    /// <summary>
    /// Payload used by the mark-attendance endpoint.
    /// </summary>
    public class MarkAttendanceDTO
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }

        public DateTime AttendanceDate { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AttendanceStatus Status { get; set; }

        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
    }
}

