using AMS_DBTC_API.AttendanceManagementSysttem.Interface;
using MidtermTeno.AttendanceManagementSysttem.DTOs;
using MidtermTeno.AttendanceManagementSysttem.Interface;
using MidtermTeno.AttendanceManagementSysttem.Interface.ServiceInterface;
using MidtermTeno.AttendanceManagementSysttem.Model;

namespace MidtermTeno.AttendanceManagementSysttem.Service
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepo;
        private readonly IEnrollmentRepository _enrollmentRepo;
        private readonly ICourseRepository _courseRepo;

        public AttendanceService(
            IAttendanceRepository attendanceRepo,
            IEnrollmentRepository enrollmentRepo,
            ICourseRepository courseRepo)
        {
            _attendanceRepo = attendanceRepo;
            _enrollmentRepo = enrollmentRepo;
            _courseRepo = courseRepo;
        }

        public async Task<ServiceResult<PagedResultDTO<AttendanceRecordDTO>>> GetAllAsync(
            int pageNumber, int pageSize, int? studentId = null, int? teacherId = null)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return ServiceResult<PagedResultDTO<AttendanceRecordDTO>>.ValidationError("pageNumber and pageSize must be greater than 0.");

            var records = await _attendanceRepo.GetAllAsync(studentId, teacherId);
            var items = records.Select(ToDto).ToList();
            return ServiceResult<PagedResultDTO<AttendanceRecordDTO>>.Ok(new PagedResultDTO<AttendanceRecordDTO>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = items.Count,
                TotalPages = (int)Math.Ceiling(items.Count / (double)pageSize),
                Items = items.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
            });
        }

        public async Task<AttendanceRecordDTO?> GetByIdAsync(int id)
        {
            var record = await _attendanceRepo.GetByIdAsync(id);
            return record is null ? null : ToDto(record);
        }

        public async Task<ServiceResult<AttendanceRecordDTO>> CreateAsync(AttendanceRecordDTO dto)
        {
            var validationError = await ValidateAttendanceAsync(dto);
            if (validationError is not null)
                return ServiceResult<AttendanceRecordDTO>.ValidationError(validationError);

            var now = DateTime.UtcNow;
            var created = await _attendanceRepo.AddAsync(new AttendanceRecord
            {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                AttendanceDate = dto.AttendanceDate.Date,
                Status = dto.Status,
                CreatedAt = now,
                LastUpdatedAt = now,
                CreatedBy = dto.CreatedBy,
                LastUpdatedBy = dto.LastUpdatedBy
            });
            return ServiceResult<AttendanceRecordDTO>.Ok(ToDto(created));
        }

        public async Task<ServiceResult<bool>> UpdateAsync(int id, AttendanceRecordDTO dto)
        {
            var validationError = await ValidateAttendanceAsync(dto);
            if (validationError is not null)
                return ServiceResult<bool>.ValidationError(validationError);

            var existing = await _attendanceRepo.GetByIdAsync(id);
            if (existing is null)
                return ServiceResult<bool>.NotFoundResult();

            existing.StudentId = dto.StudentId;
            existing.CourseId = dto.CourseId;
            existing.AttendanceDate = dto.AttendanceDate.Date;
            existing.Status = dto.Status;
            existing.LastUpdatedAt = DateTime.UtcNow;
            if (dto.CreatedBy is not null) existing.CreatedBy = dto.CreatedBy;
            if (dto.LastUpdatedBy is not null) existing.LastUpdatedBy = dto.LastUpdatedBy;

            var ok = await _attendanceRepo.UpdateAsync(existing);
            return ok ? ServiceResult<bool>.Ok(true) : ServiceResult<bool>.NotFoundResult();
        }

        public async Task<bool> DeleteAsync(int id) => await _attendanceRepo.DeleteAsync(id);

        public async Task<ServiceResult<MarkAttendanceResultDTO>> MarkAsync(MarkAttendanceDTO dto, int? teacherId = null)
        {
            if (dto.StudentId <= 0)
                return ServiceResult<MarkAttendanceResultDTO>.ValidationError("StudentId is required.");
            if (dto.CourseId <= 0)
                return ServiceResult<MarkAttendanceResultDTO>.ValidationError("CourseId is required.");
            if (dto.AttendanceDate == default)
                return ServiceResult<MarkAttendanceResultDTO>.ValidationError("AttendanceDate is required.");
            if (!Enum.IsDefined(typeof(AttendanceStatus), dto.Status))
                return ServiceResult<MarkAttendanceResultDTO>.ValidationError("Invalid AttendanceStatus.");

            if (!await _enrollmentRepo.IsActiveEnrollmentAsync(dto.StudentId, dto.CourseId))
                return ServiceResult<MarkAttendanceResultDTO>.ValidationError("Student is not actively enrolled in this course.");

            if (teacherId.HasValue && !await _courseRepo.IsOwnedByTeacherAsync(dto.CourseId, teacherId.Value))
                return ServiceResult<MarkAttendanceResultDTO>.ValidationError("You are not assigned to this course.");

            var now = DateTime.UtcNow;
            var normalizedDate = dto.AttendanceDate.Date;
            var existing = await _attendanceRepo.GetByStudentCourseDateAsync(dto.StudentId, dto.CourseId, normalizedDate);

            if (existing is null)
            {
                var created = await _attendanceRepo.AddAsync(new AttendanceRecord
                {
                    StudentId = dto.StudentId,
                    CourseId = dto.CourseId,
                    AttendanceDate = normalizedDate,
                    Status = dto.Status,
                    CreatedAt = now,
                    LastUpdatedAt = now,
                    CreatedBy = dto.CreatedBy,
                    LastUpdatedBy = dto.LastUpdatedBy
                });

                return ServiceResult<MarkAttendanceResultDTO>.Ok(new MarkAttendanceResultDTO
                {
                    Record = ToDto(created),
                    WasCreated = true
                });
            }

            existing.Status = dto.Status;
            existing.LastUpdatedAt = now;
            if (dto.LastUpdatedBy is not null) existing.LastUpdatedBy = dto.LastUpdatedBy;

            var ok = await _attendanceRepo.UpdateAsync(existing);
            if (!ok) return ServiceResult<MarkAttendanceResultDTO>.NotFoundResult();

            return ServiceResult<MarkAttendanceResultDTO>.Ok(new MarkAttendanceResultDTO
            {
                Record = ToDto(existing),
                WasCreated = false
            });
        }

        private async Task<string?> ValidateAttendanceAsync(AttendanceRecordDTO dto)
        {
            if (dto.StudentId <= 0) return "StudentId is required.";
            if (dto.CourseId <= 0) return "CourseId is required.";
            if (dto.AttendanceDate == default) return "AttendanceDate is required.";
            if (!Enum.IsDefined(typeof(AttendanceStatus), dto.Status)) return "Invalid AttendanceStatus.";
            if (!await _enrollmentRepo.IsActiveEnrollmentAsync(dto.StudentId, dto.CourseId))
                return "Student is not actively enrolled in this course.";
            return null;
        }

        private static AttendanceRecordDTO ToDto(AttendanceRecord model) => new()
        {
            AttendanceRecordId = model.AttendanceRecordId,
            StudentId = model.StudentId,
            CourseId = model.CourseId,
            AttendanceDate = model.AttendanceDate,
            Status = model.Status,
            CreatedAt = model.CreatedAt,
            LastUpdatedAt = model.LastUpdatedAt,
            CreatedBy = model.CreatedBy,
            LastUpdatedBy = model.LastUpdatedBy
        };
    }
}
