using MidtermTeno.AttendanceManagementSysttem.DTOs;
using MidtermTeno.AttendanceManagementSysttem.Interface;
using MidtermTeno.AttendanceManagementSysttem.Interface.ServiceInterface;
using MidtermTeno.AttendanceManagementSysttem.Model;

namespace MidtermTeno.AttendanceManagementSysttem.Service
{
    public class CourseService : ICourseService
    {
        private readonly ICourseRepository _courseRepo;

        public CourseService(ICourseRepository courseRepo)
        {
            _courseRepo = courseRepo;
        }

        public async Task<ServiceResult<PagedResultDTO<CourseDTO>>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return ServiceResult<PagedResultDTO<CourseDTO>>.ValidationError("pageNumber and pageSize must be greater than 0.");

            var courses = await _courseRepo.GetAllAsync();
            var totalCount = courses.Count;
            var items = courses
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(ToDto)
                .ToList();

            return ServiceResult<PagedResultDTO<CourseDTO>>.Ok(new PagedResultDTO<CourseDTO>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Items = items
            });
        }

        public async Task<CourseDTO?> GetByIdAsync(int id)
        {
            var course = await _courseRepo.GetByIdAsync(id);
            return course is null ? null : ToDto(course);
        }

        public async Task<ServiceResult<CourseDTO>> CreateAsync(CourseDTO dto)
        {
            var validationError = ValidateCourseDto(dto);
            if (validationError is not null)
                return ServiceResult<CourseDTO>.ValidationError(validationError);

            var now = DateTime.UtcNow;
            var model = new Course
            {
                CourseName = dto.CourseName.Trim(),
                CourseCode = dto.CourseCode.Trim(),
                Description = dto.Description,
                TeacherId = dto.TeacherId,
                CreatedAt = now,
                LastUpdatedAt = now,
                CreatedBy = dto.CreatedBy,
                LastUpdatedBy = dto.LastUpdatedBy
            };

            var created = await _courseRepo.AddAsync(model);
            return ServiceResult<CourseDTO>.Ok(ToDto(created));
        }

        public async Task<ServiceResult<bool>> UpdateAsync(int id, CourseDTO dto)
        {
            var validationError = ValidateCourseDto(dto);
            if (validationError is not null)
                return ServiceResult<bool>.ValidationError(validationError);

            var existing = await _courseRepo.GetByIdAsync(id);
            if (existing is null)
                return ServiceResult<bool>.NotFoundResult();

            existing.CourseName = dto.CourseName.Trim();
            existing.CourseCode = dto.CourseCode.Trim();
            existing.Description = dto.Description;
            existing.TeacherId = dto.TeacherId;
            existing.LastUpdatedAt = DateTime.UtcNow;

            if (dto.CreatedBy is not null) existing.CreatedBy = dto.CreatedBy;
            if (dto.LastUpdatedBy is not null) existing.LastUpdatedBy = dto.LastUpdatedBy;

            var ok = await _courseRepo.UpdateAsync(existing);
            return ok ? ServiceResult<bool>.Ok(true) : ServiceResult<bool>.NotFoundResult();
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _courseRepo.DeleteAsync(id);
        }

        private static string? ValidateCourseDto(CourseDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CourseName))
                return "CourseName is required.";
            if (string.IsNullOrWhiteSpace(dto.CourseCode))
                return "CourseCode is required.";
            return null;
        }

        private static CourseDTO ToDto(Course model)
        {
            return new CourseDTO
            {
                CourseId = model.CourseId,
                CourseName = model.CourseName,
                CourseCode = model.CourseCode,
                Description = model.Description,
                TeacherId = model.TeacherId,
                CreatedAt = model.CreatedAt,
                LastUpdatedAt = model.LastUpdatedAt,
                CreatedBy = model.CreatedBy,
                LastUpdatedBy = model.LastUpdatedBy
            };
        }
    }
}
