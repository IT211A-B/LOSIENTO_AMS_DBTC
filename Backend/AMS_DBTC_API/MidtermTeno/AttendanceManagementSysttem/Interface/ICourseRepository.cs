using MidtermTeno.AttendanceManagementSysttem.Model;

namespace MidtermTeno.AttendanceManagementSysttem.Interface
{
    public interface ICourseRepository
    {
        Task<List<Course>> GetAllAsync();
        Task<Course?> GetByIdAsync(int courseId);
        Task<Course> AddAsync(Course course);
        Task<bool> UpdateAsync(Course course);
        Task<bool> DeleteAsync(int courseId);
    }
}
