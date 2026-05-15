using MidtermTeno.AttendanceManagementSysttem.DTOs;
using MidtermTeno.AttendanceManagementSysttem.Interface;
using MidtermTeno.AttendanceManagementSysttem.Interface.ServiceInterface;
using MidtermTeno.AttendanceManagementSysttem.Model;

namespace MidtermTeno.AttendanceManagementSysttem.Service
{
    public class AcademicProgramService : IAcademicProgramService
    {
        private readonly IAcademicProgramRepository _programRepo;
        private readonly IDepartmentRepository _departmentRepo;

        public AcademicProgramService(IAcademicProgramRepository programRepo, IDepartmentRepository departmentRepo)
        {
            _programRepo = programRepo;
            _departmentRepo = departmentRepo;
        }

        public async Task<ServiceResult<PagedResultDTO<AcademicProgramDTO>>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return ServiceResult<PagedResultDTO<AcademicProgramDTO>>.ValidationError("pageNumber and pageSize must be greater than 0.");

            var items = await _programRepo.GetAllAsync();
            var dtos = items.Select(ToDto).ToList();
            return ServiceResult<PagedResultDTO<AcademicProgramDTO>>.Ok(new PagedResultDTO<AcademicProgramDTO>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = dtos.Count,
                TotalPages = (int)Math.Ceiling(dtos.Count / (double)pageSize),
                Items = dtos.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
            });
        }

        public async Task<AcademicProgramDTO?> GetByIdAsync(int id)
        {
            var entity = await _programRepo.GetByIdAsync(id);
            return entity is null ? null : ToDto(entity);
        }

        public async Task<ServiceResult<AcademicProgramDTO>> CreateAsync(AcademicProgramDTO dto)
        {
            var error = await ValidateAsync(dto);
            if (error is not null) return ServiceResult<AcademicProgramDTO>.ValidationError(error);

            var now = DateTime.UtcNow;
            var created = await _programRepo.AddAsync(new AcademicProgram
            {
                DepartmentId = dto.DepartmentId,
                ProgramCode = dto.ProgramCode.Trim(),
                ProgramName = dto.ProgramName.Trim(),
                CreatedAt = now,
                LastUpdatedAt = now
            });
            var loaded = await _programRepo.GetByIdAsync(created.ProgramId);
            return ServiceResult<AcademicProgramDTO>.Ok(ToDto(loaded!));
        }

        public async Task<ServiceResult<bool>> UpdateAsync(int id, AcademicProgramDTO dto)
        {
            var error = await ValidateAsync(dto);
            if (error is not null) return ServiceResult<bool>.ValidationError(error);

            var existing = await _programRepo.GetByIdAsync(id);
            if (existing is null) return ServiceResult<bool>.NotFoundResult();

            existing.DepartmentId = dto.DepartmentId;
            existing.ProgramCode = dto.ProgramCode.Trim();
            existing.ProgramName = dto.ProgramName.Trim();
            existing.LastUpdatedAt = DateTime.UtcNow;

            var ok = await _programRepo.UpdateAsync(existing);
            return ok ? ServiceResult<bool>.Ok(true) : ServiceResult<bool>.NotFoundResult();
        }

        public async Task<bool> DeleteAsync(int id) => await _programRepo.DeleteAsync(id);

        private async Task<string?> ValidateAsync(AcademicProgramDTO dto)
        {
            if (dto.DepartmentId <= 0) return "DepartmentId is required.";
            if (!await _departmentRepo.ExistsAsync(dto.DepartmentId)) return "Department not found.";
            if (string.IsNullOrWhiteSpace(dto.ProgramCode)) return "ProgramCode is required.";
            if (string.IsNullOrWhiteSpace(dto.ProgramName)) return "ProgramName is required.";
            return null;
        }

        private static AcademicProgramDTO ToDto(AcademicProgram m) => new()
        {
            ProgramId = m.ProgramId,
            DepartmentId = m.DepartmentId,
            ProgramCode = m.ProgramCode,
            ProgramName = m.ProgramName,
            DepartmentName = m.Department?.DepartmentName,
            CreatedAt = m.CreatedAt,
            LastUpdatedAt = m.LastUpdatedAt
        };
    }
}
