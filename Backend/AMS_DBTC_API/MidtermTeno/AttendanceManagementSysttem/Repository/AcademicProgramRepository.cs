using Microsoft.EntityFrameworkCore;
using MidtermTeno.AttendanceManagementSysttem.Interface;
using MidtermTeno.AttendanceManagementSysttem.Model;

namespace MidtermTeno.AttendanceManagementSysttem.Repository
{
    public class AcademicProgramRepository : IAcademicProgramRepository
    {
        private readonly DatabaseLibrary _db;

        public AcademicProgramRepository(DatabaseLibrary db) => _db = db;

        public async Task<List<AcademicProgram>> GetAllAsync() =>
            await _db.Programs.AsNoTracking().Include(p => p.Department).OrderBy(p => p.ProgramName).ToListAsync();

        public async Task<AcademicProgram?> GetByIdAsync(int programId) =>
            await _db.Programs.Include(p => p.Department).FirstOrDefaultAsync(p => p.ProgramId == programId);

        public async Task<bool> ExistsAsync(int programId) =>
            await _db.Programs.AnyAsync(p => p.ProgramId == programId);

        public async Task<AcademicProgram> AddAsync(AcademicProgram program)
        {
            _db.Programs.Add(program);
            await _db.SaveChangesAsync();
            return program;
        }

        public async Task<bool> UpdateAsync(AcademicProgram program)
        {
            if (!await ExistsAsync(program.ProgramId)) return false;
            _db.Programs.Update(program);
            await _db.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int programId)
        {
            var program = await _db.Programs.FindAsync(programId);
            if (program is null) return false;
            _db.Programs.Remove(program);
            await _db.SaveChangesAsync();
            return true;
        }
    }
}
