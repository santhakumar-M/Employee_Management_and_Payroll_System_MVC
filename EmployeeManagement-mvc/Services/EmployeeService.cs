using EmployeeHrSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeHrSystem.Services
{
    public class EmployeeService : IEmployeeService
    {
        private readonly ApplicationContext _context;

        public EmployeeService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<Employee>> GetAllEmployeesAsync()
        {
            return await _context.Employees.Include(e => e.Department).ToListAsync();
        }

        public async Task<Employee?> GetEmployeeByIdAsync(int id)
        {
            return await _context.Employees.Include(e => e.Department).FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<bool> CreateEmployeeAsync(Employee employee)
        {
            try
            {
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateEmployeeAsync(Employee employee)
        {
            try
            {
                _context.Entry(employee).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteEmployeeAsync(int id)
        {
            try
            {
                var employee = await _context.Employees.FindAsync(id);
                if (employee == null) return false;

                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
