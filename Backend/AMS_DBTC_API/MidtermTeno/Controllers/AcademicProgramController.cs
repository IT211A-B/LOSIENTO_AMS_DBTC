using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MidtermTeno.AttendanceManagementSysttem.Constants;
using MidtermTeno.AttendanceManagementSysttem.DTOs;
using MidtermTeno.AttendanceManagementSysttem.Interface.ServiceInterface;
using Swashbuckle.AspNetCore.Annotations;

namespace MidtermTeno.Controllers
{
    [ApiController]
    [Route("api/programs")]
    [Authorize]
    public class AcademicProgramController : ControllerBase
    {
        private readonly IAcademicProgramService _service;

        public AcademicProgramController(IAcademicProgramService service) => _service = service;

        [HttpGet]
        [SwaggerOperation(Summary = "Get all academic programs")]
        public async Task<ActionResult<PagedResultDTO<AcademicProgramDTO>>> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var result = await _service.GetAllAsync(pageNumber, pageSize);
            if (result.ErrorMessage is not null) return BadRequest(result.ErrorMessage);
            return Ok(result.Data);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<AcademicProgramDTO>> GetById(int id)
        {
            var item = await _service.GetByIdAsync(id);
            return item is null ? NotFound() : Ok(item);
        }

        [HttpPost]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<ActionResult<AcademicProgramDTO>> Create(AcademicProgramDTO dto)
        {
            var result = await _service.CreateAsync(dto);
            if (result.ErrorMessage is not null) return BadRequest(result.ErrorMessage);
            return CreatedAtAction(nameof(GetById), new { id = result.Data!.ProgramId }, result.Data);
        }

        [HttpPut("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Update(int id, AcademicProgramDTO dto)
        {
            var result = await _service.UpdateAsync(id, dto);
            if (result.ErrorMessage is not null) return BadRequest(result.ErrorMessage);
            if (result.NotFound) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        [Authorize(Roles = AppRoles.Admin)]
        public async Task<IActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            return ok ? NoContent() : NotFound();
        }
    }
}
