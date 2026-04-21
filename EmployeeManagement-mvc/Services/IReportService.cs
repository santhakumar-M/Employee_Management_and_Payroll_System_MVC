using EmployeeHrSystem.Models;

namespace EmployeeHrSystem.Services
{
    public interface IReportService
    {
        Task<List<HRReport>> GetAllReportsAsync();
        Task<HRReport?> GetReportByIdAsync(int id);
        Task<bool> CreateReportAsync(HRReport report);
        Task<bool> UpdateReportAsync(HRReport report);
        Task<bool> DeleteReportAsync(int id);
        Task<HRReportViewModel> GenerateMonthlyReportAsync(DateOnly reportDate);
    }
}
