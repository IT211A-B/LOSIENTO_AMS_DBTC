using System.Security.Claims;

namespace MidtermTeno.AttendanceManagementSysttem.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static int? GetTeacherId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst("teacherId")?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }

        public static int? GetStudentId(this ClaimsPrincipal user)
        {
            var claim = user.FindFirst("studentId")?.Value;
            return int.TryParse(claim, out var id) ? id : null;
        }

        public static string? GetUsername(this ClaimsPrincipal user) =>
            user.FindFirst(ClaimTypes.Name)?.Value;
    }
}
