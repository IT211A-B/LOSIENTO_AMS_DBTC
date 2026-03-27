using Microsoft.EntityFrameworkCore;
using MidtermTeno.AttendanceManagementSysttem;
using MidtermTeno.AttendanceManagementSysttem.Interface;
using MidtermTeno.AttendanceManagementSysttem.Model;

namespace MidtermTeno.AttendanceManagementSysttem.Repository
{
    /// <summary>
    /// EF Core data access implementation for attendance records.
    /// </summary>
    public class AttendanceRecordRepository : IAttendanceRepository
    {
        private readonly DatabaseLibrary _db;

        public AttendanceRecordRepository(DatabaseLibrary db)
        {
            _db = db;
        }

        /// <summary>
        /// Returns all attendance records ordered by latest date first.
        /// </summary>
        public async Task<List<AttendanceRecord>> GetAllAsync()
        {
            return await _db.AttendanceRecords
                .AsNoTracking()
                .OrderByDescending(r => r.AttendanceDate)
                .ToListAsync();
        }

        /// <summary>
        /// Returns one attendance record by ID, or null if not found.
        /// </summary>
        public async Task<AttendanceRecord?> GetByIdAsync(int attendanceRecordId)
        {
            return await _db.AttendanceRecords.FindAsync(attendanceRecordId);
        }

        /// <summary>
        /// Returns one attendance record by student, course, and date.
        /// </summary>
        public async Task<AttendanceRecord?> GetByStudentCourseDateAsync(int studentId, int courseId, DateTime attendanceDate)
        {
            var dateOnly = attendanceDate.Date;
            return await _db.AttendanceRecords.FirstOrDefaultAsync(r =>
                r.StudentId == studentId &&
                r.CourseId == courseId &&
                r.AttendanceDate == dateOnly);
        }

        /// <summary>
        /// Creates a new attendance record row.
        /// </summary>
        public async Task<AttendanceRecord> AddAsync(AttendanceRecord record)
        {
            _db.AttendanceRecords.Add(record);
            await _db.SaveChangesAsync();
            return record;
        }

        /// <summary>
        /// Updates an existing attendance record row.
        /// </summary>
        public async Task<bool> UpdateAsync(AttendanceRecord record)
        {
            var exists = await _db.AttendanceRecords.AnyAsync(r => r.AttendanceRecordId == record.AttendanceRecordId);
            if (!exists) return false;

            _db.AttendanceRecords.Update(record);
            await _db.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Deletes one attendance record row by ID.
        /// </summary>
        public async Task<bool> DeleteAsync(int attendanceRecordId)
        {
            var record = await _db.AttendanceRecords.FindAsync(attendanceRecordId);
            if (record is null) return false;

            _db.AttendanceRecords.Remove(record);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}

