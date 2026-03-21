using Microsoft.AspNetCore.Mvc;
using MidtermTeno.AttendanceManagementSysttem.DTOs;
using MidtermTeno.AttendanceManagementSysttem.Interface;
using MidtermTeno.AttendanceManagementSysttem.Model;
using Swashbuckle.AspNetCore.Annotations;

namespace MidtermTeno.Controllers
{
    /// <summary>
    /// Handles teacher-related endpoints.
    /// </summary>
    [ApiController]
    [Route("api/teachers")]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherRepository _teacherRepo;

        public TeacherController(ITeacherRepository teacherRepo)
        {
            _teacherRepo = teacherRepo;
        }

        /// <summary>
        /// Gets teachers using pagination.
        /// </summary>
        /// <param name="pageNumber">Page index starting at 1.</param>
        /// <param name="pageSize">Number of records per page.</param>
        /// <returns>A paged list of teachers.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Get all teachers", Description = "Returns a paginated list of teachers.")]
        [ProducesResponseType(typeof(PagedResultDTO<TeacherDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResultDTO<TeacherDTO>>> GetAllTeachers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0) return BadRequest("pageNumber and pageSize must be greater than 0.");

            var teachers = await _teacherRepo.GetAllAsync();
            var totalCount = teachers.Count;
            var items = teachers
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(ToDto)
                .ToList();

            return Ok(new PagedResultDTO<TeacherDTO>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Items = items
            });
        }

        /// <summary>
        /// Gets one teacher by ID.
        /// </summary>
        /// <param name="id">Teacher primary key.</param>
        /// <returns>The matching teacher record.</returns>
        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Get teacher by ID", Description = "Returns one teacher by primary key.")]
        [ProducesResponseType(typeof(TeacherDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<TeacherDTO>> GetTeacherById(int id)
        {
            var teacher = await _teacherRepo.GetByIdAsync(id);
            if (teacher is null) return NotFound();
            return Ok(ToDto(teacher));
        }

        /// <summary>
        /// Creates a new teacher.
        /// </summary>
        /// <param name="dto">Teacher payload from the request body.</param>
        /// <returns>The created teacher record.</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Create teacher", Description = "Creates a new teacher record.")]
        [ProducesResponseType(typeof(TeacherDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TeacherDTO>> CreateTeacher(TeacherDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Department))
                return BadRequest("Department is required.");

            var now = DateTime.UtcNow;
            var model = new Teacher
            {
                Department = dto.Department.Trim(),
                CreatedAt = now,
                LastUpdatedAt = now,
                CreatedBy = dto.CreatedBy,
                LastUpdatedBy = dto.LastUpdatedBy
            };

            var created = await _teacherRepo.AddAsync(model);

            return CreatedAtAction(
                nameof(GetTeacherById),
                new { id = created.TeacherId },
                ToDto(created)
            );
        }

        /// <summary>
        /// Updates an existing teacher by ID.
        /// </summary>
        /// <param name="id">Teacher primary key.</param>
        /// <param name="dto">Updated teacher payload.</param>
        /// <returns>No content when update succeeds.</returns>
        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Update teacher", Description = "Updates an existing teacher record by ID.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTeacher(int id, TeacherDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Department))
                return BadRequest("Department is required.");

            var existing = await _teacherRepo.GetByIdAsync(id);
            if (existing is null) return NotFound();

            existing.Department = dto.Department.Trim();
            existing.LastUpdatedAt = DateTime.UtcNow;

            if (dto.CreatedBy is not null) existing.CreatedBy = dto.CreatedBy;
            if (dto.LastUpdatedBy is not null) existing.LastUpdatedBy = dto.LastUpdatedBy;

            var ok = await _teacherRepo.UpdateAsync(existing);
            return ok ? NoContent() : NotFound();
        }

        /// <summary>
        /// Deletes a teacher by ID.
        /// </summary>
        /// <param name="id">Teacher primary key.</param>
        /// <returns>No content when delete succeeds.</returns>
        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Delete teacher", Description = "Deletes a teacher by ID.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var ok = await _teacherRepo.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }

        private static TeacherDTO ToDto(Teacher model)
        {
            return new TeacherDTO
            {
                TeacherId = model.TeacherId,
                Department = model.Department,
                CreatedAt = model.CreatedAt,
                LastUpdatedAt = model.LastUpdatedAt,
                CreatedBy = model.CreatedBy,
                LastUpdatedBy = model.LastUpdatedBy
            };
        }
    }
}
