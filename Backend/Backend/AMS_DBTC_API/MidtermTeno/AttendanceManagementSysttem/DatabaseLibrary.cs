using Microsoft.EntityFrameworkCore;
using MidtermTeno.AttendanceManagementSysttem.Model;

namespace MidtermTeno.AttendanceManagementSysttem
{
    public class DatabaseLibrary : DbContext
    {
        public DatabaseLibrary(DbContextOptions<DatabaseLibrary> options) : base(options)
        {
        }
        public DbSet<Course> Courses { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<Teacher> Teachers { get; set; }

        public DbSet<AttendanceRecord> AttendanceRecords { get; set; }


    }
}
