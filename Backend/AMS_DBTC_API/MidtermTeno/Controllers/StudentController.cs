using Microsoft.AspNetCore.Mvc;
using MidtermTeno.AttendanceManagementSysttem.DTOs;
using MidtermTeno.AttendanceManagementSysttem.Interface;
using MidtermTeno.AttendanceManagementSysttem.Model;
using Swashbuckle.AspNetCore.Annotations;

namespace MidtermTeno.Controllers
{
    /// <summary>
    /// Handles student-related endpoints.
    /// </summary>
    [ApiController]
    [Route("api/students")]
    public class StudentController : ControllerBase
    {
        private readonly IStudentRepository _studentRepo;

        public StudentController(IStudentRepository studentRepo)
        {
            _studentRepo = studentRepo;
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
            if (pageNumber <= 0 || pageSize <= 0) return BadRequest("pageNumber and pageSize must be greater than 0.");

            var students = await _studentRepo.GetAllAsync();
            var totalCount = students.Count;
            var items = students
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .Select(ToDto)
                .ToList();

            return Ok(new PagedResultDTO<StudentDTO>
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalCount = totalCount,
                TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize),
                Items = items
            });
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
            var student = await _studentRepo.GetByIdAsync(id);
            if (student is null) return NotFound();
            return Ok(ToDto(student));
        }

        /// <summary>
        /// Creates a new student.
        /// </summary>
        /// <param name="dto">Student payload from the request body.</param>
        /// <returns>The created student record.</returns>
        [HttpPost]
        [SwaggerOperation(Summary = "Create student", Description = "Creates a new student record.")]
        [ProducesResponseType(typeof(StudentDTO), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<StudentDTO>> CreateStudent(StudentDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Year_Level))
                return BadRequest("Year_Level is required.");

            var now = DateTime.UtcNow;
            var model = new Student
            {
                ProgramId = dto.ProgramId,
                Year_Level = dto.Year_Level.Trim(),
                CreatedAt = now,
                LastUpdatedAt = now,
                CreatedBy = dto.CreatedBy,
                LastUpdatedBy = dto.LastUpdatedBy
            };

            var created = await _studentRepo.AddAsync(model);

            return CreatedAtAction(
                nameof(GetStudentById),
                new { id = created.StudentId },
                ToDto(created)
            );
        }

        /// <summary>
        /// Updates an existing student by ID.
        /// </summary>
        /// <param name="id">Student primary key.</param>
        /// <param name="dto">Updated student payload.</param>
        /// <returns>No content when update succeeds.</returns>
        [HttpPut("{id:int}")]
        [SwaggerOperation(Summary = "Update student", Description = "Updates an existing student record by ID.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateStudent(int id, StudentDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Year_Level))
                return BadRequest("Year_Level is required.");

            var existing = await _studentRepo.GetByIdAsync(id);
            if (existing is null) return NotFound();

            existing.ProgramId = dto.ProgramId;
            existing.Year_Level = dto.Year_Level.Trim();
            existing.LastUpdatedAt = DateTime.UtcNow;

            if (dto.CreatedBy is not null) existing.CreatedBy = dto.CreatedBy;
            if (dto.LastUpdatedBy is not null) existing.LastUpdatedBy = dto.LastUpdatedBy;

            var ok = await _studentRepo.UpdateAsync(existing);
            return ok ? NoContent() : NotFound();
        }

        /// <summary>
        /// Deletes a student by ID.
        /// </summary>
        /// <param name="id">Student primary key.</param>
        /// <returns>No content when delete succeeds.</returns>
        [HttpDelete("{id:int}")]
        [SwaggerOperation(Summary = "Delete student", Description = "Deletes a student by ID.")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteStudent(int id)
        {
            var ok = await _studentRepo.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }

        private static StudentDTO ToDto(Student model)
        {
            return new StudentDTO
            {
                StudentId = model.StudentId,
                ProgramId = model.ProgramId,
                Year_Level = model.Year_Level,
                CreatedAt = model.CreatedAt,
                LastUpdatedAt = model.LastUpdatedAt,
                CreatedBy = model.CreatedBy,
                LastUpdatedBy = model.LastUpdatedBy
            };
        }
    }
}
