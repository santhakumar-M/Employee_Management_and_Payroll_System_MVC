using EmployeeHrSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeHrSystem.Services
{
    public class DepartmentService : IDepartmentService
    {
        private readonly ApplicationContext _context;

        public DepartmentService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<Department>> GetAllDepartmentsAsync()
        {
            return await _context.Departments.ToListAsync();
        }

        public async Task<Department?> GetDepartmentByIdAsync(int id)
        {
            return await _context.Departments.FindAsync(id);
        }

        public async Task<bool> CreateDepartmentAsync(Department department)
        {
            try
            {
                _context.Departments.Add(department);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateDepartmentAsync(Department department)
        {
            try
            {
                _context.Entry(department).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteDepartmentAsync(int id)
        {
            try
            {
                var department = await _context.Departments.FindAsync(id);
                if (department == null) return false;

                _context.Departments.Remove(department);
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
