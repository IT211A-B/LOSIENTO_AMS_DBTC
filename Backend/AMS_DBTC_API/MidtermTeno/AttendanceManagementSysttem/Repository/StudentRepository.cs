using Microsoft.EntityFrameworkCore;
using MidtermTeno.AttendanceManagementSysttem;
using MidtermTeno.AttendanceManagementSysttem.Interface;
using MidtermTeno.AttendanceManagementSysttem.Model;

namespace MidtermTeno.AttendanceManagementSysttem.Repository
{
    /// <summary>
    /// EF Core data access implementation for students.
    /// </summary>
    public class StudentRepository : IStudentRepository
    {
        private readonly DatabaseLibrary _db;

        public StudentRepository(DatabaseLibrary db)
        {
            _db = db;
        }

        /// <summary>
        /// Returns all students.
        /// </summary>
        public async Task<List<Student>> GetAllAsync()
        {
            return await _db.Students.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// Returns one student by ID, or null if not found.
        /// </summary>
        public async Task<Student?> GetByIdAsync(int studentId)
        {
            return await _db.Students.FindAsync(studentId);
        }

        /// <summary>
        /// Creates a new student row.
        /// </summary>
        public async Task<Student> AddAsync(Student student)
        {
            _db.Students.Add(student);
            await _db.SaveChangesAsync();
            return student;
        }

        /// <summary>
        /// Updates an existing student row.
        /// </summary>
        public async Task<bool> UpdateAsync(Student student)
        {
            var exists = await _db.Students.AnyAsync(s => s.StudentId == student.StudentId);
            if (!exists) return false;

            _db.Students.Update(student);
            await _db.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Deletes one student row by ID.
        /// </summary>
        public async Task<bool> DeleteAsync(int studentId)
        {
            var student = await _db.Students.FindAsync(studentId);
            if (student is null) return false;

            _db.Students.Remove(student);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}

