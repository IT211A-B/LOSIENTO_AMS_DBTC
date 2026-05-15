namespace MidtermTeno.AttendanceManagementSysttem.DTOs
{
    public class ServiceResult<T>
    {
        public T? Data { get; init; }
        public string? ErrorMessage { get; init; }
        public bool NotFound { get; init; }

        public bool IsSuccess => ErrorMessage is null && !NotFound;

        public static ServiceResult<T> Ok(T data) => new() { Data = data };

        public static ServiceResult<T> ValidationError(string message) => new() { ErrorMessage = message };

        public static ServiceResult<T> NotFoundResult() => new() { NotFound = true };
    }
}
