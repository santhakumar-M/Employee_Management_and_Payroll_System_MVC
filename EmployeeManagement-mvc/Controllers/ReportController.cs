using EmployeeHrSystem.Models;
using EmployeeHrSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeHrSystem.Controllers
{
    [Authorize(Roles = "Admin,HR Officer")]
    public class ReportController : Controller
    {
        private readonly IReportService _reportService;
        private readonly IDashboardService _dashboardService;

        public ReportController(IReportService reportService, IDashboardService dashboardService)
        {
            _reportService = reportService;
            _dashboardService = dashboardService;
        }

        // GET: /Report/HR
        public async Task<IActionResult> HR()
        {
            var today = DateOnly.FromDateTime(DateTime.Today);

            // Generate monthly report using service
            var reportVm = await _reportService.GenerateMonthlyReportAsync(today);

            return View(reportVm);
        }

        // GET: /Report/ExportCsv?year=2026&month=03
        [HttpGet]
        public async Task<IActionResult> ExportCsv(int? year, int? month)
        {
            var reportDate = (year.HasValue && month.HasValue)
                ? new DateOnly(year.Value, month.Value, 1)
                : DateOnly.FromDateTime(DateTime.Today);

            var report = await _reportService.GenerateMonthlyReportAsync(reportDate);

            var sb = new System.Text.StringBuilder();
            sb.AppendLine("Department,EmployeeCount,PayrollCost");
            foreach (var d in report.DepartmentBreakdown)
            {
                // Escape double quotes in department name
                var dep = d.DepartmentName?.Replace("\"", "\"\"") ?? string.Empty;
                sb.Append('"').Append(dep).Append('"').Append(',');
                sb.Append(d.EmployeeCount).Append(',');
                sb.AppendLine(d.PayrollCost.ToString(System.Globalization.CultureInfo.InvariantCulture));
            }

            var bytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
            var fileName = $"department_payroll_{report.ReportingPeriod.Replace(' ', '_')}.csv";

            return File(bytes, "text/csv", fileName);
        }
    }
}