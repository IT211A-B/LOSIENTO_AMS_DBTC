using MidtermTeno.AttendanceManagementSysttem.DTOs;
using MidtermTeno.AttendanceManagementSysttem.Interface;
using MidtermTeno.AttendanceManagementSysttem.Interface.ServiceInterface;
using MidtermTeno.AttendanceManagementSysttem.Model;

namespace MidtermTeno.AttendanceManagementSysttem.Service
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _repo;

        public DepartmentService(IDepartmentRepository repo) => _repo = repo;

        public async Task<ServiceResult<PagedResultDTO<DepartmentDTO>>> GetAllAsync(int pageNumber, int pageSize)
        {
            if (pageNumber <= 0 || pageSize <= 0)
                return ServiceResult<PagedResultDTO<DepartmentDTO>>.ValidationError("pageNumber and pageSize must be greater than 0.");

            var items = await _repo.GetAllAsync();
            return ServiceResult<PagedResultDTO<DepartmentDTO>>.Ok(Page(items.Select(ToDto).ToList(), pageNumber, pageSize, items.Count));
        }

        public async Task<DepartmentDTO?> GetByIdAsync(int id)
        {
            var entity = await _repo.GetByIdAsync(id);
            return entity is null ? null : ToDto(entity);
        }

        public async Task<ServiceResult<DepartmentDTO>> CreateAsync(DepartmentDTO dto)
        {
            var error = Validate(dto);
            if (error is not null) return ServiceResult<DepartmentDTO>.ValidationError(error);

            var now = DateTime.UtcNow;
            var created = await _repo.AddAsync(new Department
            {
                DepartmentCode = dto.DepartmentCode.Trim(),
                DepartmentName = dto.DepartmentName.Trim(),
                CreatedAt = now,
                LastUpdatedAt = now
            });
            return ServiceResult<DepartmentDTO>.Ok(ToDto(created));
        }

        public async Task<ServiceResult<bool>> UpdateAsync(int id, DepartmentDTO dto)
        {
            var error = Validate(dto);
            if (error is not null) return ServiceResult<bool>.ValidationError(error);

            var existing = await _repo.GetByIdAsync(id);
            if (existing is null) return ServiceResult<bool>.NotFoundResult();

            existing.DepartmentCode = dto.DepartmentCode.Trim();
            existing.DepartmentName = dto.DepartmentName.Trim();
            existing.LastUpdatedAt = DateTime.UtcNow;

            var ok = await _repo.UpdateAsync(existing);
            return ok ? ServiceResult<bool>.Ok(true) : ServiceResult<bool>.NotFoundResult();
        }

        public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);

        private static string? Validate(DepartmentDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.DepartmentCode)) return "DepartmentCode is required.";
            if (string.IsNullOrWhiteSpace(dto.DepartmentName)) return "DepartmentName is required.";
            return null;
        }

        private static DepartmentDTO ToDto(Department m) => new()
        {
            DepartmentId = m.DepartmentId,
            DepartmentCode = m.DepartmentCode,
            DepartmentName = m.DepartmentName,
            CreatedAt = m.CreatedAt,
            LastUpdatedAt = m.LastUpdatedAt
        };

        private static PagedResultDTO<T> Page<T>(List<T> all, int pageNumber, int pageSize, int total) => new()
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalCount = total,
            TotalPages = (int)Math.Ceiling(total / (double)pageSize),
            Items = all.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList()
        };
    }
}
