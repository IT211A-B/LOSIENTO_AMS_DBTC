using Microsoft.EntityFrameworkCore;
using MidtermTeno.AttendanceManagementSysttem;
using MidtermTeno.AttendanceManagementSysttem.Interface;
using MidtermTeno.AttendanceManagementSysttem.Model;

namespace MidtermTeno.AttendanceManagementSysttem.Repository
{
    /// <summary>
    /// EF Core data access implementation for teachers.
    /// </summary>
    public class TeacherRepository : ITeacherRepository
    {
        private readonly DatabaseLibrary _db;

        public TeacherRepository(DatabaseLibrary db)
        {
            _db = db;
        }

        /// <summary>
        /// Returns all teachers.
        /// </summary>
        public async Task<List<Teacher>> GetAllAsync()
        {
            return await _db.Teachers.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Returns one teacher by ID, or null if not found.
        /// </summary>
        public async Task<Teacher?> GetByIdAsync(int teacherId)
        {
            return await _db.Teachers.FindAsync(teacherId);
        }

        /// <summary>
        /// Creates a new teacher row.
        /// </summary>
        public async Task<Teacher> AddAsync(Teacher teacher)
        {
            _db.Teachers.Add(teacher);
            await _db.SaveChangesAsync();
            return teacher;
        }

        /// <summary>
        /// Updates an existing teacher row.
        /// </summary>
        public async Task<bool> UpdateAsync(Teacher teacher)
        {
            var exists = await _db.Teachers.AnyAsync(t => t.TeacherId == teacher.TeacherId);
            if (!exists) return false;

            _db.Teachers.Update(teacher);
            await _db.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Deletes one teacher row by ID.
        /// </summary>
        public async Task<bool> DeleteAsync(int teacherId)
        {
            var teacher = await _db.Teachers.FindAsync(teacherId);
            if (teacher is null) return false;

            _db.Teachers.Remove(teacher);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}

