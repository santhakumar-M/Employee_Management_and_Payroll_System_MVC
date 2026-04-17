# Attendance System - Troubleshooting & Fixes

## Common Issues and Solutions

### Issue 1: "Page Not Found" when accessing Attendance pages

**Symptoms:**
- 404 error when trying to access `/Attendance/MarkForDay`
- Routes not recognized

**Solutions:**
1. **Verify routing in Program.cs:**
   ```csharp
   // In Program.cs, ensure this line exists:
   app.MapControllerRoute(
       name: "default",
       pattern: "{controller=Home}/{action=Index}/{id?}");
   ```

2. **Check controller name:**
   - Controller file: `AttendanceController.cs`
   - Route: `/Attendance/ActionName`

3. **Clear browser cache:**
   - Ctrl+Shift+Delete (or Cmd+Shift+Delete on Mac)
   - Clear all cached images and files

4. **Restart application:**
   - Stop debugging
   - Clean solution: Build > Clean Solution
   - Rebuild: Build > Rebuild Solution
   - Start debugging again

### Issue 2: "Error marking attendance. Please try again."

**Symptoms:**
- Click "Mark Attendance" button
- Error message appears
- Attendance not saved

**Common Causes & Fixes:**

**a) No employees exist in database:**
```
✓ Solution: Add employees first via Employee Management > Create Employee
✓ Verify: Go to Attendance > Mark Attendance - should show employee list
```

**b) Model validation failed:**
```
✓ Solution: Ensure date is selected
✓ Verify: Date field should have a value (defaults to today)
```

**c) Database connection issue:**
```
✓ Check: Database server is running
✓ Verify: Connection string in appsettings.json is correct
✓ Test: Run database migrations: dotnet ef database update
```

**d) Missing monthly attendance summary records:**
```
✓ Solution: Run ResetMonthlyAttendanceAsync() to initialize
✓ Code to add in DbInitializer.cs:
    // Initialize monthly attendance summaries
    var employees = context.Employees.ToList();
    var currentMonth = DateTime.Now.ToString("yyyy-MM");
    
    foreach (var employee in employees)
    {
        var exists = context.MonthlyAttendanceSummaries
            .Any(m => m.EmployeeId == employee.Id && m.Month == currentMonth);
        
        if (!exists)
        {
            context.MonthlyAttendanceSummaries.Add(new MonthlyAttendanceSummary
            {
                EmployeeId = employee.Id,
                Month = currentMonth,
                DaysPresent = 0,
                TotalWorkingDays = 20, // Calculate properly in production
                CreatedDate = DateTime.Now
            });
        }
    }
    context.SaveChanges();
```

### Issue 3: Checkboxes not working / Can't select employees

**Symptoms:**
- Checkboxes appear but don't respond to clicks
- "Select All" doesn't work
- "Mark Present" button doesn't toggle employee

**Causes & Fixes:**

**a) JavaScript error in console:**
```
✓ Check: Open browser Developer Tools (F12)
✓ Go to: Console tab
✓ Look for: JavaScript errors
✓ Fix: Copy error message and contact support
```

**b) Form not binding correctly:**
```
✓ Verify: Hidden inputs have correct names:
   <input type="hidden" name="Employees[@i].EmployeeId" ... />
   <input type="hidden" name="Employees[@i].EmployeeName" ... />
   
✓ Check: @i in the loop generates correct indices (0, 1, 2...)
```

**c) Checkbox ID mismatch:**
```
✓ Each checkbox must have id="checkbox_@i"
✓ The onclick handler references: toggleCheckbox(@i)
✓ Verify: All are matched correctly in MarkForDay.cshtml
```

**Quick Fix (Force Reload):**
1. Press Ctrl+Shift+Delete
2. Clear browser cache
3. Press Ctrl+F5 (hard refresh)
4. Try again

### Issue 4: Page shows "No employees found"

**Symptoms:**
- MarkForDay page loads but shows no employees
- Employee list is empty

**Causes & Fixes:**

**a) No employees in database:**
```
✓ Check: Go to Employee Management
✓ Add: Create at least one employee
✓ Verify: Refresh attendance page
```

**b) GetAllEmployeesForAttendanceAsync not returning data:**
```
✓ Check: Database query
✓ Code in AttendanceService.cs should be:
    public async Task<List<Employee>> GetAllEmployeesForAttendanceAsync()
    {
        return await _context.Employees
            .OrderBy(e => e.Name)
            .ToListAsync();
    }
✓ Verify: This method is implemented correctly
```

**c) Authentication/Authorization issue:**
```
✓ Check: Logged in as Admin, HR Officer, or Manager
✓ Not logged in: Login first at /Account/Login
✓ Wrong role: Contact administrator to assign correct role
```

### Issue 5: "No attendance records found for this month" in Monthly Report

**Symptoms:**
- Monthly Report page shows empty message
- No employee data displayed

**Causes & Fixes:**

**a) No attendance marked yet:**
```
✓ Solution: Go to MarkForDay and mark some employees
✓ Return to MonthlyReport - should now show data
```

**b) Monthly summary records don't exist:**
```
✓ Check: MonthlyAttendanceSummaries table is empty
✓ Solution: Initialize summaries
   - Go to MarkForDay
   - Mark any employee for today
   - This will create summary records
   - Check MonthlyReport again
```

**c) Wrong month selected:**
```
✓ Check: URL should have ?month=yyyy-MM
✓ Current month: January 2024 = ?month=2024-01
✓ Switch months: Modify URL month parameter
```

### Issue 6: Employee Attendance Report shows "No summary found"

**Symptoms:**
- Navigate to EmployeeAttendanceReport
- Page returns "No attendance summary found"

**Causes & Fixes:**

**a) Employee not yet marked in current month:**
```
✓ Solution: Go to MarkForDay
✓ Mark the employee for today
✓ Return to view their report
```

**b) Wrong month in URL:**
```
✓ Check: URL contains ?month=yyyy-MM
✓ Verify: Month matches when attendance was marked
✓ Example: If marked in Jan 2024, use ?month=2024-01
```

**c) Employee deleted after marking attendance:**
```
✓ Check: Employee still exists in database
✓ Verify: EmployeeId matches in Attendances table
```

### Issue 7: Date picker doesn't show / Invalid date format

**Symptoms:**
- Date field appears empty
- Cannot select date
- Error: "Value is not in a valid date format"

**Causes & Fixes:**

**a) Browser date input support:**
```
✓ Update: Use modern browser (Chrome, Firefox, Edge, Safari latest)
✓ Note: Internet Explorer not supported
```

**b) JavaScript date parsing:**
```
✓ Verify: MarkForDay.cshtml line has:
   value="@Model.SelectedDate.ToString("yyyy-MM-dd")"
✓ Format: Must be "yyyy-MM-dd" (e.g., "2024-01-15")
```

**c) Force date with JavaScript:**
```
✓ In browser console, run:
   document.getElementById('selectedDate').value = '2024-01-15';
✓ Then try submitting form
```

## Database Issues

### Issue 8: "No such table: MonthlyAttendanceSummaries"

**Symptoms:**
- Error: "Invalid object name 'MonthlyAttendanceSummaries'"
- EntityFramework can't find table

**Solution:**
```powershell
# In Package Manager Console:
cd EmployeeManagement-mvc\EmployeeManagement-mvc

# Run migrations
dotnet ef migrations add AddMonthlyAttendanceSummary

# Apply to database
dotnet ef database update

# If still failing:
dotnet ef database update --force

# As last resort, remove and recreate:
dotnet ef database drop --force
dotnet ef database update
```

### Issue 9: "Migrations are pending" error

**Symptoms:**
- Application won't start
- Error: "Migrations are pending"
- Database context error

**Solution:**
```powershell
# Navigate to project
cd C:\Users\[user]\source\repos\EmployeeManagement-mvc\EmployeeManagement-mvc

# Apply all pending migrations
dotnet ef database update

# Verify
dotnet ef migrations list
```

### Issue 10: Foreign Key constraint violation

**Symptoms:**
- Error: "The INSERT, UPDATE, or DELETE statement conflicted with a FOREIGN KEY constraint"
- Can't mark attendance for an employee

**Causes & Fixes:**

**a) Employee was deleted:**
```
✓ Check: Employee exists in Employees table
✓ Re-add employee if needed
✓ Use existing employees for attendance marking
```

**b) Orphaned attendance records:**
```
✓ In SQL Server Management Studio:
   DELETE FROM Attendances WHERE EmployeeId NOT IN (SELECT Id FROM Employees);
   DELETE FROM MonthlyAttendanceSummaries WHERE EmployeeId NOT IN (SELECT Id FROM Employees);
```

## Performance Issues

### Issue 11: Page loads slowly / Attendance list is slow

**Symptoms:**
- MarkForDay takes 10+ seconds to load
- MonthlyReport shows spinning loader

**Causes & Fixes:**

**a) Many employees (>1000):**
```
✓ Add pagination: Modify controller to load 50 at a time
✓ Implement search: Filter by employee name
✓ Use stored procedure for monthly summaries
```

**b) Large database with many attendance records:**
```
✓ Create database index:
   CREATE INDEX IX_Attendances_Date ON Attendances(Date);
   CREATE INDEX IX_Attendances_EmployeeId ON Attendances(EmployeeId);
   CREATE INDEX IX_MonthlyAttendanceSummaries_Month 
       ON MonthlyAttendanceSummaries(Month);
```

**c) N+1 query problem:**
```
✓ Verify: Include() is used in queries
✓ Check: GetAllMonthlyAttendanceSummariesAsync() includes Employee:
   .Include(m => m.Employee)
```

## View/Display Issues

### Issue 12: Null reference exceptions in views

**Symptoms:**
- Yellow error page: "Object reference not set to an instance"
- Stack trace shows view file

**Quick Fixes:**
```razor
<!-- Before: may throw null reference error -->
<div>@Model.Summary.Employee.Name</div>

<!-- After: use null-coalescing -->
<div>@Model.Summary?.Employee?.Name</div>

<!-- Better: check first -->
@if (Model?.Summary?.Employee != null)
{
    <div>@Model.Summary.Employee.Name</div>
}
else
{
    <div>No employee data</div>
}
```

### Issue 13: Attendance percentage shows wrong value

**Symptoms:**
- Shows 0% even with days present
- Shows >100%
- Decimal calculations incorrect

**Cause & Fix:**
```csharp
// In AttendanceService.cs, UpdateMonthlyAttendanceSummaryAsync():

// CORRECT:
summary.AttendancePercentage = summary.TotalWorkingDays > 0 
    ? (decimal)summary.DaysPresent / summary.TotalWorkingDays * 100 
    : 0;

// WRONG (common mistakes):
// summary.AttendancePercentage = summary.DaysPresent / summary.TotalWorkingDays * 100; // int division!
// summary.AttendancePercentage = summary.DaysPresent / summary.TotalWorkingDays; // no 100 multiplier
```

## Quick Diagnostic Checklist

When something doesn't work:

- [ ] Employees exist in database
- [ ] User is logged in with correct role (Admin/HR Officer/Manager)
- [ ] Date is properly formatted (yyyy-MM-dd)
- [ ] Database migrations applied
- [ ] Application rebuilt (Clean + Rebuild)
- [ ] Browser cache cleared (Ctrl+Shift+Delete)
- [ ] Browser hard refresh (Ctrl+F5)
- [ ] Database connection string correct
- [ ] SQL Server running (if using LocalDB or network)
- [ ] No JavaScript errors (F12 > Console tab)

## Getting Help

**If issue persists after trying above:**
1. Note the exact error message
2. Include stack trace if available
3. Check browser console for JavaScript errors (F12)
4. Run `dotnet ef migrations list` to verify migrations
5. Check database directly in SQL Server Management Studio
6. Provide this information to system administrator

## Support Contact

For technical issues:
- Check `ATTENDANCE_SYSTEM_GUIDE.md` for user guide
- Verify database setup: `dotnet ef database update`
- Clear cache and restart: Ctrl+Shift+Delete, then Ctrl+F5
