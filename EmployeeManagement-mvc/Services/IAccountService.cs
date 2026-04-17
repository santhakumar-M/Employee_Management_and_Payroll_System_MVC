using EmployeeHrSystem.Models;

namespace EmployeeHrSystem.Services
{
    public interface IAccountService
    {
        Task<AppUser?> AuthenticateAsync(string username, string password, string role);
        Task<bool> CreateUserAsync(AppUser user, string password);
        Task<bool> UpdateUserAsync(AppUser user);
        Task<bool> DeleteUserAsync(int id);
        Task<AppUser?> GetUserByIdAsync(int id);
        Task<AppUser?> GetUserByUsernameAsync(string username);
    }
}
