using EmployeeHrSystem.Models;

namespace EmployeeHrSystem.Services
{
    public interface IPerformanceService
    {
        Task<List<Evaluation>> GetAllEvaluationsAsync();
        Task<Evaluation?> GetEvaluationByIdAsync(int id);
        Task<List<Evaluation>> GetEmployeeEvaluationsAsync(int employeeId);
        Task<bool> CreateEvaluationAsync(Evaluation evaluation);
        Task<bool> UpdateEvaluationAsync(Evaluation evaluation);
        Task<bool> DeleteEvaluationAsync(int id);
        Task<decimal> GetEmployeeAverageScoreAsync(int employeeId);
    }
}
