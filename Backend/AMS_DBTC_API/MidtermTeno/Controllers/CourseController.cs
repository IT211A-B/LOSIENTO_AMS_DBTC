using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MidtermTeno.AttendanceManagementSysttem.Constants;
using MidtermTeno.AttendanceManagementSysttem.DTOs;
using MidtermTeno.AttendanceManagementSysttem.Interface.ServiceInterface;
using Swashbuckle.AspNetCore.Annotations;

namespace MidtermTeno.Controllers
{
    /// <summary>
    /// Handles course-related endpoints.
    /// </summary>
    [ApiController]
    [Route("api/courses")]
    [Authorize]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;

        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
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
            var result = await _courseService.GetAllAsync(pageNumber, pageSize);
            if (result.ErrorMessage is not null) return BadRequest(result.ErrorMessage);
            return Ok(result.Data);
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
            var course = await _courseService.GetByIdAsync(id);
            if (course is null) return NotFound();
            return Ok(course);
        }

        /// <summary>
        /// Creates a new course.
        /// </summary>
        /// <param name="dto">Course payload from the request body.</param>
        /// <returns>The created course record.</returns>
        [HttpPost]
        [Authorize(Roles = AppRoles.Admin)]
        [SwaggerOperation(Summary = "Create course", Description = "Creates a new course record.")]
        [ProducesResponseType(typeof(CourseDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<CourseDTO>> CreateCourse(CourseDTO dto)
        {
            var result = await _courseService.CreateAsync(dto);
            if (result.ErrorMessage is not null) return BadRequest(result.ErrorMessage);
            return CreatedAtAction(nameof(GetCourseById), new { id = result.Data!.CourseId }, result.Data);
        }

        /// <summary>
        /// Updates an existing course by ID.
        /// </summary>
        /// <param name="id">Course primary key.</param>
        /// <param name="dto">Updated course payload.</param>
        /// <returns>No content when update succeeds.</returns>
        [HttpPut("{id:int}")]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Teacher}")]
        [SwaggerOperation(Summary = "Update course", Description = "Updates an existing course record by ID.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateCourse(int id, CourseDTO dto)
        {
            var result = await _courseService.UpdateAsync(id, dto);
            if (result.ErrorMessage is not null) return BadRequest(result.ErrorMessage);
            if (result.NotFound) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Deletes a course by ID.
        /// </summary>
        /// <param name="id">Course primary key.</param>
        /// <returns>No content when delete succeeds.</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        [SwaggerOperation(Summary = "Delete course", Description = "Deletes a course by ID.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteCourse(int id)
        {
            var ok = await _courseService.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
