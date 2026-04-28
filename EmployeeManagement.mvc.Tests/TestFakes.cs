using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using EmployeeHrSystem.Models;
using EmployeeHrSystem.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace EmployeeManagement.mvc.Tests
{
    // Shared fakes and helpers for controller tests
    internal class FakeAccountService : IAccountService
    {
        public Task<AppUser?> AuthenticateAsync(string username, string password, string role)
        {
            if (username == "valid" && password == "pass" && !string.IsNullOrWhiteSpace(role))
                return Task.FromResult<AppUser?>(new AppUser { Username = username, Role = role, EmployeeId = 1 });
            return Task.FromResult<AppUser?>(null);
        }

        public Task<bool> CreateUserAsync(AppUser user, string password) => Task.FromResult(true);
        public Task<bool> UpdateUserAsync(AppUser user) => Task.FromResult(true);
        public Task<bool> DeleteUserAsync(int id) => Task.FromResult(true);
        public Task<AppUser?> GetUserByIdAsync(int id) => Task.FromResult<AppUser?>(null);
        public Task<AppUser?> GetUserByUsernameAsync(string username) => Task.FromResult<AppUser?>(null);
    }

    internal class FakeEmployeeService : IEmployeeService
    {
        public Task<List<Employee>> GetAllEmployeesAsync() => Task.FromResult(new List<Employee> { new Employee { Id = 1, Name = "John", Designation = "Dev" } });
        public Task<Employee?> GetEmployeeByIdAsync(int id) => Task.FromResult(id == 1 ? new Employee { Id = 1, Name = "John" } : null as Employee);
        public Task<bool> CreateEmployeeAsync(Employee employee) => Task.FromResult(true);
        public Task<bool> UpdateEmployeeAsync(Employee employee) => Task.FromResult(true);
        public Task<bool> DeleteEmployeeAsync(int id) => Task.FromResult(id == 1);
    }

    internal class FakeDepartmentService : IDepartmentService
    {
        public Task<List<Department>> GetAllDepartmentsAsync() => Task.FromResult(new List<Department>());
        public Task<bool> CreateDepartmentAsync(Department dept) => Task.FromResult(true);
        public Task<Department?> GetDepartmentByIdAsync(int id) => Task.FromResult<Department?>(null);
        public Task<bool> UpdateDepartmentAsync(Department department) => Task.FromResult(true);
        public Task<bool> DeleteDepartmentAsync(int id) => Task.FromResult(true);
    }

    internal class FakeAttendanceService : IAttendanceService
    {
        public Task<List<Attendance>> GetAllAttendanceAsync() => Task.FromResult(new List<Attendance>());
        public Task<Attendance?> GetAttendanceByIdAsync(int id) => Task.FromResult<Attendance?>(null);
        public Task<bool> MarkAttendanceAsync(Attendance attendance) => Task.FromResult(true);
        public Task<bool> UpdateAttendanceAsync(Attendance attendance) => Task.FromResult(true);
        public Task<bool> DeleteAttendanceAsync(int id) => Task.FromResult(true);
        public Task<List<Attendance>> GetAttendanceByDateRangeAsync(DateOnly startDate, DateOnly endDate) => Task.FromResult(new List<Attendance>());
        public Task<List<Attendance>> GetEmployeeAttendanceAsync(int employeeId)
            => Task.FromResult(new List<Attendance> { new Attendance { Date = DateOnly.FromDateTime(DateTime.Today), Status = "PRESENT", EmployeeId = employeeId } });

        public Task<List<DateOnly>> GetDistinctAttendanceDatesAsync() => Task.FromResult(new List<DateOnly> { DateOnly.FromDateTime(DateTime.Today) });
        public Task<List<Attendance>> GetAttendanceByDateAsync(DateOnly date) => Task.FromResult(new List<Attendance>());
        public Task<List<Employee>> GetAllEmployeesForAttendanceAsync() => Task.FromResult(new List<Employee> { new Employee { Id = 1, Name = "John", Designation = "Dev" } });
        public Task<bool> MarkMultipleAttendanceAsync(DateOnly date, List<int> employeeIds) => Task.FromResult(true);
        public Task<bool> MarkBulkAttendanceAsync(DateOnly date, List<EmployeeAttendanceItem> employees) => Task.FromResult(true);
        public Task<MonthlyAttendanceSummary?> GetMonthlyAttendanceSummaryAsync(int employeeId, string month) => Task.FromResult<MonthlyAttendanceSummary?>(null);
        public Task<List<MonthlyAttendanceSummary>> GetAllMonthlyAttendanceSummariesAsync(string month) => Task.FromResult(new List<MonthlyAttendanceSummary>());
        public Task<List<MonthlyAttendanceSummary>> GetSummariesForEmployeeAsync(int employeeId) => Task.FromResult(new List<MonthlyAttendanceSummary>());
        public Task<bool> ResetMonthlyAttendanceAsync(string newMonth) => Task.FromResult(true);
        public Task<decimal> GetAttendancePercentageAsync(int employeeId, string month) => Task.FromResult(100m);
        public Task<List<Attendance>> GetAttendanceForDateAsync(DateOnly date) => Task.FromResult(new List<Attendance>());
    }

    internal class FakeLeaveService : ILeaveService
    {
        public Task<List<LeaveRequest>> GetAllLeaveRequestsAsync() => Task.FromResult(new List<LeaveRequest>());
        public Task<LeaveRequest?> GetLeaveRequestByIdAsync(int id) => Task.FromResult<LeaveRequest?>(null);
        public Task<List<LeaveRequest>> GetEmployeeLeaveRequestsAsync(int employeeId) => Task.FromResult(new List<LeaveRequest>());
        public Task<bool> ApplyLeaveAsync(LeaveRequest leaveRequest) => Task.FromResult(true);
        public Task<bool> UpdateLeaveStatusAsync(int leaveId, string status) => Task.FromResult(true);
        public Task<bool> DeleteLeaveRequestAsync(int id) => Task.FromResult(true);
        public Task<List<LeaveRequest>> GetLeaveRequestsByStatusAsync(string status) => Task.FromResult(new List<LeaveRequest>());
    }

    internal class FakePayrollService : IPayrollService
    {
        public Task<List<Payroll>> GetAllPayrollsAsync() => Task.FromResult(new List<Payroll>());
        public Task<Payroll?> GetPayrollByIdAsync(int id) => Task.FromResult<Payroll?>(null);
        public Task<List<Payroll>> GetEmployeePayrollsAsync(int employeeId) => Task.FromResult(new List<Payroll>());
        public Task<bool> ProcessPayrollAsync(Payroll payroll, bool applyLeaveDeduction = false, decimal perDayDeduction = 0m) => Task.FromResult(true);
        public Task<bool> UpdatePayrollAsync(Payroll payroll) => Task.FromResult(true);
        public Task<bool> DeletePayrollAsync(int id) => Task.FromResult(true);
        public Task<List<Payroll>> GetPayrollsByMonthAsync(string month) => Task.FromResult(new List<Payroll>());
        public Task<bool> UpdatePaymentStatusAsync(int payrollId, string status) => Task.FromResult(true);
    }

    internal class FakePerformanceService : IPerformanceService
    {
        public Task<List<Evaluation>> GetAllEvaluationsAsync() => Task.FromResult(new List<Evaluation>());
        public Task<Evaluation?> GetEvaluationByIdAsync(int id) => Task.FromResult<Evaluation?>(null);
        public Task<List<Evaluation>> GetEmployeeEvaluationsAsync(int employeeId) => Task.FromResult(new List<Evaluation>());
        public Task<bool> CreateEvaluationAsync(Evaluation evaluation) => Task.FromResult(true);
        public Task<bool> UpdateEvaluationAsync(Evaluation evaluation) => Task.FromResult(true);
        public Task<bool> DeleteEvaluationAsync(int id) => Task.FromResult(true);
        public Task<decimal> GetEmployeeAverageScoreAsync(int employeeId) => Task.FromResult(0m);
    }

    internal class FakeAppraisalService : IAppraisalService
    {
        public Task<List<Appraisal>> GetAllAppraisalsAsync() => Task.FromResult(new List<Appraisal>());
        public Task<Appraisal?> GetAppraisalByIdAsync(int id) => Task.FromResult<Appraisal?>(null);
        public Task<List<Appraisal>> GetEmployeeAppraisalsAsync(int employeeId) => Task.FromResult(new List<Appraisal>());
        public Task<bool> CreateAppraisalAsync(Appraisal appraisal) => Task.FromResult(true);
        public Task<bool> UpdateAppraisalAsync(Appraisal appraisal) => Task.FromResult(true);
        public Task<bool> DeleteAppraisalAsync(int id) => Task.FromResult(true);
        public Task<List<Appraisal>> GetAppraisalsByDateRangeAsync(DateOnly startDate, DateOnly endDate) => Task.FromResult(new List<Appraisal>());
    }

    internal class FakeReportService : IReportService
    {
        public Task<List<HRReport>> GetAllReportsAsync() => Task.FromResult(new List<HRReport>());
        public Task<HRReport?> GetReportByIdAsync(int id) => Task.FromResult<HRReport?>(null);
        public Task<bool> CreateReportAsync(HRReport report) => Task.FromResult(true);
        public Task<bool> UpdateReportAsync(HRReport report) => Task.FromResult(true);
        public Task<bool> DeleteReportAsync(int id) => Task.FromResult(true);
        public Task<HRReportViewModel> GenerateMonthlyReportAsync(DateOnly reportDate)
        {
            var vm = new HRReportViewModel
            {
                ReportingPeriod = reportDate.ToString("MMMM yyyy"),
                DepartmentBreakdown = new List<(string DepartmentName, int EmployeeCount, decimal PayrollCost)> { ("HR", 1, 1000m) }
            };
            return Task.FromResult(vm);
        }
    }

    internal class FakeDashboardService : IDashboardService
    {
        public Task<int> GetTotalEmployeesAsync() => Task.FromResult(10);
        public Task<int> GetTotalDepartmentsAsync() => Task.FromResult(1);
        public Task<decimal> GetAverageAttendanceAsync() => Task.FromResult(95m);
        public Task<int> GetPendingLeaveRequestsAsync() => Task.FromResult(0);
        public Task<decimal> GetTotalPayrollAmountAsync(string month) => Task.FromResult(10000m);
    }

    internal class FakeAuthenticationService : IAuthenticationService
    {
        public Task<AuthenticateResult> AuthenticateAsync(HttpContext context, string scheme) => Task.FromResult(AuthenticateResult.NoResult());
        public Task ChallengeAsync(HttpContext context, string scheme, AuthenticationProperties properties) => Task.CompletedTask;
        public Task ForbidAsync(HttpContext context, string scheme, AuthenticationProperties properties) => Task.CompletedTask;
        public Task SignInAsync(HttpContext context, string scheme, ClaimsPrincipal principal, AuthenticationProperties properties) => Task.CompletedTask;
        public Task SignOutAsync(HttpContext context, string scheme, AuthenticationProperties properties) => Task.CompletedTask;
    }

    // Minimal in-memory TempData provider and factory for tests
    internal class FakeTempDataProvider : ITempDataProvider
    {
        public IDictionary<string, object> LoadTempData(HttpContext context) => new Dictionary<string, object>();
        public void SaveTempData(HttpContext context, IDictionary<string, object> values) { }
    }

    internal class FakeTempDataDictionaryFactory : ITempDataDictionaryFactory
    {
        private readonly ITempDataProvider _provider = new FakeTempDataProvider();
        public ITempDataDictionary GetTempData(HttpContext context)
        {
            return new TempDataDictionary(context, _provider);
        }
    }

    // Minimal UrlHelper and factory
    internal class FakeUrlHelper : IUrlHelper
    {
        public ActionContext ActionContext { get; set; }
        public string? Action(UrlActionContext values) => string.Empty;
        public string Content(string contentPath) => contentPath ?? string.Empty;
        public bool IsLocalUrl(string url) => true;
        public string? Link(string routeName, object values) => string.Empty;
        public string? RouteUrl(UrlRouteContext values) => string.Empty;
    }

    internal class FakeUrlHelperFactory : IUrlHelperFactory
    {
        public IUrlHelper GetUrlHelper(ActionContext context) => new FakeUrlHelper { ActionContext = context };
    }

    internal static class TestHelper
    {
        public static IServiceProvider BuildServices() => new SimpleServiceProvider();

        private class SimpleServiceProvider : IServiceProvider
        {
            public object? GetService(Type serviceType)
            {
                if (serviceType == typeof(IAuthenticationService)) return new FakeAuthenticationService();
                if (serviceType == typeof(IAttendanceService)) return new FakeAttendanceService();
                if (serviceType == typeof(ITempDataDictionaryFactory)) return new FakeTempDataDictionaryFactory();
                if (serviceType == typeof(IUrlHelperFactory)) return new FakeUrlHelperFactory();
                return null;
            }
        }

        public static Microsoft.AspNetCore.Mvc.ControllerContext CreateContext(ClaimsPrincipal? user = null)
        {
            var ctx = new DefaultHttpContext();
            ctx.RequestServices = BuildServices();
            if (user != null) ctx.User = user;
            return new Microsoft.AspNetCore.Mvc.ControllerContext { HttpContext = ctx };
        }
    }
}
