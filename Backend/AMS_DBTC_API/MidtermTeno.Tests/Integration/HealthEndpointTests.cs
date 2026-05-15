using System.Net;
using FluentAssertions;

namespace AMS_DBTC_API.Tests.Integration
{
    public class HealthEndpointTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly HttpClient _client;

        public HealthEndpointTests(CustomWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task Health_ShouldReturnOk_OrServiceUnavailable_WithoutPostgres()
        {
            var response = await _client.GetAsync("/health");

            response.StatusCode.Should().BeOneOf(HttpStatusCode.OK, HttpStatusCode.ServiceUnavailable);
        }
    }
}
