using MidtermTeno.AttendanceManagementSysttem.DTOs;

namespace MidtermTeno.AttendanceManagementSysttem.Interface.ServiceInterface
{
    public interface IDepartmentService
    {
        Task<ServiceResult<PagedResultDTO<DepartmentDTO>>> GetAllAsync(int pageNumber, int pageSize);
        Task<DepartmentDTO?> GetByIdAsync(int id);
        Task<ServiceResult<DepartmentDTO>> CreateAsync(DepartmentDTO dto);
        Task<ServiceResult<bool>> UpdateAsync(int id, DepartmentDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
