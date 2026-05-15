using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MidtermTeno.AttendanceManagementSysttem.Constants;
using MidtermTeno.AttendanceManagementSysttem.DTOs;
using MidtermTeno.AttendanceManagementSysttem.Interface.ServiceInterface;
using Swashbuckle.AspNetCore.Annotations;

namespace MidtermTeno.Controllers
{
    /// <summary>
    /// Handles teacher-related endpoints.
    /// </summary>
    [ApiController]
    [Route("api/teachers")]
    [Authorize]
    public class TeacherController : ControllerBase
    {
        private readonly ITeacherService _teacherService;

        public TeacherController(ITeacherService teacherService)
        {
            _teacherService = teacherService;
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
            var result = await _teacherService.GetAllAsync(pageNumber, pageSize);
            if (result.ErrorMessage is not null) return BadRequest(result.ErrorMessage);
            return Ok(result.Data);
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
            var teacher = await _teacherService.GetByIdAsync(id);
            if (teacher is null) return NotFound();
            return Ok(teacher);
        }

        /// <summary>
        /// Creates a new teacher.
        /// </summary>
        /// <param name="dto">Teacher payload from the request body.</param>
        /// <returns>The created teacher record.</returns>
        [HttpPost]
        [Authorize(Roles = AppRoles.Admin)]
        [SwaggerOperation(Summary = "Create teacher", Description = "Creates a new teacher record.")]
        [ProducesResponseType(typeof(TeacherDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<TeacherDTO>> CreateTeacher(TeacherDTO dto)
        {
            var result = await _teacherService.CreateAsync(dto);
            if (result.ErrorMessage is not null) return BadRequest(result.ErrorMessage);
            return CreatedAtAction(nameof(GetTeacherById), new { id = result.Data!.TeacherId }, result.Data);
        }

        /// <summary>
        /// Updates an existing teacher by ID.
        /// </summary>
        /// <param name="id">Teacher primary key.</param>
        /// <param name="dto">Updated teacher payload.</param>
        /// <returns>No content when update succeeds.</returns>
        [HttpPut("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        [SwaggerOperation(Summary = "Update teacher", Description = "Updates an existing teacher record by ID.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTeacher(int id, TeacherDTO dto)
        {
            var result = await _teacherService.UpdateAsync(id, dto);
            if (result.ErrorMessage is not null) return BadRequest(result.ErrorMessage);
            if (result.NotFound) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Deletes a teacher by ID.
        /// </summary>
        /// <param name="id">Teacher primary key.</param>
        /// <returns>No content when delete succeeds.</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        [SwaggerOperation(Summary = "Delete teacher", Description = "Deletes a teacher by ID.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTeacher(int id)
        {
            var ok = await _teacherService.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
