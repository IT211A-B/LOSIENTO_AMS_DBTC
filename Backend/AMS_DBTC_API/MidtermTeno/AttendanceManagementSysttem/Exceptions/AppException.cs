namespace MidtermTeno.AttendanceManagementSysttem.Exceptions
{
    public class AppException : Exception
    {
        public int StatusCode { get; }
        public string ErrorCode { get; }

        public AppException(string message, int statusCode = StatusCodes.Status400BadRequest, string errorCode = "app_error")
            : base(message)
        {
            StatusCode = statusCode;
            ErrorCode = errorCode;
        }
    }
}
