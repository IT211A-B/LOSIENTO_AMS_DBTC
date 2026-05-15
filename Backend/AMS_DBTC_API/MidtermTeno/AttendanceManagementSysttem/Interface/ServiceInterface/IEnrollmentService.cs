using MidtermTeno.AttendanceManagementSysttem.DTOs;

namespace MidtermTeno.AttendanceManagementSysttem.Interface.ServiceInterface
{
    public interface IEnrollmentService
    {
        Task<ServiceResult<PagedResultDTO<EnrollmentDTO>>> GetAllAsync(int pageNumber, int pageSize);
        Task<EnrollmentDTO?> GetByIdAsync(int id);
        Task<ServiceResult<EnrollmentDTO>> CreateAsync(EnrollmentDTO dto);
        Task<ServiceResult<bool>> UpdateAsync(int id, EnrollmentDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
