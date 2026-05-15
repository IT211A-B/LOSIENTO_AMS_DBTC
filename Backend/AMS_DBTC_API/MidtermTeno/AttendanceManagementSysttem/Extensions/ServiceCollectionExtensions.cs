using System.Text;
using AMS_DBTC_API.AttendanceManagementSysttem.Interface;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using MidtermTeno.AttendanceManagementSysttem.Auth;
using MidtermTeno.AttendanceManagementSysttem.Configuration;
using MidtermTeno.AttendanceManagementSysttem.Interface;
using MidtermTeno.AttendanceManagementSysttem.Interface.ServiceInterface;
using MidtermTeno.AttendanceManagementSysttem.Middleware;
using MidtermTeno.AttendanceManagementSysttem.Repository;
using MidtermTeno.AttendanceManagementSysttem.Service;

namespace MidtermTeno.AttendanceManagementSysttem.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
            services.Configure<RateLimitSettings>(configuration.GetSection(RateLimitSettings.SectionName));

            var jwtSettings = configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()
                ?? throw new InvalidOperationException("JWT settings are not configured.");

            var secretKey = jwtSettings.SecretKey;
            if (string.IsNullOrWhiteSpace(secretKey))
                secretKey = configuration["JWT_SECRET_KEY"];

            if (string.IsNullOrWhiteSpace(secretKey) || secretKey.Length < 32)
                throw new InvalidOperationException("Jwt:SecretKey (or JWT_SECRET_KEY env var) must be at least 32 characters.");

            services.Configure<JwtSettings>(options =>
            {
                options.Issuer = jwtSettings.Issuer;
                options.Audience = jwtSettings.Audience;
                options.SecretKey = secretKey;
                options.ExpirationMinutes = jwtSettings.ExpirationMinutes;
            });

            var rateLimitSettings = configuration.GetSection(RateLimitSettings.SectionName).Get<RateLimitSettings>()
                ?? new RateLimitSettings();

            services.AddProblemDetails();
            services.AddExceptionHandler<GlobalExceptionHandler>();

            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(options =>
            {
                var xmlFilename = $"{System.Reflection.Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFilename);
                if (File.Exists(xmlPath))
                    options.IncludeXmlComments(xmlPath);

                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header,
                    Description = "Enter: Bearer {your JWT token}"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        Array.Empty<string>()
                    }
                });
            });

            var connectionString = configuration.GetConnectionString("AttendanceDBString")
                ?? throw new InvalidOperationException("Connection string 'AttendanceDBString' is not configured.");

            services.AddDbContext<DatabaseLibrary>(options => options.UseNpgsql(connectionString));
            services.AddHealthChecks().AddNpgSql(connectionString);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                        ClockSkew = TimeSpan.FromMinutes(1)
                    };
                });

            services.AddAuthorization();
            services.AddRateLimiter(options =>
            {
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
                options.AddFixedWindowLimiter("api", limiter =>
                {
                    limiter.Window = TimeSpan.FromSeconds(rateLimitSettings.WindowSeconds);
                    limiter.PermitLimit = rateLimitSettings.ApiPermitLimit;
                    limiter.QueueLimit = 0;
                });
                options.AddFixedWindowLimiter("auth", limiter =>
                {
                    limiter.Window = TimeSpan.FromSeconds(rateLimitSettings.WindowSeconds);
                    limiter.PermitLimit = rateLimitSettings.AuthPermitLimit;
                    limiter.QueueLimit = 0;
                });
            });

            services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
            services.AddScoped<IUserAccountRepository, UserAccountRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IAcademicProgramRepository, AcademicProgramRepository>();
            services.AddScoped<IEnrollmentRepository, EnrollmentRepository>();
            services.AddScoped<ITeacherRepository, TeacherRepository>();
            services.AddScoped<IStudentRepository, StudentRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<IAttendanceRepository, AttendanceRecordRepository>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IAcademicProgramService, AcademicProgramService>();
            services.AddScoped<IEnrollmentService, EnrollmentService>();
            services.AddScoped<ITeacherService, TeacherService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IAttendanceService, AttendanceService>();

            return services;
        }
    }
}
