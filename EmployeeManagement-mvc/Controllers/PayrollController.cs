using EmployeeHrSystem.Models;
using EmployeeHrSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace EmployeeHrSystem.Controllers
{
    [Authorize(Roles = "Admin,Payroll Officer")]
    public class PayrollController : Controller
    {
        private readonly IPayrollService _payrollService;
        private readonly IEmployeeService _employeeService;

        public PayrollController(IPayrollService payrollService, IEmployeeService employeeService)
        {
            _payrollService = payrollService;
            _employeeService = employeeService;
        }

        // GET: /Payroll
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            ViewBag.EmployeeItems = new SelectList(employees, "Id", "Name");

            var list = await _payrollService.GetAllPayrollsAsync();
            return View(list);
        }

        // POST: /Payroll/Process
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Process([FromForm] int employeeId, [FromForm] string month)
        {
            if (string.IsNullOrWhiteSpace(month))
                ModelState.AddModelError("month", "Month is required (YYYY-MM).");

            if (!ModelState.IsValid)
            {
                var employees = await _employeeService.GetAllEmployeesAsync();
                ViewBag.EmployeeItems = new SelectList(employees, "Id", "Name");
                var list = await _payrollService.GetAllPayrollsAsync();
                return View("Index", list);
            }

            var p = new Payroll
            {
                EmployeeId = employeeId,
                Month = month.Trim(),
                BasicSalary = 0, // Service will populate from employee
                Deductions = 0,
                NetSalary = 0,
                PaymentStatus = "PENDING"
            };

            var success = await _payrollService.ProcessPayrollAsync(p);
            if (!success)
            {
                ModelState.AddModelError("", "Error processing payroll. Employee may not exist.");
                var employees = await _employeeService.GetAllEmployeesAsync();
                ViewBag.EmployeeItems = new SelectList(employees, "Id", "Name");
                var list = await _payrollService.GetAllPayrollsAsync();
                return View("Index", list);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}