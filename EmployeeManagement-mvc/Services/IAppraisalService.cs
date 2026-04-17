using EmployeeHrSystem.Models;

namespace EmployeeHrSystem.Services
{
    public interface IAppraisalService
    {
        Task<List<Appraisal>> GetAllAppraisalsAsync();
        Task<Appraisal?> GetAppraisalByIdAsync(int id);
        Task<List<Appraisal>> GetEmployeeAppraisalsAsync(int employeeId);
        Task<bool> CreateAppraisalAsync(Appraisal appraisal);
        Task<bool> UpdateAppraisalAsync(Appraisal appraisal);
        Task<bool> DeleteAppraisalAsync(int id);
        Task<List<Appraisal>> GetAppraisalsByDateRangeAsync(DateOnly startDate, DateOnly endDate);
    }
}
