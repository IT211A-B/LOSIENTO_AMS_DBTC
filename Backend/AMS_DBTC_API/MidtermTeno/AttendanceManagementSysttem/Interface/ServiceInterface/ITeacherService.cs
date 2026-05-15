using MidtermTeno.AttendanceManagementSysttem.DTOs;

namespace MidtermTeno.AttendanceManagementSysttem.Interface.ServiceInterface
{
    public interface ITeacherService
    {
        Task<ServiceResult<PagedResultDTO<TeacherDTO>>> GetAllAsync(int pageNumber, int pageSize);
        Task<TeacherDTO?> GetByIdAsync(int id);
        Task<ServiceResult<TeacherDTO>> CreateAsync(TeacherDTO dto);
        Task<ServiceResult<bool>> UpdateAsync(int id, TeacherDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
