using Microsoft.AspNetCore.Mvc;
using MidtermTeno.AttendanceManagementSysttem.DTOs;
using MidtermTeno.AttendanceManagementSysttem.Interface;
using MidtermTeno.AttendanceManagementSysttem.Model;
using Swashbuckle.AspNetCore.Annotations;

namespace MidtermTeno.Controllers
{
    /// <summary>
    /// Handles course-related endpoints.
    /// </summary>
    [ApiController]
    [Route("api/courses")]
    public class CourseController : ControllerBase
    {
        private readonly ICourseRepository _courseRepo;

        public CourseController(ICourseRepository courseRepo)
        {
            _courseRepo = courseRepo;
        }

        /// <summary>
        /// Gets courses using pagination.
        /// </summary>
        /// <param name="pageNumber">Page index starting at 1.</param>
        /// <param name="pageSize">Number of records per page.</param>
        /// <returns>A paged list of courses.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Get all courses", Description = "Returns a paginated list of courses.")]
        [ProducesResponseType(typeof(PagedResultDTO<CourseDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResultDTO<CourseDTO>>> GetAllCourses([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            if (pageNumber <= 0 || pageSize <= 0) return BadRequest("pageNumber and pageSize must be greater than 0.");

            var courses = await _courseRepo.GetAllAsync();
            var totalCount = courses.Count;
            var items = courses
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(ToDto)
                .ToList();

            return Ok(new PagedResultDTO<CourseDTO>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Items = items
            });
        }

        /// <summary>
        /// Gets one course by ID.
        /// </summary>
        /// <param name="id">Course primary key.</param>
        /// <returns>The matching course record.</returns>
        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Get course by ID", Description = "Returns one course by primary key.")]
        [ProducesResponseType(typeof(CourseDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CourseDTO>> GetCourseById(int id)
        {
            var course = await _courseRepo.GetByIdAsync(id);
            if (course is null) return NotFound();
            return Ok(ToDto(course));
        }

        /// <summary>
        /// Creates a new course.
        /// </summary>
        /// <param name="dto">Course payload from the request body.</param>
        /// <returns>The created course record.</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Create course", Description = "Creates a new course record.")]
        [ProducesResponseType(typeof(CourseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CourseDTO>> CreateCourse(CourseDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CourseName))
                return BadRequest("CourseName is required.");
            if (string.IsNullOrWhiteSpace(dto.CourseCode))
                return BadRequest("CourseCode is required.");

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

            return CreatedAtAction(
                nameof(GetCourseById),
                new { id = created.CourseId },
                ToDto(created)
            );
        }

        /// <summary>
        /// Updates an existing course by ID.
        /// </summary>
        /// <param name="id">Course primary key.</param>
        /// <param name="dto">Updated course payload.</param>
        /// <returns>No content when update succeeds.</returns>
        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Update course", Description = "Updates an existing course record by ID.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCourse(int id, CourseDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.CourseName))
                return BadRequest("CourseName is required.");
            if (string.IsNullOrWhiteSpace(dto.CourseCode))
                return BadRequest("CourseCode is required.");

            var existing = await _courseRepo.GetByIdAsync(id);
            if (existing is null) return NotFound();

            existing.CourseName = dto.CourseName.Trim();
            existing.CourseCode = dto.CourseCode.Trim();
            existing.Description = dto.Description;
            existing.TeacherId = dto.TeacherId;
            existing.LastUpdatedAt = DateTime.UtcNow;

            if (dto.CreatedBy is not null) existing.CreatedBy = dto.CreatedBy;
            if (dto.LastUpdatedBy is not null) existing.LastUpdatedBy = dto.LastUpdatedBy;

            var ok = await _courseRepo.UpdateAsync(existing);
            return ok ? NoContent() : NotFound();
        }

        /// <summary>
        /// Deletes a course by ID.
        /// </summary>
        /// <param name="id">Course primary key.</param>
        /// <returns>No content when delete succeeds.</returns>
        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Delete course", Description = "Deletes a course by ID.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var ok = await _courseRepo.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
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
