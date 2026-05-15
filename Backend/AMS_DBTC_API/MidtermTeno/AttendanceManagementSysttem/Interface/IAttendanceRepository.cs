using MidtermTeno.AttendanceManagementSysttem.Model;

namespace AMS_DBTC_API.AttendanceManagementSysttem.Interface
{
    public interface IAttendanceRepository
    {
        Task<List<AttendanceRecord>> GetAllAsync(int? studentId = null, int? teacherId = null);
        Task<AttendanceRecord?> GetByIdAsync(int attendanceRecordId);

        // Returns the record for one student + one course on one date.
        Task<AttendanceRecord?> GetByStudentCourseDateAsync(int studentId, int courseId, DateTime attendanceDate);

        Task<AttendanceRecord> AddAsync(AttendanceRecord record);
        Task<bool> UpdateAsync(AttendanceRecord record);
        Task<bool> DeleteAsync(int attendanceRecordId);
    }
}

