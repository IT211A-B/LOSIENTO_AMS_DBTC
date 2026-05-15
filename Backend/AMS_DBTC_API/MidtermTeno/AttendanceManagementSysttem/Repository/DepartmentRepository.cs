using Microsoft.EntityFrameworkCore;
using MidtermTeno.AttendanceManagementSysttem.Interface;
using MidtermTeno.AttendanceManagementSysttem.Model;

namespace MidtermTeno.AttendanceManagementSysttem.Repository
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly DatabaseLibrary _db;

        public DepartmentRepository(DatabaseLibrary db) => _db = db;

        public async Task<List<Department>> GetAllAsync() =>
            await _db.Departments.AsNoTracking().OrderBy(d => d.DepartmentName).ToListAsync();

        public async Task<Department?> GetByIdAsync(int departmentId) =>
            await _db.Departments.FindAsync(departmentId);

        public async Task<bool> ExistsAsync(int departmentId) =>
            await _db.Departments.AnyAsync(d => d.DepartmentId == departmentId);

        public async Task<Department> AddAsync(Department department)
        {
            _db.Departments.Add(department);
            await _db.SaveChangesAsync();
            return department;
        }

        public async Task<bool> UpdateAsync(Department department)
        {
            if (!await ExistsAsync(department.DepartmentId)) return false;
            _db.Departments.Update(department);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int departmentId)
        {
            var department = await _db.Departments.FindAsync(departmentId);
            if (department is null) return false;
            _db.Departments.Remove(department);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
