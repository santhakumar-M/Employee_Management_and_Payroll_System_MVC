using EmployeeHrSystem.Models;

namespace EmployeeHrSystem.Services
{
    public interface IAttendanceService
    {
        Task<List<Attendance>> GetAllAttendanceAsync();
        Task<Attendance?> GetAttendanceByIdAsync(int id);
        Task<bool> MarkAttendanceAsync(Attendance attendance);
        Task<bool> UpdateAttendanceAsync(Attendance attendance);
        Task<bool> DeleteAttendanceAsync(int id);
        Task<List<Attendance>> GetAttendanceByDateRangeAsync(DateOnly startDate, DateOnly endDate);
        Task<List<Attendance>> GetEmployeeAttendanceAsync(int employeeId);

        // Get distinct dates and attendance by date
        Task<List<DateOnly>> GetDistinctAttendanceDatesAsync();
        Task<List<Attendance>> GetAttendanceByDateAsync(DateOnly date);

        // New methods for bulk attendance marking
        Task<List<Employee>> GetAllEmployeesForAttendanceAsync();
        Task<bool> MarkMultipleAttendanceAsync(DateOnly date, List<int> employeeIds);
        Task<bool> MarkBulkAttendanceAsync(DateOnly date, List<EmployeeAttendanceItem> employees);
        Task<MonthlyAttendanceSummary?> GetMonthlyAttendanceSummaryAsync(int employeeId, string month);
        Task<List<MonthlyAttendanceSummary>> GetAllMonthlyAttendanceSummariesAsync(string month);
        // Get all monthly summaries for a specific employee
        Task<List<MonthlyAttendanceSummary>> GetSummariesForEmployeeAsync(int employeeId);
        Task<bool> ResetMonthlyAttendanceAsync(string newMonth);
        Task<decimal> GetAttendancePercentageAsync(int employeeId, string month);
        Task<List<Attendance>> GetAttendanceForDateAsync(DateOnly date);
    }
}

