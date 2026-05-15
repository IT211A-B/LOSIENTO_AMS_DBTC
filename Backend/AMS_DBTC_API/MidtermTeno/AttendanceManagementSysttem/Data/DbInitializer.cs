using Microsoft.EntityFrameworkCore;
using MidtermTeno.AttendanceManagementSysttem.Auth;
using MidtermTeno.AttendanceManagementSysttem.Model;

namespace MidtermTeno.AttendanceManagementSysttem.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(DatabaseLibrary db, IPasswordHasher passwordHasher, IConfiguration configuration)
        {
            await db.Database.MigrateAsync();

            if (await db.Departments.AnyAsync()) return;

            var now = DateTime.UtcNow;
            var department = new Department
            {
                DepartmentCode = "CIT",
                DepartmentName = "College of Information Technology",
                CreatedAt = now,
                LastUpdatedAt = now
            };
            db.Departments.Add(department);
            await db.SaveChangesAsync();

            var program = new AcademicProgram
            {
                DepartmentId = department.DepartmentId,
                ProgramCode = "BSIT",
                ProgramName = "Bachelor of Science in Information Technology",
                CreatedAt = now,
                LastUpdatedAt = now
            };
            db.Programs.Add(program);
            await db.SaveChangesAsync();

            var adminPassword = configuration["Seed:AdminPassword"] ?? "Admin@12345";
            db.UserAccounts.Add(new UserAccount
            {
                Username = configuration["Seed:AdminUsername"] ?? "admin",
                PasswordHash = passwordHasher.Hash(adminPassword),
                Role = UserRole.Admin,
                IsActive = true,
                CreatedAt = now,
                LastUpdatedAt = now
            });
            await db.SaveChangesAsync();
        }
    }
}
