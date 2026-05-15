using Microsoft.EntityFrameworkCore;
using MidtermTeno.AttendanceManagementSysttem.Interface;
using MidtermTeno.AttendanceManagementSysttem.Model;

namespace MidtermTeno.AttendanceManagementSysttem.Repository
{
    public class EnrollmentRepository : IEnrollmentRepository
    {
        private readonly DatabaseLibrary _db;

        public EnrollmentRepository(DatabaseLibrary db) => _db = db;

        public async Task<List<Enrollment>> GetAllAsync() =>
            await _db.Enrollments.AsNoTracking().OrderByDescending(e => e.EnrolledAt).ToListAsync();

        public async Task<Enrollment?> GetByIdAsync(int enrollmentId) =>
            await _db.Enrollments.FindAsync(enrollmentId);

        public async Task<bool> IsActiveEnrollmentAsync(int studentId, int courseId) =>
            await _db.Enrollments.AnyAsync(e =>
                e.StudentId == studentId &&
                e.CourseId == courseId &&
                e.Status == EnrollmentStatus.Active);

        public async Task<Enrollment> AddAsync(Enrollment enrollment)
        {
            _db.Enrollments.Add(enrollment);
            await _db.SaveChangesAsync();
            return enrollment;
        }

        public async Task<bool> UpdateAsync(Enrollment enrollment)
        {
            var exists = await _db.Enrollments.AnyAsync(e => e.EnrollmentId == enrollment.EnrollmentId);
            if (!exists) return false;
            _db.Enrollments.Update(enrollment);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int enrollmentId)
        {
            var enrollment = await _db.Enrollments.FindAsync(enrollmentId);
            if (enrollment is null) return false;
            _db.Enrollments.Remove(enrollment);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
