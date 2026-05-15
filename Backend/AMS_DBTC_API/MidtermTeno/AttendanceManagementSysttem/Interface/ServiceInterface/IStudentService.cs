using MidtermTeno.AttendanceManagementSysttem.DTOs;

namespace MidtermTeno.AttendanceManagementSysttem.Interface.ServiceInterface
{
    public interface IStudentService
    {
        Task<ServiceResult<PagedResultDTO<StudentDTO>>> GetAllAsync(int pageNumber, int pageSize);
        Task<StudentDTO?> GetByIdAsync(int id);
        Task<ServiceResult<StudentDTO>> CreateAsync(StudentDTO dto);
        Task<ServiceResult<bool>> UpdateAsync(int id, StudentDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
