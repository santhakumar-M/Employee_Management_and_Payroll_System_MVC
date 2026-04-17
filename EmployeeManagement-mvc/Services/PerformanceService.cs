using EmployeeHrSystem.Models;
using Microsoft.EntityFrameworkCore;

namespace EmployeeHrSystem.Services
{
    public class PerformanceService : IPerformanceService
    {
        private readonly ApplicationContext _context;

        public PerformanceService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<Evaluation>> GetAllEvaluationsAsync()
        {
            return await _context.Evaluations
                .Include(e => e.Employee)
                .OrderByDescending(e => e.EvaluationDate)
                .ToListAsync();
        }

        public async Task<Evaluation?> GetEvaluationByIdAsync(int id)
        {
            return await _context.Evaluations
                .Include(e => e.Employee)
                .FirstOrDefaultAsync(e => e.EvaluationId == id);
        }

        public async Task<List<Evaluation>> GetEmployeeEvaluationsAsync(int employeeId)
        {
            return await _context.Evaluations
                .Include(e => e.Employee)
                .Where(e => e.EmployeeId == employeeId)
                .OrderByDescending(e => e.EvaluationDate)
                .ToListAsync();
        }

        public async Task<bool> CreateEvaluationAsync(Evaluation evaluation)
        {
            try
            {
                // Validate employee exists
                if (!await _context.Employees.AnyAsync(e => e.Id == evaluation.EmployeeId))
                    return false;

                // Validate score range (0-100)
                if (evaluation.Score < 0 || evaluation.Score > 100)
                    return false;

                _context.Evaluations.Add(evaluation);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateEvaluationAsync(Evaluation evaluation)
        {
            try
            {
                // Validate score range
                if (evaluation.Score < 0 || evaluation.Score > 100)
                    return false;

                _context.Entry(evaluation).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteEvaluationAsync(int id)
        {
            try
            {
                var evaluation = await _context.Evaluations.FindAsync(id);
                if (evaluation == null) return false;

                _context.Evaluations.Remove(evaluation);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<decimal> GetEmployeeAverageScoreAsync(int employeeId)
        {
            var evaluations = await _context.Evaluations
                .Where(e => e.EmployeeId == employeeId)
                .ToListAsync();

            if (evaluations.Count == 0) return 0;

            return evaluations.Average(e => e.Score);
        }
    }
}
