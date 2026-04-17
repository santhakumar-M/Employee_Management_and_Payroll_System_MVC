using EmployeeHrSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace EmployeeHrSystem.Services
{
    public class AttendanceService : IAttendanceService
    {
        private readonly ApplicationContext _context;

        public AttendanceService(ApplicationContext context)
        {
            _context = context;
        }

        public async Task<List<Attendance>> GetAllAttendanceAsync()
        {
            return await _context.Attendances
                .Include(a => a.Employee)
                .OrderByDescending(a => a.AttendanceId)
                .ToListAsync();
        }

        public async Task<Attendance?> GetAttendanceByIdAsync(int id)
        {
            return await _context.Attendances
                .Include(a => a.Employee)
                .FirstOrDefaultAsync(a => a.AttendanceId == id);
        }

        public async Task<bool> MarkAttendanceAsync(Attendance attendance)
        {
            try
            {
                // Validate employee exists
                if (!await _context.Employees.AnyAsync(e => e.Id == attendance.EmployeeId))
                    return false;

                _context.Attendances.Add(attendance);
                await _context.SaveChangesAsync();

                // Update monthly attendance summary
                await UpdateMonthlyAttendanceSummaryAsync(attendance.EmployeeId, attendance.Date);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> UpdateAttendanceAsync(Attendance attendance)
        {
            try
            {
                _context.Entry(attendance).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> DeleteAttendanceAsync(int id)
        {
            try
            {
                var attendance = await _context.Attendances.FindAsync(id);
                if (attendance == null) return false;

                _context.Attendances.Remove(attendance);
                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<List<Attendance>> GetAttendanceByDateRangeAsync(DateOnly startDate, DateOnly endDate)
        {
            return await _context.Attendances
                .Include(a => a.Employee)
                .Where(a => a.Date >= startDate && a.Date <= endDate)
                .OrderByDescending(a => a.Date)
                .ToListAsync();
        }

        public async Task<List<Attendance>> GetEmployeeAttendanceAsync(int employeeId)
        {
            return await _context.Attendances
                .Include(a => a.Employee)
                .Where(a => a.EmployeeId == employeeId)
                .OrderByDescending(a => a.Date)
                .ToListAsync();
        }

        // ===== NEW BULK ATTENDANCE METHODS =====

        public async Task<List<Employee>> GetAllEmployeesForAttendanceAsync()
        {
            return await _context.Employees
                .OrderBy(e => e.Name)
                .ToListAsync();
        }

        public async Task<List<Attendance>> GetAttendanceForDateAsync(DateOnly date)
        {
            return await _context.Attendances
                .Include(a => a.Employee)
                .Where(a => a.Date == date)
                .ToListAsync();
        }

        public async Task<bool> MarkMultipleAttendanceAsync(DateOnly date, List<int> employeeIds)
        {
            try
            {
                string currentMonth = date.ToString("yyyy-MM");

                // Remove any existing attendance for this date
                var existingAttendance = await _context.Attendances
                    .Where(a => a.Date == date)
                    .ToListAsync();

                _context.Attendances.RemoveRange(existingAttendance);
                await _context.SaveChangesAsync();

                // Add new attendance records for selected employees
                foreach (var employeeId in employeeIds)
                {
                    // Verify employee exists
                    if (!await _context.Employees.AnyAsync(e => e.Id == employeeId))
                        continue;

                    var attendance = new Attendance
                    {
                        EmployeeId = employeeId,
                        Date = date,
                        Status = "PRESENT"
                    };

                    _context.Attendances.Add(attendance);
                }

                await _context.SaveChangesAsync();

                // Update monthly summaries for all marked employees
                foreach (var employeeId in employeeIds)
                {
                    await UpdateMonthlyAttendanceSummaryAsync(employeeId, date);
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        private async Task UpdateMonthlyAttendanceSummaryAsync(int employeeId, DateOnly attendanceDate)
        {
            try
            {
                string currentMonth = attendanceDate.ToString("yyyy-MM");

                // Get or create monthly summary
                var summary = await _context.MonthlyAttendanceSummaries
                    .FirstOrDefaultAsync(s => s.EmployeeId == employeeId && s.Month == currentMonth);

                if (summary == null)
                {
                    summary = new MonthlyAttendanceSummary
                    {
                        EmployeeId = employeeId,
                        Month = currentMonth,
                        DaysPresent = 0,
                        TotalWorkingDays = GetWorkingDaysInMonth(attendanceDate),
                        CreatedDate = DateTime.Now
                    };
                    _context.MonthlyAttendanceSummaries.Add(summary);
                }

                // Count present days for this month
                var presentDays = await _context.Attendances
                    .Where(a => a.EmployeeId == employeeId 
                        && a.Date.Month == attendanceDate.Month 
                        && a.Date.Year == attendanceDate.Year
                        && a.Status == "PRESENT")
                    .CountAsync();

                summary.DaysPresent = presentDays;
                summary.TotalWorkingDays = GetWorkingDaysInMonth(attendanceDate);
                summary.AttendancePercentage = summary.TotalWorkingDays > 0 
                    ? (decimal)summary.DaysPresent / summary.TotalWorkingDays * 100 
                    : 0;

                await _context.SaveChangesAsync();
            }
            catch
            {
                // Log error if needed
            }
        }

        public async Task<MonthlyAttendanceSummary?> GetMonthlyAttendanceSummaryAsync(int employeeId, string month)
        {
            return await _context.MonthlyAttendanceSummaries
                .Include(m => m.Employee)
                .FirstOrDefaultAsync(m => m.EmployeeId == employeeId && m.Month == month);
        }

        public async Task<List<MonthlyAttendanceSummary>> GetAllMonthlyAttendanceSummariesAsync(string month)
        {
            return await _context.MonthlyAttendanceSummaries
                .Include(m => m.Employee)
                .Where(m => m.Month == month)
                .OrderBy(m => m.Employee!.Name)
                .ToListAsync();
        }

        public async Task<decimal> GetAttendancePercentageAsync(int employeeId, string month)
        {
            var summary = await GetMonthlyAttendanceSummaryAsync(employeeId, month);
            return summary?.AttendancePercentage ?? 0;
        }

        public async Task<bool> ResetMonthlyAttendanceAsync(string newMonth)
        {
            try
            {
                // Archive previous month's data (optional)
                var previousMonth = GetPreviousMonth(newMonth);
                var previousSummaries = await _context.MonthlyAttendanceSummaries
                    .Where(m => m.Month == previousMonth)
                    .ToListAsync();

                foreach (var summary in previousSummaries)
                {
                    // Store previous month's data as JSON for record-keeping
                    summary.PreviousMonthData = JsonSerializer.Serialize(new
                    {
                        DaysPresent = summary.DaysPresent,
                        TotalWorkingDays = summary.TotalWorkingDays,
                        AttendancePercentage = summary.AttendancePercentage
                    });

                    _context.MonthlyAttendanceSummaries.Update(summary);
                }

                // Create new summaries for the new month
                var employees = await _context.Employees.ToListAsync();
                foreach (var employee in employees)
                {
                    var existingSummary = await _context.MonthlyAttendanceSummaries
                        .FirstOrDefaultAsync(m => m.EmployeeId == employee.Id && m.Month == newMonth);

                    if (existingSummary == null)
                    {
                        var newSummary = new MonthlyAttendanceSummary
                        {
                            EmployeeId = employee.Id,
                            Month = newMonth,
                            DaysPresent = 0,
                            TotalWorkingDays = GetWorkingDaysInMonth(DateOnly.ParseExact(newMonth + "-01", "yyyy-MM-dd")),
                            CreatedDate = DateTime.Now
                        };
                        _context.MonthlyAttendanceSummaries.Add(newSummary);
                    }
                }

                await _context.SaveChangesAsync();
                return true;
            }
            catch
            {
                return false;
            }
        }

        // ===== HELPER METHODS =====

        private int GetWorkingDaysInMonth(DateOnly date)
        {
            int year = date.Year;
            int month = date.Month;

            // Get total days in month
            int daysInMonth = DateTime.DaysInMonth(year, month);

            // Count weekdays (Monday-Friday)
            int workingDays = 0;
            for (int day = 1; day <= daysInMonth; day++)
            {
                var dayOfWeek = new DateOnly(year, month, day).DayOfWeek;
                if (dayOfWeek != DayOfWeek.Saturday && dayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays++;
                }
            }

            return workingDays;
        }

        private string GetPreviousMonth(string currentMonth)
        {
            // currentMonth format: "2024-01"
            if (DateTime.TryParseExact(currentMonth + "-01", "yyyy-MM-dd", null, System.Globalization.DateTimeStyles.None, out DateTime date))
            {
                var previousDate = date.AddMonths(-1);
                return previousDate.ToString("yyyy-MM");
            }
            return currentMonth;
        }
    }
}

