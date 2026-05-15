using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MidtermTeno.AttendanceManagementSysttem.Constants;
using MidtermTeno.AttendanceManagementSysttem.DTOs;
using MidtermTeno.AttendanceManagementSysttem.Extensions;
using MidtermTeno.AttendanceManagementSysttem.Interface.ServiceInterface;
using Swashbuckle.AspNetCore.Annotations;

namespace MidtermTeno.Controllers
{
    [ApiController]
    [Route("api/attendance")]
    [Authorize]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _attendanceService;

        public AttendanceController(IAttendanceService attendanceService)
        {
            _attendanceService = attendanceService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get all attendance records", Description = "Returns a paginated list of attendance records.")]
        public async Task<ActionResult<PagedResultDTO<AttendanceRecordDTO>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            int? studentFilter = User.IsInRole(AppRoles.Student) ? User.GetStudentId() : null;
            int? teacherFilter = User.IsInRole(AppRoles.Teacher) ? User.GetTeacherId() : null;

            var result = await _attendanceService.GetAllAsync(pageNumber, pageSize, studentFilter, teacherFilter);
            if (result.ErrorMessage is not null) return BadRequest(result.ErrorMessage);
            return Ok(result.Data);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AttendanceRecordDTO>> GetById(int id)
        {
            var record = await _attendanceService.GetByIdAsync(id);
            if (record is null) return NotFound();

            if (User.IsInRole(AppRoles.Student) && record.StudentId != User.GetStudentId())
                return Forbid();

            return Ok(record);
        }

        [HttpPost]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Teacher}")]
        public async Task<ActionResult<AttendanceRecordDTO>> Create(AttendanceRecordDTO dto)
        {
            var result = await _attendanceService.CreateAsync(dto);
            if (result.ErrorMessage is not null) return BadRequest(result.ErrorMessage);
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.AttendanceRecordId }, result.Data);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Teacher}")]
        public async Task<IActionResult> Update(int id, AttendanceRecordDTO dto)
        {
            var result = await _attendanceService.UpdateAsync(id, dto);
            if (result.ErrorMessage is not null) return BadRequest(result.ErrorMessage);
            if (result.NotFound) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _attendanceService.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }

        [HttpPost("mark")]
        [Authorize(Roles = $"{AppRoles.Admin},{AppRoles.Teacher}")]
        [SwaggerOperation(Summary = "Mark attendance", Description = "Creates or updates attendance for one student, course, and date.")]
        public async Task<ActionResult<AttendanceRecordDTO>> Mark(MarkAttendanceDTO dto)
        {
            int? teacherId = User.IsInRole(AppRoles.Teacher) ? User.GetTeacherId() : null;
            var result = await _attendanceService.MarkAsync(dto, teacherId);
            if (result.ErrorMessage is not null) return BadRequest(result.ErrorMessage);
            if (result.NotFound) return NotFound();

            if (result.Data!.WasCreated)
                return CreatedAtAction(nameof(GetById), new { id = result.Data.Record.AttendanceRecordId }, result.Data.Record);

            return Ok(result.Data.Record);
        }
    }
}
