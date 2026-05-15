using Microsoft.EntityFrameworkCore;
using MidtermTeno.AttendanceManagementSysttem.Model;

namespace MidtermTeno.AttendanceManagementSysttem
{
    public class DatabaseLibrary : DbContext
    {
        public DatabaseLibrary(DbContextOptions<DatabaseLibrary> options) : base(options)
        {
        }

        public DbSet<Department> Departments => Set<Department>();
        public DbSet<AcademicProgram> Programs => Set<AcademicProgram>();
        public DbSet<Teacher> Teachers => Set<Teacher>();
        public DbSet<Student> Students => Set<Student>();
        public DbSet<Course> Courses => Set<Course>();
        public DbSet<Enrollment> Enrollments => Set<Enrollment>();
        public DbSet<AttendanceRecord> AttendanceRecords => Set<AttendanceRecord>();
        public DbSet<UserAccount> UserAccounts => Set<UserAccount>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Department>(e =>
            {
                e.HasKey(d => d.DepartmentId);
                e.HasIndex(d => d.DepartmentCode).IsUnique();
                e.Property(d => d.DepartmentCode).HasMaxLength(20);
                e.Property(d => d.DepartmentName).HasMaxLength(150);
            });

            modelBuilder.Entity<AcademicProgram>(e =>
            {
                e.HasKey(p => p.ProgramId);
                e.HasIndex(p => p.ProgramCode).IsUnique();
                e.Property(p => p.ProgramCode).HasMaxLength(20);
                e.Property(p => p.ProgramName).HasMaxLength(150);
                e.HasOne(p => p.Department)
                    .WithMany(d => d.Programs)
                    .HasForeignKey(p => p.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Teacher>(e =>
            {
                e.HasKey(t => t.TeacherId);
                e.HasIndex(t => t.Email).IsUnique();
                e.Property(t => t.FirstName).HasMaxLength(100);
                e.Property(t => t.LastName).HasMaxLength(100);
                e.Property(t => t.Email).HasMaxLength(200);
                e.HasOne(t => t.Department)
                    .WithMany(d => d.Teachers)
                    .HasForeignKey(t => t.DepartmentId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Student>(e =>
            {
                e.HasKey(s => s.StudentId);
                e.HasIndex(s => s.StudentNumber).IsUnique();
                e.HasIndex(s => s.Email).IsUnique();
                e.Property(s => s.StudentNumber).HasMaxLength(30);
                e.Property(s => s.FirstName).HasMaxLength(100);
                e.Property(s => s.LastName).HasMaxLength(100);
                e.Property(s => s.Email).HasMaxLength(200);
                e.Property(s => s.Year_Level).HasMaxLength(20);
                e.HasOne(s => s.Program)
                    .WithMany(p => p.Students)
                    .HasForeignKey(s => s.ProgramId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Course>(e =>
            {
                e.HasKey(c => c.CourseId);
                e.HasIndex(c => c.CourseCode).IsUnique();
                e.Property(c => c.CourseCode).HasMaxLength(20);
                e.Property(c => c.CourseName).HasMaxLength(150);
                e.HasOne(c => c.Teacher)
                    .WithMany(t => t.Courses)
                    .HasForeignKey(c => c.TeacherId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<Enrollment>(e =>
            {
                e.HasKey(en => en.EnrollmentId);
                e.HasIndex(en => new { en.StudentId, en.CourseId }).IsUnique();
                e.HasOne(en => en.Student)
                    .WithMany(s => s.Enrollments)
                    .HasForeignKey(en => en.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(en => en.Course)
                    .WithMany(c => c.Enrollments)
                    .HasForeignKey(en => en.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<AttendanceRecord>(e =>
            {
                e.HasKey(a => a.AttendanceRecordId);
                e.HasIndex(a => new { a.StudentId, a.CourseId, a.AttendanceDate }).IsUnique();
                e.HasOne(a => a.Student)
                    .WithMany(s => s.AttendanceRecords)
                    .HasForeignKey(a => a.StudentId)
                    .OnDelete(DeleteBehavior.Cascade);
                e.HasOne(a => a.Course)
                    .WithMany(c => c.AttendanceRecords)
                    .HasForeignKey(a => a.CourseId)
                    .OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<UserAccount>(e =>
            {
                e.HasKey(u => u.UserId);
                e.HasIndex(u => u.Username).IsUnique();
                e.Property(u => u.Username).HasMaxLength(100);
                e.Property(u => u.PasswordHash).HasMaxLength(500);
                e.HasOne(u => u.Teacher)
                    .WithOne(t => t.UserAccount)
                    .HasForeignKey<UserAccount>(u => u.TeacherId)
                    .OnDelete(DeleteBehavior.SetNull);
                e.HasOne(u => u.Student)
                    .WithOne(s => s.UserAccount)
                    .HasForeignKey<UserAccount>(u => u.StudentId)
                    .OnDelete(DeleteBehavior.SetNull);
            });
        }
    }
}
