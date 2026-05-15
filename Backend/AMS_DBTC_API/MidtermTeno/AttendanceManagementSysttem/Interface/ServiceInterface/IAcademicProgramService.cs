using MidtermTeno.AttendanceManagementSysttem.DTOs;

namespace MidtermTeno.AttendanceManagementSysttem.Interface.ServiceInterface
{
    public interface IAcademicProgramService
    {
        Task<ServiceResult<PagedResultDTO<AcademicProgramDTO>>> GetAllAsync(int pageNumber, int pageSize);
        Task<AcademicProgramDTO?> GetByIdAsync(int id);
        Task<ServiceResult<AcademicProgramDTO>> CreateAsync(AcademicProgramDTO dto);
        Task<ServiceResult<bool>> UpdateAsync(int id, AcademicProgramDTO dto);
        Task<bool> DeleteAsync(int id);
    }
}
