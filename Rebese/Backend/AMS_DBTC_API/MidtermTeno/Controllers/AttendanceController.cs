using Microsoft.AspNetCore.Mvc;
using MidtermTeno.AttendanceManagementSysttem.DTOs;
using MidtermTeno.AttendanceManagementSysttem.Interface;
using MidtermTeno.AttendanceManagementSysttem.Model;
using Swashbuckle.AspNetCore.Annotations;

namespace MidtermTeno.Controllers
{
    /// <summary>
    /// Handles attendance-related endpoints.
    /// </summary>
    [ApiController]
    [Route("api/attendance")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceRepository _attendanceRepo;

        public AttendanceController(IAttendanceRepository attendanceRepo)
        {
            _attendanceRepo = attendanceRepo;
        }
        /// <summary>
        /// Gets attendance records using pagination.
        /// </summary>
        /// <param name="pageNumber">Page index starting at 1.</param>
        /// <param name="pageSize">Number of records per page.</param>
        /// <returns>A paged list of attendance records.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Get all attendance records", Description = "Returns a paginated list of attendance records.")]
        [ProducesResponseType(typeof(PagedResultDTO<AttendanceRecordDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResultDTO<AttendanceRecordDTO>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0) return BadRequest("pageNumber and pageSize must be greater than 0.");

            var records = await _attendanceRepo.GetAllAsync();
            var totalCount = records.Count;
            var items = records
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(ToDto)
                .ToList();

            return Ok(new PagedResultDTO<AttendanceRecordDTO>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Items = items
            });
        }

        /// <summary>
        /// Gets one attendance record by ID.
        /// </summary>
        /// <param name="id">Attendance record primary key.</param>
        /// <returns>The matching attendance record.</returns>
        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Get attendance record by ID", Description = "Returns one attendance record by primary key.")]
        [ProducesResponseType(typeof(AttendanceRecordDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AttendanceRecordDTO>> GetById(int id)
        {
            var record = await _attendanceRepo.GetByIdAsync(id);
            if (record is null) return NotFound();
            return Ok(ToDto(record));
        }

        /// <summary>
        /// Creates a new attendance record.
        /// </summary>
        /// <param name="dto">Attendance payload from the request body.</param>
        /// <returns>The created attendance record.</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Create attendance record", Description = "Creates a new attendance record.")]
        [ProducesResponseType(typeof(AttendanceRecordDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AttendanceRecordDTO>> Create(AttendanceRecordDTO dto)
        {
            if (dto.StudentId <= 0) return BadRequest("StudentId is required.");
            if (dto.CourseId <= 0) return BadRequest("CourseId is required.");
            if (dto.AttendanceDate == default) return BadRequest("AttendanceDate is required.");
            if (!Enum.IsDefined(typeof(AttendanceStatus), dto.Status)) return BadRequest("Invalid AttendanceStatus.");

            var now = DateTime.UtcNow;
            var model = new AttendanceRecord
            {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                AttendanceDate = dto.AttendanceDate.Date,
                Status = dto.Status,
                CreatedAt = now,
                LastUpdatedAt = now,
                CreatedBy = dto.CreatedBy,
                LastUpdatedBy = dto.LastUpdatedBy
            };

            var created = await _attendanceRepo.AddAsync(model);
            return CreatedAtAction(nameof(GetById), new { id = created.AttendanceRecordId }, ToDto(created));
        }

        /// <summary>
        /// Updates an existing attendance record by ID.
        /// </summary>
        /// <param name="id">Attendance record primary key.</param>
        /// <param name="dto">Updated attendance payload.</param>
        /// <returns>No content when update succeeds.</returns>
        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Update attendance record", Description = "Updates an existing attendance record by ID.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Update(int id, AttendanceRecordDTO dto)
        {
            if (dto.StudentId <= 0) return BadRequest("StudentId is required.");
            if (dto.CourseId <= 0) return BadRequest("CourseId is required.");
            if (dto.AttendanceDate == default) return BadRequest("AttendanceDate is required.");
            if (!Enum.IsDefined(typeof(AttendanceStatus), dto.Status)) return BadRequest("Invalid AttendanceStatus.");

            var existing = await _attendanceRepo.GetByIdAsync(id);
            if (existing is null) return NotFound();

            existing.StudentId = dto.StudentId;
            existing.CourseId = dto.CourseId;
            existing.AttendanceDate = dto.AttendanceDate.Date;
            existing.Status = dto.Status;
            existing.LastUpdatedAt = DateTime.UtcNow;

            if (dto.CreatedBy is not null) existing.CreatedBy = dto.CreatedBy;
            if (dto.LastUpdatedBy is not null) existing.LastUpdatedBy = dto.LastUpdatedBy;

            var ok = await _attendanceRepo.UpdateAsync(existing);
            return ok ? NoContent() : NotFound();
        }

        /// <summary>
        /// Deletes an attendance record by ID.
        /// </summary>
        /// <param name="id">Attendance record primary key.</param>
        /// <returns>No content when delete succeeds.</returns>
        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Delete attendance record", Description = "Deletes an attendance record by ID.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _attendanceRepo.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }

        /// <summary>
        /// Marks attendance for one student and course on a specific date.
        /// If a record exists for the same student, course, and date, it is updated.
        /// Otherwise, a new record is created.
        /// </summary>
        /// <param name="dto">Attendance marking payload.</param>
        /// <returns>The created or updated attendance record.</returns>
        [HttpPost("mark")]
        [SwaggerOperation(Summary = "Mark attendance", Description = "Creates or updates attendance for one student, course, and date.")]
        [ProducesResponseType(typeof(AttendanceRecordDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(typeof(AttendanceRecordDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<AttendanceRecordDTO>> Mark(MarkAttendanceDTO dto)
        {
            if (dto.StudentId <= 0) return BadRequest("StudentId is required.");
            if (dto.CourseId <= 0) return BadRequest("CourseId is required.");
            if (dto.AttendanceDate == default) return BadRequest("AttendanceDate is required.");
            if (!Enum.IsDefined(typeof(AttendanceStatus), dto.Status)) return BadRequest("Invalid AttendanceStatus.");

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

                return CreatedAtAction(nameof(GetById), new { id = created.AttendanceRecordId }, ToDto(created));
            }

            existing.Status = dto.Status;
            existing.LastUpdatedAt = now;
            if (dto.LastUpdatedBy is not null) existing.LastUpdatedBy = dto.LastUpdatedBy;

            var ok = await _attendanceRepo.UpdateAsync(existing);
            if (!ok) return NotFound();
            return Ok(ToDto(existing));
        }

        private static AttendanceRecordDTO ToDto(AttendanceRecord model)
        {
            return new AttendanceRecordDTO
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
}

