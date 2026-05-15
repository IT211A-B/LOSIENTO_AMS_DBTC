using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MidtermTeno.AttendanceManagementSysttem.Exceptions;

namespace MidtermTeno.AttendanceManagementSysttem.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;
        private readonly IHostEnvironment _environment;

        public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger, IHostEnvironment environment)
        {
            _logger = logger;
            _environment = environment;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            var (statusCode, errorCode, title) = MapException(exception);

            if (statusCode >= StatusCodes.Status500InternalServerError)
                _logger.LogError(exception, "Unhandled exception at {Path}", httpContext.Request.Path);
            else
                _logger.LogWarning(exception, "Handled exception at {Path}", httpContext.Request.Path);

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = _environment.IsDevelopment() ? exception.Message : GetSafeDetail(exception, statusCode),
                Instance = httpContext.Request.Path,
                Type = $"https://httpstatuses.com/{statusCode}"
            };
            problem.Extensions["errorCode"] = errorCode;
            problem.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;

            httpContext.Response.StatusCode = statusCode;
            httpContext.Response.ContentType = "application/problem+json";
            await httpContext.Response.WriteAsJsonAsync(problem, cancellationToken);
            return true;
        }

        private static (int StatusCode, string ErrorCode, string Title) MapException(Exception exception) =>
            exception switch
            {
                AppException app => (app.StatusCode, app.ErrorCode, GetTitle(app.StatusCode)),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "unauthorized", "Unauthorized"),
                KeyNotFoundException => (StatusCodes.Status404NotFound, "not_found", "Not Found"),
                ArgumentException => (StatusCodes.Status400BadRequest, "bad_request", "Bad Request"),
                InvalidOperationException => (StatusCodes.Status409Conflict, "conflict", "Conflict"),
                _ => (StatusCodes.Status500InternalServerError, "internal_error", "Internal Server Error")
            };

        private static string GetTitle(int statusCode) => statusCode switch
        {
            StatusCodes.Status400BadRequest => "Bad Request",
            StatusCodes.Status401Unauthorized => "Unauthorized",
            StatusCodes.Status404NotFound => "Not Found",
            StatusCodes.Status409Conflict => "Conflict",
            StatusCodes.Status429TooManyRequests => "Too Many Requests",
            _ => "Internal Server Error"
        };

        private static string GetSafeDetail(Exception exception, int statusCode) =>
            statusCode >= StatusCodes.Status500InternalServerError
                ? "An unexpected error occurred. Please try again later."
                : exception.Message;
    }
}
