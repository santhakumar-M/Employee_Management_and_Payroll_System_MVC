using EmployeeHrSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace EmployeeHrSystem.Data
{
    public static class DbInitializer
    {
        public static void Seed(ApplicationContext ctx, IPasswordHasher<AppUser> hasher)
        {
            ctx.Database.EnsureCreated();

            // Seed Departments
            if (!ctx.Departments.Any())
            {
                ctx.Departments.AddRange(
                    new Department { DepartmentName = "Engineering" },
                    new Department { DepartmentName = "QA" },
                    new Department { DepartmentName = "HR" },
                    new Department { DepartmentName = "Payroll" },
                    new Department { DepartmentName = "Management" }
                );
                ctx.SaveChanges();
            }

            // Seed Employees
            if (!ctx.Employees.Any())
            {
                var eng = ctx.Departments.First(d => d.DepartmentName == "Engineering");
                var qa = ctx.Departments.First(d => d.DepartmentName == "QA");
                var hr = ctx.Departments.First(d => d.DepartmentName == "HR");
                var payroll = ctx.Departments.First(d => d.DepartmentName == "Payroll");
                var management = ctx.Departments.First(d => d.DepartmentName == "Management");

                ctx.Employees.AddRange(
                    new Employee { Name = "Anita", Designation = "Developer", DepartmentId = eng.DepartmentId, BasicSalary = 65000, ContactInfo = "anita@example.com" },
                    new Employee { Name = "Ravi", Designation = "Tester", DepartmentId = qa.DepartmentId, BasicSalary = 52000, ContactInfo = "ravi@example.com" },
                    new Employee { Name = "Priya", Designation = "HR Officer", DepartmentId = hr.DepartmentId, BasicSalary = 55000, ContactInfo = "priya@example.com" },
                    new Employee { Name = "Rajesh", Designation = "Payroll Officer", DepartmentId = payroll.DepartmentId, BasicSalary = 58000, ContactInfo = "rajesh@example.com" },
                    new Employee { Name = "Vikram", Designation = "Manager", DepartmentId = management.DepartmentId, BasicSalary = 75000, ContactInfo = "vikram@example.com" }
                );
                ctx.SaveChanges();
            }

            // Seed AppUsers with hashed passwords
            if (!ctx.Users.Any())
            {
                var adminUser = new AppUser { Username = "admin", Role = "Admin" };
                adminUser.PasswordHash = hasher.HashPassword(adminUser, "Admin@123");

                var hrUser = new AppUser { Username = "hruser", Role = "HR Officer" };
                hrUser.PasswordHash = hasher.HashPassword(hrUser, "Hr@123");

                var payrollUser = new AppUser { Username = "payroll", Role = "Payroll Officer" };
                payrollUser.PasswordHash = hasher.HashPassword(payrollUser, "Pay@123");

                var managerUser = new AppUser { Username = "manager", Role = "Manager" };
                managerUser.PasswordHash = hasher.HashPassword(managerUser, "Mgr@123");

                var employeeUser = new AppUser { Username = "employee", Role = "Employee" };
                employeeUser.PasswordHash = hasher.HashPassword(employeeUser, "Emp@123");

                ctx.Users.AddRange(adminUser, hrUser, payrollUser, managerUser, employeeUser);
                ctx.SaveChanges();
            }

            // Seed Monthly Attendance Summaries for current month
            var currentMonth = DateTime.Now.ToString("yyyy-MM");
            if (!ctx.MonthlyAttendanceSummaries.Any(m => m.Month == currentMonth))
            {
                var employees = ctx.Employees.ToList();
                var summaries = new List<MonthlyAttendanceSummary>();

                foreach (var emp in employees)
                {
                    // Calculate working days (Mon-Fri only)
                    int year = DateTime.Now.Year;
                    int month = DateTime.Now.Month;
                    int daysInMonth = DateTime.DaysInMonth(year, month);
                    int workingDays = 0;

                    for (int day = 1; day <= daysInMonth; day++)
                    {
                        var dayOfWeek = new DateOnly(year, month, day).DayOfWeek;
                        if (dayOfWeek != DayOfWeek.Saturday && dayOfWeek != DayOfWeek.Sunday)
                        {
                            workingDays++;
                        }
                    }

                    summaries.Add(new MonthlyAttendanceSummary
                    {
                        EmployeeId = emp.Id,
                        Month = currentMonth,
                        DaysPresent = 0,
                        TotalWorkingDays = workingDays,
                        AttendancePercentage = 0,
                        CreatedDate = DateTime.Now
                    });
                }

                ctx.MonthlyAttendanceSummaries.AddRange(summaries);
                ctx.SaveChanges();
            }
        }
    }
}