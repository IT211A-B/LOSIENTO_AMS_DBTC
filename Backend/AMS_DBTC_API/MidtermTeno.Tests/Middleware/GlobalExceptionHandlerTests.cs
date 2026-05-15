using System.Text.Json;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging.Abstractions;
using MidtermTeno.AttendanceManagementSysttem.Exceptions;
using MidtermTeno.AttendanceManagementSysttem.Middleware;
using Moq;

namespace AMS_DBTC_API.Tests.Middleware
{
    public class GlobalExceptionHandlerTests
    {
        [Fact]
        public async Task TryHandleAsync_ShouldReturnProblemDetails_ForNotFoundException()
        {
            var env = new Mock<IHostEnvironment>();
            env.Setup(e => e.EnvironmentName).Returns(Environments.Development);

            var handler = new GlobalExceptionHandler(NullLogger<GlobalExceptionHandler>.Instance, env.Object);
            var context = new DefaultHttpContext();
            context.Response.Body = new MemoryStream();
            context.Request.Path = "/api/test";

            var handled = await handler.TryHandleAsync(
                context,
                new NotFoundException("Item not found"),
                CancellationToken.None);

            handled.Should().BeTrue();
            context.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
            context.Response.ContentType.Should().StartWith("application/json");

            context.Response.Body.Position = 0;
            var problem = await JsonSerializer.DeserializeAsync<ProblemDetails>(
                context.Response.Body,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            problem!.Status.Should().Be(404);
            problem.Title.Should().Be("Not Found");
        }
    }
}
