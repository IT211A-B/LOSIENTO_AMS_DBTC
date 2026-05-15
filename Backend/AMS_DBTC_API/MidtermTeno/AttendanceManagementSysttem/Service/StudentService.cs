using MidtermTeno.AttendanceManagementSysttem.DTOs;
using MidtermTeno.AttendanceManagementSysttem.Interface;
using MidtermTeno.AttendanceManagementSysttem.Interface.ServiceInterface;
using MidtermTeno.AttendanceManagementSysttem.Model;

namespace MidtermTeno.AttendanceManagementSysttem.Service
{
    public class StudentService : IStudentService
    {
        private readonly IStudentRepository _studentRepo;
        private readonly IAcademicProgramRepository _programRepo;

        public StudentService(IStudentRepository studentRepo, IAcademicProgramRepository programRepo)
        {
            _studentRepo = studentRepo;
            _programRepo = programRepo;
        }

        public async Task<ServiceResult<PagedResultDTO<StudentDTO>>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return ServiceResult<PagedResultDTO<StudentDTO>>.ValidationError("pageNumber and pageSize must be greater than 0.");

            var students = await _studentRepo.GetAllAsync();
            var items = students.Select(ToDto).ToList();
            return ServiceResult<PagedResultDTO<StudentDTO>>.Ok(new PagedResultDTO<StudentDTO>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = items.Count,
                TotalPages = (int)Math.Ceiling(items.Count / (double)pageSize),
                Items = items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
            });
        }

        public async Task<StudentDTO?> GetByIdAsync(int id)
        {
            var student = await _studentRepo.GetByIdAsync(id);
            return student is null ? null : ToDto(student);
        }

        public async Task<ServiceResult<StudentDTO>> CreateAsync(StudentDTO dto)
        {
            var error = await ValidateAsync(dto);
            if (error is not null) return ServiceResult<StudentDTO>.ValidationError(error);

            var now = DateTime.UtcNow;
            var created = await _studentRepo.AddAsync(new Student
            {
                ProgramId = dto.ProgramId,
                StudentNumber = dto.StudentNumber.Trim(),
                FirstName = dto.FirstName.Trim(),
                LastName = dto.LastName.Trim(),
                Year_Level = dto.Year_Level.Trim(),
                Email = dto.Email.Trim().ToLowerInvariant(),
                CreatedAt = now,
                LastUpdatedAt = now,
                CreatedBy = dto.CreatedBy,
                LastUpdatedBy = dto.LastUpdatedBy
            });
            var loaded = await _studentRepo.GetByIdAsync(created.StudentId);
            return ServiceResult<StudentDTO>.Ok(ToDto(loaded!));
        }

        public async Task<ServiceResult<bool>> UpdateAsync(int id, StudentDTO dto)
        {
            var error = await ValidateAsync(dto);
            if (error is not null) return ServiceResult<bool>.ValidationError(error);

            var existing = await _studentRepo.GetByIdAsync(id);
            if (existing is null) return ServiceResult<bool>.NotFoundResult();

            existing.ProgramId = dto.ProgramId;
            existing.StudentNumber = dto.StudentNumber.Trim();
            existing.FirstName = dto.FirstName.Trim();
            existing.LastName = dto.LastName.Trim();
            existing.Year_Level = dto.Year_Level.Trim();
            existing.Email = dto.Email.Trim().ToLowerInvariant();
            existing.LastUpdatedAt = DateTime.UtcNow;
            if (dto.CreatedBy is not null) existing.CreatedBy = dto.CreatedBy;
            if (dto.LastUpdatedBy is not null) existing.LastUpdatedBy = dto.LastUpdatedBy;

            var ok = await _studentRepo.UpdateAsync(existing);
            return ok ? ServiceResult<bool>.Ok(true) : ServiceResult<bool>.NotFoundResult();
        }

        public async Task<bool> DeleteAsync(int id) => await _studentRepo.DeleteAsync(id);

        private async Task<string?> ValidateAsync(StudentDTO dto)
        {
            if (dto.ProgramId <= 0) return "ProgramId is required.";
            if (!await _programRepo.ExistsAsync(dto.ProgramId)) return "Program not found.";
            if (string.IsNullOrWhiteSpace(dto.StudentNumber)) return "StudentNumber is required.";
            if (string.IsNullOrWhiteSpace(dto.FirstName)) return "FirstName is required.";
            if (string.IsNullOrWhiteSpace(dto.LastName)) return "LastName is required.";
            if (string.IsNullOrWhiteSpace(dto.Year_Level)) return "Year_Level is required.";
            if (string.IsNullOrWhiteSpace(dto.Email)) return "Email is required.";
            return null;
        }

        private static StudentDTO ToDto(Student model) => new()
        {
            StudentId = model.StudentId,
            ProgramId = model.ProgramId,
            ProgramName = model.Program?.ProgramName,
            StudentNumber = model.StudentNumber,
            FirstName = model.FirstName,
            LastName = model.LastName,
            Year_Level = model.Year_Level,
            Email = model.Email,
            CreatedAt = model.CreatedAt,
            LastUpdatedAt = model.LastUpdatedAt,
            CreatedBy = model.CreatedBy,
            LastUpdatedBy = model.LastUpdatedBy
        };
    }
}
