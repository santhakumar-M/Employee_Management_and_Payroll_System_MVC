using EmployeeHrSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace EmployeeHrSystem.Services
{
    public class AccountService : IAccountService
    {
        private readonly ApplicationContext _context;
        private readonly IPasswordHasher<AppUser> _passwordHasher;

        public AccountService(ApplicationContext context, IPasswordHasher<AppUser> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<AppUser?> AuthenticateAsync(string username, string password, string role)
        {
            try
            {
                // Find user by username and role
                var user = await _context.Users
                    .FirstOrDefaultAsync(u => u.Username == username.Trim() && u.Role == role);

                if (user == null)
                    return null;

                // Verify password
                var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);
                
                return result == PasswordVerificationResult.Success ? user : null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> CreateUserAsync(AppUser user, string password)
        {
            try
            {
                // Check if user already exists
                if (await _context.Users.AnyAsync(u => u.Username == user.Username))
                    return false;

                // Hash password
                user.PasswordHash = _passwordHasher.HashPassword(user, password);

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateUserAsync(AppUser user)
        {
            try
            {
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteUserAsync(int id)
        {
            try
            {
                var user = await _context.Users.FindAsync(id);
                if (user == null) return false;

                _context.Users.Remove(user);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<AppUser?> GetUserByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<AppUser?> GetUserByUsernameAsync(string username)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        }
    }
}
