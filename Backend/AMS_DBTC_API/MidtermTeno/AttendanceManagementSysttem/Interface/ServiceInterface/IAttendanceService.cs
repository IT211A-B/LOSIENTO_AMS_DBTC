using MidtermTeno.AttendanceManagementSysttem.DTOs;

namespace MidtermTeno.AttendanceManagementSysttem.Interface.ServiceInterface
{
    public interface IAttendanceService
    {
        Task<ServiceResult<PagedResultDTO<AttendanceRecordDTO>>> GetAllAsync(int pageNumber, int pageSize, int? studentId = null, int? teacherId = null);
        Task<AttendanceRecordDTO?> GetByIdAsync(int id);
        Task<ServiceResult<AttendanceRecordDTO>> CreateAsync(AttendanceRecordDTO dto);
        Task<ServiceResult<bool>> UpdateAsync(int id, AttendanceRecordDTO dto);
        Task<bool> DeleteAsync(int id);
        Task<ServiceResult<MarkAttendanceResultDTO>> MarkAsync(MarkAttendanceDTO dto, int? teacherId = null);
    }
}
