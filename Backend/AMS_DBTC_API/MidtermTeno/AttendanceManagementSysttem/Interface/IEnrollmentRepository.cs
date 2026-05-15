using MidtermTeno.AttendanceManagementSysttem.Model;

namespace MidtermTeno.AttendanceManagementSysttem.Interface
{
    public interface IEnrollmentRepository
    {
        Task<List<Enrollment>> GetAllAsync();
        Task<Enrollment?> GetByIdAsync(int enrollmentId);
        Task<bool> IsActiveEnrollmentAsync(int studentId, int courseId);
        Task<Enrollment> AddAsync(Enrollment enrollment);
        Task<bool> UpdateAsync(Enrollment enrollment);
        Task<bool> DeleteAsync(int enrollmentId);
    }
}
