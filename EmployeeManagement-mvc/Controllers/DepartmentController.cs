using EmployeeHrSystem.Models;
using EmployeeHrSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeHrSystem.Controllers
{
    [Authorize(Roles = "Admin,HR Officer")]
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        // GET: /Department
        public async Task<IActionResult> Index()
        {
            var list = await _departmentService.GetAllDepartmentsAsync();
            return View(list);
        }

        // GET: /Department/Create
        [HttpGet]
        public IActionResult Create() => View();

        // POST: /Department/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Department dept)
        {
            if (!ModelState.IsValid) return View(dept);

            var result = await _departmentService.CreateDepartmentAsync(dept);
            if (!result)
            {
                ModelState.AddModelError("", "Error creating department.");
                return View(dept);
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
