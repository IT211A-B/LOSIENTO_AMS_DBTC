using MidtermTeno.AttendanceManagementSysttem.Auth;

namespace MidtermTeno.AttendanceManagementSysttem.Data
{
    public static class DatabaseBootstrap
    {
        public static async Task InitializeAsync(
            WebApplication app,
            IConfiguration configuration,
            ILogger logger)
        {
            if (configuration.GetValue<bool>("Database:SkipInitialization"))
            {
                logger.LogInformation("Database initialization skipped (Database:SkipInitialization=true).");
                return;
            }

            try
            {
                using var scope = app.Services.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<DatabaseLibrary>();
                var hasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();
                await DbInitializer.InitializeAsync(db, hasher, configuration);
                logger.LogInformation("Database initialized successfully.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Database initialization failed. Ensure PostgreSQL is running and the connection string is correct.");

                if (app.Environment.IsProduction())
                    throw;

                logger.LogWarning("Application will continue without a database connection. API data endpoints will fail until PostgreSQL is available.");
            }
        }
    }
}
