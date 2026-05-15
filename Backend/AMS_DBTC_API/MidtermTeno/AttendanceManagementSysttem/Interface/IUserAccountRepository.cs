using MidtermTeno.AttendanceManagementSysttem.Model;

namespace MidtermTeno.AttendanceManagementSysttem.Interface
{
    public interface IUserAccountRepository
    {
        Task<UserAccount?> GetByUsernameAsync(string username);
        Task<UserAccount?> GetByIdAsync(int userId);
        Task<bool> UsernameExistsAsync(string username);
        Task<UserAccount> AddAsync(UserAccount user);
    }
}
