using MidtermTeno.AttendanceManagementSysttem.Model;
using System.Text.Json.Serialization;

namespace MidtermTeno.AttendanceManagementSysttem.DTOs
{
    public class EnrollmentDTO
    {
        public int EnrollmentId { get; set; }
        public int StudentId { get; set; }
        public int CourseId { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public EnrollmentStatus Status { get; set; }

        public DateTime EnrolledAt { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime LastUpdatedAt { get; set; }
    }
}
