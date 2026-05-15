using MidtermTeno.AttendanceManagementSysttem.DTOs;
using MidtermTeno.AttendanceManagementSysttem.Interface;
using MidtermTeno.AttendanceManagementSysttem.Interface.ServiceInterface;
using MidtermTeno.AttendanceManagementSysttem.Model;

namespace MidtermTeno.AttendanceManagementSysttem.Service
{
    public class TeacherService : ITeacherService
    {
        private readonly ITeacherRepository _teacherRepo;
        private readonly IDepartmentRepository _departmentRepo;

        public TeacherService(ITeacherRepository teacherRepo, IDepartmentRepository departmentRepo)
        {
            _teacherRepo = teacherRepo;
            _departmentRepo = departmentRepo;
        }

        public async Task<ServiceResult<PagedResultDTO<TeacherDTO>>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return ServiceResult<PagedResultDTO<TeacherDTO>>.ValidationError("pageNumber and pageSize must be greater than 0.");

            var teachers = await _teacherRepo.GetAllAsync();
            var items = teachers.Select(ToDto).ToList();
            return ServiceResult<PagedResultDTO<TeacherDTO>>.Ok(new PagedResultDTO<TeacherDTO>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = items.Count,
                TotalPages = (int)Math.Ceiling(items.Count / (double)pageSize),
                Items = items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
            });
        }

        public async Task<TeacherDTO?> GetByIdAsync(int id)
        {
            var teacher = await _teacherRepo.GetByIdAsync(id);
            return teacher is null ? null : ToDto(teacher);
        }

        public async Task<ServiceResult<TeacherDTO>> CreateAsync(TeacherDTO dto)
        {
            var error = await ValidateAsync(dto);
            if (error is not null) return ServiceResult<TeacherDTO>.ValidationError(error);

            var now = DateTime.UtcNow;
            var created = await _teacherRepo.AddAsync(new Teacher
            {
                DepartmentId = dto.DepartmentId,
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Email = dto.Email.Trim().ToLowerInvariant(),
                CreatedAt = now,
                LastUpdatedAt = now,
                CreatedBy = dto.CreatedBy,
                LastUpdatedBy = dto.LastUpdatedBy
            });
            var loaded = await _teacherRepo.GetByIdAsync(created.TeacherId);
            return ServiceResult<TeacherDTO>.Ok(ToDto(loaded!));
        }

        public async Task<ServiceResult<bool>> UpdateAsync(int id, TeacherDTO dto)
        {
            var error = await ValidateAsync(dto);
            if (error is not null) return ServiceResult<bool>.ValidationError(error);

            var existing = await _teacherRepo.GetByIdAsync(id);
            if (existing is null) return ServiceResult<bool>.NotFoundResult();

            existing.DepartmentId = dto.DepartmentId;
            existing.FirstName = dto.FirstName.Trim();
            existing.LastName = dto.LastName.Trim();
            existing.Email = dto.Email.Trim().ToLowerInvariant();
            existing.LastUpdatedAt = DateTime.UtcNow;
            if (dto.CreatedBy is not null) existing.CreatedBy = dto.CreatedBy;
            if (dto.LastUpdatedBy is not null) existing.LastUpdatedBy = dto.LastUpdatedBy;

            var ok = await _teacherRepo.UpdateAsync(existing);
            return ok ? ServiceResult<bool>.Ok(true) : ServiceResult<bool>.NotFoundResult();
        }

        public async Task<bool> DeleteAsync(int id) => await _teacherRepo.DeleteAsync(id);

        private async Task<string?> ValidateAsync(TeacherDTO dto)
        {
            if (dto.DepartmentId <= 0) return "DepartmentId is required.";
            if (!await _departmentRepo.ExistsAsync(dto.DepartmentId)) return "Department not found.";
            if (string.IsNullOrWhiteSpace(dto.FirstName)) return "FirstName is required.";
            if (string.IsNullOrWhiteSpace(dto.LastName)) return "LastName is required.";
            if (string.IsNullOrWhiteSpace(dto.Email)) return "Email is required.";
            return null;
        }

        private static TeacherDTO ToDto(Teacher model) => new()
        {
            TeacherId = model.TeacherId,
            DepartmentId = model.DepartmentId,
            DepartmentName = model.Department?.DepartmentName,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Email = model.Email,
            CreatedAt = model.CreatedAt,
            LastUpdatedAt = model.LastUpdatedAt,
            CreatedBy = model.CreatedBy,
            LastUpdatedBy = model.LastUpdatedBy
        };
    }
}
