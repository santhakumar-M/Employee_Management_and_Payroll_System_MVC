# Dependency Injection & Service Layer Documentation

## Overview

The Employee Management MVC application has been refactored to implement a **Service Layer Pattern** with **Dependency Injection**. This architecture separates business logic from controllers, making the code more maintainable, testable, and follows SOLID principles.

---

## Architecture

### Layers

```
┌─────────────────────────────────┐
│      Controllers                │
│  (Handles HTTP requests/responses)
└────────────────┬────────────────┘
                 │ Depends on
┌────────────────▼────────────────┐
│      Services (Interfaces)      │
│  (Business Logic)               │
└────────────────┬────────────────┘
                 │ Implements
┌────────────────▼────────────────┐
│      Services (Implementation)   │
│  (Database Access)              │
└────────────────┬────────────────┘
                 │ Uses
┌────────────────▼────────────────┐
│      ApplicationContext         │
│  (Entity Framework Core)        │
└─────────────────────────────────┘
```

---

## Services Created

### 1. **AccountService** (`IAccountService` / `AccountService`)
**Responsibilities:**
- User authentication
- Password verification using ASP.NET Identity PasswordHasher
- User CRUD operations

**Methods:**
```csharp
Task<AppUser?> AuthenticateAsync(string username, string password, string role);
Task<bool> CreateUserAsync(AppUser user, string password);
Task<bool> UpdateUserAsync(AppUser user);
Task<bool> DeleteUserAsync(int id);
Task<AppUser?> GetUserByIdAsync(int id);
Task<AppUser?> GetUserByUsernameAsync(string username);
```

**Example Usage in AccountController:**
```csharp
var user = await _accountService.AuthenticateAsync(username, password, role);
if (user == null)
{
    ViewBag.Error = "Invalid credentials.";
    return View();
}
```

---

### 2. **EmployeeService** (`IEmployeeService` / `EmployeeService`)
**Responsibilities:**
- Employee CRUD operations
- Employee data retrieval with department information

**Methods:**
```csharp
Task<List<Employee>> GetAllEmployeesAsync();
Task<Employee?> GetEmployeeByIdAsync(int id);
Task<bool> CreateEmployeeAsync(Employee employee);
Task<bool> UpdateEmployeeAsync(Employee employee);
Task<bool> DeleteEmployeeAsync(int id);
```

**Example Usage in EmployeeController:**
```csharp
var employees = await _employeeService.GetAllEmployeesAsync();
var success = await _employeeService.CreateEmployeeAsync(employee);
```

---

### 3. **DepartmentService** (`IDepartmentService` / `DepartmentService`)
**Responsibilities:**
- Department CRUD operations

**Methods:**
```csharp
Task<List<Department>> GetAllDepartmentsAsync();
Task<Department?> GetDepartmentByIdAsync(int id);
Task<bool> CreateDepartmentAsync(Department department);
Task<bool> UpdateDepartmentAsync(Department department);
Task<bool> DeleteDepartmentAsync(int id);
```

---

### 4. **AttendanceService** (`IAttendanceService` / `AttendanceService`)
**Responsibilities:**
- Attendance marking and tracking
- Date range queries and employee-specific attendance

**Methods:**
```csharp
Task<List<Attendance>> GetAllAttendanceAsync();
Task<Attendance?> GetAttendanceByIdAsync(int id);
Task<bool> MarkAttendanceAsync(Attendance attendance);
Task<bool> UpdateAttendanceAsync(Attendance attendance);
Task<bool> DeleteAttendanceAsync(int id);
Task<List<Attendance>> GetAttendanceByDateRangeAsync(DateOnly startDate, DateOnly endDate);
Task<List<Attendance>> GetEmployeeAttendanceAsync(int employeeId);
```

---

### 5. **LeaveService** (`ILeaveService` / `LeaveService`)
**Responsibilities:**
- Leave request management
- Status updates
- Employee-specific leave queries

**Methods:**
```csharp
Task<List<LeaveRequest>> GetAllLeaveRequestsAsync();
Task<LeaveRequest?> GetLeaveRequestByIdAsync(int id);
Task<List<LeaveRequest>> GetEmployeeLeaveRequestsAsync(int employeeId);
Task<bool> ApplyLeaveAsync(LeaveRequest leaveRequest);
Task<bool> UpdateLeaveStatusAsync(int leaveId, string status);
Task<bool> DeleteLeaveRequestAsync(int id);
Task<List<LeaveRequest>> GetLeaveRequestsByStatusAsync(string status);
```

---

### 6. **PayrollService** (`IPayrollService` / `PayrollService`)
**Responsibilities:**
- Payroll processing
- Salary calculations
- Payment status tracking

**Methods:**
```csharp
Task<List<Payroll>> GetAllPayrollsAsync();
Task<Payroll?> GetPayrollByIdAsync(int id);
Task<List<Payroll>> GetEmployeePayrollsAsync(int employeeId);
Task<bool> ProcessPayrollAsync(Payroll payroll);
Task<bool> UpdatePayrollAsync(Payroll payroll);
Task<bool> DeletePayrollAsync(int id);
Task<List<Payroll>> GetPayrollsByMonthAsync(string month);
Task<bool> UpdatePaymentStatusAsync(int payrollId, string status);
```

---

### 7. **PerformanceService** (`IPerformanceService` / `PerformanceService`)
**Responsibilities:**
- Employee evaluation management
- Performance score tracking
- Average score calculations

**Methods:**
```csharp
Task<List<Evaluation>> GetAllEvaluationsAsync();
Task<Evaluation?> GetEvaluationByIdAsync(int id);
Task<List<Evaluation>> GetEmployeeEvaluationsAsync(int employeeId);
Task<bool> CreateEvaluationAsync(Evaluation evaluation);
Task<bool> UpdateEvaluationAsync(Evaluation evaluation);
Task<bool> DeleteEvaluationAsync(int id);
Task<decimal> GetEmployeeAverageScoreAsync(int employeeId);
```

---

### 8. **AppraisalService** (`IAppraisalService` / `AppraisalService`)
**Responsibilities:**
- Appraisal record management
- Salary adjustments

**Methods:**
```csharp
Task<List<Appraisal>> GetAllAppraisalsAsync();
Task<Appraisal?> GetAppraisalByIdAsync(int id);
Task<List<Appraisal>> GetEmployeeAppraisalsAsync(int employeeId);
Task<bool> CreateAppraisalAsync(Appraisal appraisal);
Task<bool> UpdateAppraisalAsync(Appraisal appraisal);
Task<bool> DeleteAppraisalAsync(int id);
Task<List<Appraisal>> GetAppraisalsByDateRangeAsync(DateOnly startDate, DateOnly endDate);
```

---

### 9. **ReportService** (`IReportService` / `ReportService`)
**Responsibilities:**
- HR report generation
- Monthly report compilation

**Methods:**
```csharp
Task<List<HRReport>> GetAllReportsAsync();
Task<HRReport?> GetReportByIdAsync(int id);
Task<bool> CreateReportAsync(HRReport report);
Task<bool> UpdateReportAsync(HRReport report);
Task<bool> DeleteReportAsync(int id);
Task<HRReport> GenerateMonthlyReportAsync(DateOnly reportDate);
```

---

### 10. **DashboardService** (`IDashboardService` / `DashboardService`)
**Responsibilities:**
- Dashboard statistics
- Summary calculations

**Methods:**
```csharp
Task<int> GetTotalEmployeesAsync();
Task<int> GetTotalDepartmentsAsync();
Task<decimal> GetAverageAttendanceAsync();
Task<int> GetPendingLeaveRequestsAsync();
Task<decimal> GetTotalPayrollAmountAsync(string month);
```

---

## Dependency Injection Registration

All services are registered in `Program.cs`:

```csharp
// Register Services for Dependency Injection
builder.Services.AddScoped<IAccountService, AccountService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IAttendanceService, AttendanceService>();
builder.Services.AddScoped<ILeaveService, LeaveService>();
builder.Services.AddScoped<IPayrollService, PayrollService>();
builder.Services.AddScoped<IPerformanceService, PerformanceService>();
builder.Services.AddScoped<IAppraisalService, AppraisalService>();
builder.Services.AddScoped<IReportService, ReportService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
```

**Service Lifetime:**
- **Scoped**: Services are created per HTTP request, ideal for DbContext usage

---

## How to Update Controllers

### Before (Direct Database Access)
```csharp
public class EmployeeController : Controller
{
    private readonly ApplicationContext _ctx;
    
    public EmployeeController(ApplicationContext ctx) => _ctx = ctx;
    
    public IActionResult Index()
    {
        var list = _ctx.Employees.Include(e => e.Department).ToList();
        return View(list);
    }
}
```

### After (Using Services)
```csharp
public class EmployeeController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly IDepartmentService _departmentService;
    
    public EmployeeController(IEmployeeService employeeService, IDepartmentService departmentService)
    {
        _employeeService = employeeService;
        _departmentService = departmentService;
    }
    
    public async Task<IActionResult> Index()
    {
        var list = await _employeeService.GetAllEmployeesAsync();
        return View(list);
    }
}
```

---

## Benefits of This Architecture

### 1. **Separation of Concerns**
- Controllers focus on HTTP handling
- Services handle business logic
- DbContext isolated from controllers

### 2. **Testability**
- Easy to create mock services for unit tests
- Controllers can be tested without database

```csharp
[Test]
public async Task Index_ReturnsAllEmployees()
{
    // Arrange
    var mockService = new Mock<IEmployeeService>();
    var employees = new List<Employee> { /* test data */ };
    mockService.Setup(s => s.GetAllEmployeesAsync())
        .ReturnsAsync(employees);
    
    var controller = new EmployeeController(mockService.Object, /**/);
    
    // Act
    var result = await controller.Index() as ViewResult;
    
    // Assert
    Assert.That(result.Model, Is.EqualTo(employees));
}
```

### 3. **Reusability**
- Services can be used by multiple controllers
- Business logic centralized and consistent

### 4. **Maintainability**
- Changes to database logic only affect services
- Controllers don't need database knowledge
- Error handling centralized

### 5. **Scalability**
- Easy to add caching to services
- Simple to implement repositories
- Can swap implementations (e.g., different databases)

---

## Error Handling

All services return `bool` or `nullable` values for safety:

```csharp
// Service returns bool
var success = await _employeeService.CreateEmployeeAsync(employee);
if (!success)
{
    ModelState.AddModelError("", "Error creating employee.");
    return View(employee);
}

// Service returns nullable
var employee = await _employeeService.GetEmployeeByIdAsync(id);
if (employee == null) return NotFound();
```

---

## Service Features

### Validation
- Employee existence checks before operations
- Score range validation (0-100) in PerformanceService
- Date range validation in LeaveService
- Username uniqueness in AccountService

### Calculations
- Net Salary = Basic Salary - Deductions (PayrollService)
- Average Attendance % (AttendanceService)
- Employee Average Score (PerformanceService)
- Monthly Report Generation (ReportService)

### Filtering
- By status (LeaveService, PayrollService)
- By date range (AttendanceService, AppraisalService)
- By employee (multiple services)

---

## Future Enhancements

1. **Add Repository Pattern** - Further abstract data access
2. **Add Caching** - Use IMemoryCache for frequently accessed data
3. **Add Logging** - Use ILogger for service operations
4. **Add Validation** - Use FluentValidation for robust model validation
5. **Add Unit of Work Pattern** - Manage multiple services in transactions

---

## Summary

The service layer provides a clean, maintainable, and testable architecture that follows SOLID principles. Controllers are now thin and focused on HTTP concerns, while all business logic resides in well-organized, reusable services.
