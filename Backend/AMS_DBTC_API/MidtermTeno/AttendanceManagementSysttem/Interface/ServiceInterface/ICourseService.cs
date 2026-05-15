using MidtermTeno.AttendanceManagementSysttem.DTOs;

namespace MidtermTeno.AttendanceManagementSysttem.Interface.ServiceInterface
{
    public interface ICourseService
    {
        Task<ServiceResult<PagedResultDTO<CourseDTO>>> GetAllAsync(int pageNumber, int pageSize);
        Task<CourseDTO?> GetByIdAsync(int id);
        Task<ServiceResult<CourseDTO>> CreateAsync(CourseDTO dto);
        Task<ServiceResult<bool>> UpdateAsync(int id, CourseDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
