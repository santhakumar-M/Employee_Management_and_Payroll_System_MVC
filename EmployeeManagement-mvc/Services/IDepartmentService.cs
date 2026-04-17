using EmployeeHrSystem.Models;

namespace EmployeeHrSystem.Services
{
    public interface IDepartmentService
    {
        Task<List<Department>> GetAllDepartmentsAsync();
        Task<Department?> GetDepartmentByIdAsync(int id);
        Task<bool> CreateDepartmentAsync(Department department);
        Task<bool> UpdateDepartmentAsync(Department department);
        Task<bool> DeleteDepartmentAsync(int id);
    }
}
