using MidtermTeno.AttendanceManagementSysttem.DTOs;
using MidtermTeno.AttendanceManagementSysttem.Interface;
using MidtermTeno.AttendanceManagementSysttem.Interface.ServiceInterface;
using MidtermTeno.AttendanceManagementSysttem.Model;

namespace MidtermTeno.AttendanceManagementSysttem.Service
{
    public class EnrollmentService : IEnrollmentService
    {
        private readonly IEnrollmentRepository _enrollmentRepo;
        private readonly IStudentRepository _studentRepo;
        private readonly ICourseRepository _courseRepo;

        public EnrollmentService(
            IEnrollmentRepository enrollmentRepo,
            IStudentRepository studentRepo,
            ICourseRepository courseRepo)
        {
            _enrollmentRepo = enrollmentRepo;
            _studentRepo = studentRepo;
            _courseRepo = courseRepo;
        }

        public async Task<ServiceResult<PagedResultDTO<EnrollmentDTO>>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return ServiceResult<PagedResultDTO<EnrollmentDTO>>.ValidationError("pageNumber and pageSize must be greater than 0.");

            var items = await _enrollmentRepo.GetAllAsync();
            var dtos = items.Select(ToDto).ToList();
            return ServiceResult<PagedResultDTO<EnrollmentDTO>>.Ok(new PagedResultDTO<EnrollmentDTO>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = dtos.Count,
                TotalPages = (int)Math.Ceiling(dtos.Count / (double)pageSize),
                Items = dtos.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
            });
        }

        public async Task<EnrollmentDTO?> GetByIdAsync(int id)
        {
            var entity = await _enrollmentRepo.GetByIdAsync(id);
            return entity is null ? null : ToDto(entity);
        }

        public async Task<ServiceResult<EnrollmentDTO>> CreateAsync(EnrollmentDTO dto)
        {
            var error = await ValidateAsync(dto);
            if (error is not null) return ServiceResult<EnrollmentDTO>.ValidationError(error);

            var now = DateTime.UtcNow;
            var created = await _enrollmentRepo.AddAsync(new Enrollment
            {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                Status = dto.Status == default ? EnrollmentStatus.Active : dto.Status,
                EnrolledAt = dto.EnrolledAt == default ? now : dto.EnrolledAt,
                CreatedAt = now,
                LastUpdatedAt = now
            });
            return ServiceResult<EnrollmentDTO>.Ok(ToDto(created));
        }

        public async Task<ServiceResult<bool>> UpdateAsync(int id, EnrollmentDTO dto)
        {
            var error = await ValidateAsync(dto);
            if (error is not null) return ServiceResult<bool>.ValidationError(error);

            var existing = await _enrollmentRepo.GetByIdAsync(id);
            if (existing is null) return ServiceResult<bool>.NotFoundResult();

            existing.StudentId = dto.StudentId;
            existing.CourseId = dto.CourseId;
            existing.Status = dto.Status;
            existing.EnrolledAt = dto.EnrolledAt;
            existing.LastUpdatedAt = DateTime.UtcNow;

            var ok = await _enrollmentRepo.UpdateAsync(existing);
            return ok ? ServiceResult<bool>.Ok(true) : ServiceResult<bool>.NotFoundResult();
        }

        public async Task<bool> DeleteAsync(int id) => await _enrollmentRepo.DeleteAsync(id);

        private async Task<string?> ValidateAsync(EnrollmentDTO dto)
        {
            if (dto.StudentId <= 0) return "StudentId is required.";
            if (dto.CourseId <= 0) return "CourseId is required.";
            if (await _studentRepo.GetByIdAsync(dto.StudentId) is null) return "Student not found.";
            if (await _courseRepo.GetByIdAsync(dto.CourseId) is null) return "Course not found.";
            return null;
        }

        private static EnrollmentDTO ToDto(Enrollment m) => new()
        {
            EnrollmentId = m.EnrollmentId,
            StudentId = m.StudentId,
            CourseId = m.CourseId,
            Status = m.Status,
            EnrolledAt = m.EnrolledAt,
            CreatedAt = m.CreatedAt,
            LastUpdatedAt = m.LastUpdatedAt
        };
    }
}
