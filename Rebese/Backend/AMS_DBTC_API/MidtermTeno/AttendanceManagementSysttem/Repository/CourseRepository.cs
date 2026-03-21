using Microsoft.EntityFrameworkCore;
using MidtermTeno.AttendanceManagementSysttem;
using MidtermTeno.AttendanceManagementSysttem.Interface;
using MidtermTeno.AttendanceManagementSysttem.Model;

namespace MidtermTeno.AttendanceManagementSysttem.Repository
{
    /// <summary>
    /// EF Core data access implementation for courses.
    /// </summary>
    public class CourseRepository : ICourseRepository
    {
        private readonly DatabaseLibrary _db;

        public CourseRepository(DatabaseLibrary db)
        {
            _db = db;
        }

        /// <summary>
        /// Returns all courses.
        /// </summary>
        public async Task<List<Course>> GetAllAsync()
        {
            return await _db.Courses.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Returns one course by ID, or null if not found.
        /// </summary>
        public async Task<Course?> GetByIdAsync(int courseId)
        {
            return await _db.Courses.FindAsync(courseId);
        }

        /// <summary>
        /// Creates a new course row.
        /// </summary>
        public async Task<Course> AddAsync(Course course)
        {
            _db.Courses.Add(course);
            await _db.SaveChangesAsync();
            return course;
        }

        /// <summary>
        /// Updates an existing course row.
        /// </summary>
        public async Task<bool> UpdateAsync(Course course)
        {
            var exists = await _db.Courses.AnyAsync(c => c.CourseId == course.CourseId);
            if (!exists) return false;

            _db.Courses.Update(course);
            await _db.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Deletes one course row by ID.
        /// </summary>
        public async Task<bool> DeleteAsync(int courseId)
        {
            var course = await _db.Courses.FindAsync(courseId);
            if (course is null) return false;

            _db.Courses.Remove(course);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}

