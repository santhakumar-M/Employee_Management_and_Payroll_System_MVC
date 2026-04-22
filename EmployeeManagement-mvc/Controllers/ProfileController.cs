using EmployeeHrSystem.Models;
using EmployeeHrSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeHrSystem.Controllers
{
    [Authorize(Roles = "Employee")]
    public class ProfileController : Controller
    {
        private readonly IEmployeeService _employeeService;
        private readonly IAttendanceService _attendanceService;
        private readonly IPerformanceService _performanceService;

        public ProfileController(IEmployeeService employeeService, IAttendanceService attendanceService, IPerformanceService performanceService)
        {
            _employeeService = employeeService;
            _attendanceService = attendanceService;
            _performanceService = performanceService;
        }

        // GET: /Profile - Personalized employee page showing attendance summary for a month
        public async Task<IActionResult> Index(string? month)
        {
            // Use current month if not specified
            month ??= DateTime.Now.ToString("yyyy-MM");

            // Get employee id from claims
            var empIdClaim = User.FindFirst("EmployeeId")?.Value;
            if (string.IsNullOrWhiteSpace(empIdClaim) || !int.TryParse(empIdClaim, out var employeeId))
            {
                return Forbid();
            }

            var employee = await _employeeService.GetEmployeeByIdAsync(employeeId);
            if (employee == null) return NotFound();

            var records = await _attendanceService.GetEmployeeAttendanceAsync(employeeId);
            var monthRecords = records.Where(a => a.Date.ToString("yyyy-MM") == month).ToList();

            // Try to use monthly summary if available (contains DaysPresent and TotalWorkingDays)
            var summary = await _attendanceService.GetMonthlyAttendanceSummaryAsync(employeeId, month);

            int present;
            int totalWorkingDays;

            if (summary != null)
            {
                present = summary.DaysPresent;
                totalWorkingDays = summary.TotalWorkingDays;
            }
            else
            {
                present = monthRecords.Count(a => a.Status == "PRESENT");

                // compute working days from month string yyyy-MM
                var parts = month.Split('-');
                if (parts.Length == 2 && int.TryParse(parts[0], out var y) && int.TryParse(parts[1], out var m))
                {
                    totalWorkingDays = GetWorkingDaysInMonth(y, m);
                }
                else
                {
                    totalWorkingDays = monthRecords.Count; // fallback
                }
            }

            // Explicit absent records (marked ABSENT)
            int explicitAbsent = monthRecords.Count(a => a.Status == "ABSENT");
            int leaveDays = monthRecords.Count(a => a.Status == "LEAVE");
            // Implied absent days = working days minus present (days without a present record)
            int impliedAbsent = Math.Max(0, totalWorkingDays - present);

            // Use explicit absent as the primary AbsentDays value in the view model
            int absent = explicitAbsent;

            // Load performance evaluations for this employee
            var evaluations = await _performanceService.GetEmployeeEvaluationsAsync(employeeId);

            var vm = new EmployeeAttendanceSummaryViewModel
            {
                Employee = employee,
                Month = month,
                PresentDays = present,
                AbsentDays = absent,
                LeaveDays = leaveDays,
                Records = monthRecords,
                Evaluations = evaluations
            };

            ViewBag.TotalWorkingDays = totalWorkingDays;
            ViewBag.ImpliedAbsent = impliedAbsent;
            return View(vm);
        }

        // helper moved into the controller as a private static method
        private static int GetWorkingDaysInMonth(int year, int month)
        {
            int daysInMonth = DateTime.DaysInMonth(year, month);
            int workingDays = 0;
            for (int day = 1; day <= daysInMonth; day++)
            {
                var dow = new DateOnly(year, month, day).DayOfWeek;
                if (dow != DayOfWeek.Saturday && dow != DayOfWeek.Sunday)
                    workingDays++;
            }
            return workingDays;
        }
    }
}
