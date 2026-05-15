using Microsoft.EntityFrameworkCore;
using MidtermTeno.AttendanceManagementSysttem.Interface;
using MidtermTeno.AttendanceManagementSysttem.Model;

namespace MidtermTeno.AttendanceManagementSysttem.Repository
{
    public class UserAccountRepository : IUserAccountRepository
    {
        private readonly DatabaseLibrary _db;

        public UserAccountRepository(DatabaseLibrary db) => _db = db;

        public async Task<UserAccount?> GetByUsernameAsync(string username) =>
            await _db.UserAccounts.AsNoTracking().FirstOrDefaultAsync(u => u.Username == username);

        public async Task<UserAccount?> GetByIdAsync(int userId) =>
            await _db.UserAccounts.FindAsync(userId);

        public async Task<bool> UsernameExistsAsync(string username) =>
            await _db.UserAccounts.AnyAsync(u => u.Username == username);

        public async Task<UserAccount> AddAsync(UserAccount user)
        {
            _db.UserAccounts.Add(user);
            await _db.SaveChangesAsync();
            return user;
        }
    }
}
