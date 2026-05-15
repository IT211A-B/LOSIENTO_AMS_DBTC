using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MidtermTeno.AttendanceManagementSysttem;

namespace AMS_DBTC_API.Tests.Integration
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Development");
            builder.UseSetting("Database:SkipInitialization", "true");
            builder.UseSetting("Jwt:SecretKey", "IntegrationTestSecretKey_AtLeast32Chars!");
            builder.UseSetting("ConnectionStrings:AttendanceDBString", "Host=localhost;Database=test");

            builder.ConfigureServices(services =>
            {
                var descriptors = services
                    .Where(d =>
                        d.ServiceType == typeof(DbContextOptions<DatabaseLibrary>) ||
                        d.ServiceType == typeof(DbContextOptions) ||
                        d.ServiceType == typeof(DatabaseLibrary))
                    .ToList();

                foreach (var descriptor in descriptors)
                    services.Remove(descriptor);

                services.AddDbContext<DatabaseLibrary>(options =>
                    options.UseInMemoryDatabase("IntegrationTestDb"));
            });
        }
    }
}
