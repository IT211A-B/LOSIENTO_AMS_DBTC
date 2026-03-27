using MidtermTeno.AttendanceManagementSysttem.Model;
using System.Text.Json.Serialization;

namespace MidtermTeno.AttendanceManagementSysttem.DTOs
{
    /// <summary>
    /// Data transfer object for attendance record API requests and responses.
    /// </summary>
    public class AttendanceRecordDTO
    {
        public int AttendanceRecordId { get; set; }

        public int StudentId { get; set; }
        public int CourseId { get; set; }

        public DateTime AttendanceDate { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public AttendanceStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }

        public string? CreatedBy { get; set; }
        public string? LastUpdatedBy { get; set; }
    }
}

