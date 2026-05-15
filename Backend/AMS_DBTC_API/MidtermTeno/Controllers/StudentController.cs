using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MidtermTeno.AttendanceManagementSysttem.Constants;
using MidtermTeno.AttendanceManagementSysttem.DTOs;
using MidtermTeno.AttendanceManagementSysttem.Interface.ServiceInterface;
using Swashbuckle.AspNetCore.Annotations;

namespace MidtermTeno.Controllers
{
    /// <summary>
    /// Handles student-related endpoints.
    /// </summary>
    [ApiController]
    [Route("api/students")]
    [Authorize]
    public class StudentController : ControllerBase
    {
        private readonly IStudentService _studentService;

        public StudentController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        /// <summary>
        /// Gets students using pagination.
        /// </summary>
        /// <param name="pageNumber">Page index starting at 1.</param>
        /// <param name="pageSize">Number of records per page.</param>
        /// <returns>A paged list of students.</returns>
        [HttpGet]
        [SwaggerOperation(Summary = "Get all students", Description = "Returns a paginated list of students.")]
        [ProducesResponseType(typeof(PagedResultDTO<StudentDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<PagedResultDTO<StudentDTO>>> GetAllStudents([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _studentService.GetAllAsync(pageNumber, pageSize);
            if (result.ErrorMessage is not null) return BadRequest(result.ErrorMessage);
            return Ok(result.Data);
        }

        /// <summary>
        /// Gets one student by ID.
        /// </summary>
        /// <param name="id">Student primary key.</param>
        /// <returns>The matching student record.</returns>
        [HttpGet("{id:int}")]
        [SwaggerOperation(Summary = "Get student by ID", Description = "Returns one student by primary key.")]
        [ProducesResponseType(typeof(StudentDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<StudentDTO>> GetStudentById(int id)
        {
            var student = await _studentService.GetByIdAsync(id);
            if (student is null) return NotFound();
            return Ok(student);
        }

        /// <summary>
        /// Creates a new student.
        /// </summary>
        /// <param name="dto">Student payload from the request body.</param>
        /// <returns>The created student record.</returns>
        [HttpPost]
        [Authorize(Roles = AppRoles.Admin)]
        [SwaggerOperation(Summary = "Create student", Description = "Creates a new student record.")]
        [ProducesResponseType(typeof(StudentDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<StudentDTO>> CreateStudent(StudentDTO dto)
        {
            var result = await _studentService.CreateAsync(dto);
            if (result.ErrorMessage is not null) return BadRequest(result.ErrorMessage);
            return CreatedAtAction(nameof(GetStudentById), new { id = result.Data!.StudentId }, result.Data);
        }

        /// <summary>
        /// Updates an existing student by ID.
        /// </summary>
        /// <param name="id">Student primary key.</param>
        /// <param name="dto">Updated student payload.</param>
        /// <returns>No content when update succeeds.</returns>
        [HttpPut("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        [SwaggerOperation(Summary = "Update student", Description = "Updates an existing student record by ID.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStudent(int id, StudentDTO dto)
        {
            var result = await _studentService.UpdateAsync(id, dto);
            if (result.ErrorMessage is not null) return BadRequest(result.ErrorMessage);
            if (result.NotFound) return NotFound();
            return NoContent();
        }

        /// <summary>
        /// Deletes a student by ID.
        /// </summary>
        /// <param name="id">Student primary key.</param>
        /// <returns>No content when delete succeeds.</returns>
        [HttpDelete("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        [SwaggerOperation(Summary = "Delete student", Description = "Deletes a student by ID.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var ok = await _studentService.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
