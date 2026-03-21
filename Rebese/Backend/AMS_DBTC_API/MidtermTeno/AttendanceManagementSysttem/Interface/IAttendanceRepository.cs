using MidtermTeno.AttendanceManagementSysttem.Model;

namespace MidtermTeno.AttendanceManagementSysttem.Interface
{
    public interface IAttendanceRepository
    {
        Task<List<AttendanceRecord>> GetAllAsync();
        Task<AttendanceRecord?> GetByIdAsync(int attendanceRecordId);

        // Returns the record for one student + one course on one date.
        Task<AttendanceRecord?> GetByStudentCourseDateAsync(int studentId, int courseId, DateTime attendanceDate);

        Task<AttendanceRecord> AddAsync(AttendanceRecord record);
        Task<bool> UpdateAsync(AttendanceRecord record);
        Task<bool> DeleteAsync(int attendanceRecordId);
    }
}

