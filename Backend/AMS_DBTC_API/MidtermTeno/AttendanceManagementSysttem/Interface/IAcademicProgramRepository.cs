using MidtermTeno.AttendanceManagementSysttem.Model;

namespace MidtermTeno.AttendanceManagementSysttem.Interface
{
    public interface IAcademicProgramRepository
    {
        Task<List<AcademicProgram>> GetAllAsync();
        Task<AcademicProgram?> GetByIdAsync(int programId);
        Task<bool> ExistsAsync(int programId);
        Task<AcademicProgram> AddAsync(AcademicProgram program);
        Task<bool> UpdateAsync(AcademicProgram program);
        Task<bool> DeleteAsync(int programId);
    }
}
