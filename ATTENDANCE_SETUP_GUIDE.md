# Attendance System - Setup & Testing Guide

## Prerequisites

- Visual Studio 2022+ or Visual Studio Code
- .NET 8.0 SDK
- SQL Server (LocalDB, SQL Server Express, or full version)
- PowerShell or Command Prompt

## Initial Setup

### Step 1: Verify Database Connection

1. Open `appsettings.json` in the project root
2. Check the connection string:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EmployeeHrDb;Trusted_Connection=true;"
   }
   ```
3. Update `Server` value if using different SQL Server instance

### Step 2: Run Migrations

```powershell
# Navigate to project directory
cd C:\path\to\EmployeeManagement-mvc\EmployeeManagement-mvc

# Create and apply migrations
dotnet ef database update

# Verify migration status
dotnet ef migrations list
# Should show: 20260415035532_InitialMigration and 20260417043301_AddMonthlyAttendanceSummary
```

### Step 3: Start the Application

```powershell
# In Visual Studio: Press F5 or Debug > Start Debugging
# Or in terminal:
dotnet run

# Application should open at: https://localhost:7113
```

### Step 4: Initial Login

The database is seeded with test users:

| Username | Password | Role |
|----------|----------|------|
| admin | Admin@123 | Admin |
| hruser | Hr@123 | HR Officer |
| manager | Mgr@123 | Manager |
| payroll | Pay@123 | Payroll Officer |
| employee | Emp@123 | Employee |

**Login as HR Officer or Admin to access attendance features**

```
1. Go to https://localhost:7113/Account/Login
2. Enter: Username = "hruser", Password = "Hr@123"
3. Click Login
4. Navigate to Attendance menu
```

## Testing the System

### Test 1: View Employee List

**Objective**: Verify employees exist in the system

**Steps**:
1. Login as hruser
2. Navigate to **Employee Management > Employees**
3. Should see 5 test employees:
   - Anita (Developer)
   - Ravi (Tester)
   - Priya (HR Officer)
   - Rajesh (Payroll Officer)
   - Vikram (Manager)

**Expected**: List shows all employees

**Troubleshoot**:
- If empty: Database not seeded properly, run `dotnet ef database update`
- If error: Check database connection in appsettings.json

### Test 2: Mark Attendance

**Objective**: Mark employees present for today

**Steps**:
1. Navigate to **Attendance > Mark Attendance**
2. Verify date field shows today's date
3. Verify all 5 employees appear in the list
4. Check **Select All** checkbox
5. Click **Mark Attendance** button

**Expected Results**:
- ✅ Page shows success message: "Attendance marked successfully for 5 employee(s)."
- ✅ Redirected to attendance list
- ✅ No error messages

**Troubleshoot**:
- If "No employees found": Check database connection and employee data
- If error on mark: See ATTENDANCE_TROUBLESHOOTING.md Issue #2

### Test 3: View Monthly Report

**Objective**: Verify monthly summaries are calculated

**Steps**:
1. Navigate to **Attendance > Monthly Report**
2. Verify current month displays (e.g., "January 2025")
3. Should see all 5 employees with statistics:
   - Days Present: 1 (just marked)
   - Working Days: ~20 (for current month)
   - Attendance %: ~5% (1 ÷ ~20 × 100)
4. Scroll down to "Summary Statistics"

**Expected Results**:
- ✅ All employees listed with calculations
- ✅ Days Present = 1 (from previous test)
- ✅ Attendance % calculated correctly
- ✅ Status badges show colors (red for low %)

**Troubleshoot**:
- If empty: Mark attendance first (Test 2), then refresh
- If 0%: Check TotalWorkingDays calculation

### Test 4: View Employee Detail Report

**Objective**: Test individual employee report

**Steps**:
1. From Monthly Report, click **View Details** on any employee (e.g., Anita)
2. Verify employee name appears: "Anita - Attendance Report"
3. View summary cards showing:
   - Days Present: 1
   - Working Days: ~20
   - Attendance %: ~5%
   - Days Absent: ~19
4. Scroll down to see daily attendance table

**Expected Results**:
- ✅ Employee name and designation display
- ✅ Summary cards show correct values
- ✅ Daily records table shows today's entry with PRESENT status
- ✅ Today's row highlighted in green

**Troubleshoot**:
- If "No summary found": Mark attendance for this employee first
- If daily records empty: Check Attendances table in database

### Test 5: Toggle Individual Employee Checkbox

**Objective**: Test single employee selection

**Steps**:
1. Go to **Mark Attendance**
2. Uncheck **Select All** to deselect everyone
3. Check only Anita's checkbox
4. Verify row highlights green
5. Click **Mark Attendance**

**Expected Results**:
- ✅ Success message: "Attendance marked successfully for 1 employee(s)."
- ✅ Only Anita marked for today
- ✅ Monthly Report shows Anita with updated count

**Troubleshoot**:
- If checkbox doesn't respond: Check browser console (F12) for JS errors
- Clear cache (Ctrl+Shift+Delete) and hard refresh (Ctrl+F5)

### Test 6: Mark Attendance for Different Date

**Objective**: Test marking attendance for historical dates

**Steps**:
1. Go to **Mark Attendance**
2. Change date to 2 days ago
3. Select 3 employees (Anita, Ravi, Priya)
4. Click **Mark Attendance**
5. Go to **Monthly Report**

**Expected Results**:
- ✅ Success: "Attendance marked successfully for 3 employee(s)."
- ✅ Monthly Report shows updated counts for those 3 employees
- ✅ Days Present increased by 1 for each

**Troubleshoot**:
- If count wrong: Previous records for that date may have been replaced (expected behavior)

### Test 7: Test Select All Functionality

**Objective**: Verify bulk selection works

**Steps**:
1. Go to **Mark Attendance**
2. Select 3 employees manually
3. Click **Select All** checkbox
4. All employees should be checked
5. Click **Select All** again
6. All should be unchecked

**Expected Results**:
- ✅ Select All toggles all checkboxes
- ✅ Can select individual employees
- ✅ Can deselect individual employees

**Troubleshoot**:
- If toggle doesn't work: Check `updateSelectAllCheckbox()` function in MarkForDay.cshtml

### Test 8: Authorization Test

**Objective**: Verify role-based access control

**Steps**:
1. Login as employee user (Username: "employee", Password: "Emp@123")
2. Try to access **Attendance > Mark Attendance**

**Expected Results**:
- ✅ Access denied / 403 Forbidden page shown
- ✅ Cannot access attendance features
- ✅ Redirected to login or home page

**Troubleshoot**:
- If employee can access: Check [Authorize] attributes on AttendanceController

## Database Verification

### Check Seeded Data

Open SQL Server Management Studio or use terminal:

```sql
-- Check employees
SELECT * FROM Employees;
-- Should show: 5 employees (Anita, Ravi, Priya, Rajesh, Vikram)

-- Check monthly summaries
SELECT * FROM MonthlyAttendanceSummaries;
-- Should show: 5 records for current month

-- Check attendance records
SELECT * FROM Attendances;
-- Should show: records from marking tests

-- Check users
SELECT * FROM Users;
-- Should show: 5 users (admin, hruser, payroll, manager, employee)
```

### Create Missing Tables (if needed)

If migration didn't create tables:

```powershell
# Force database update
dotnet ef database update --force

# Or drop and recreate
dotnet ef database drop --force
dotnet ef database update
```

## Common Setup Issues

### Issue: "Database already exists. Skipping creation."

**Solution**: Normal message, database was already created. Continue.

### Issue: "No migrations were applied"

**Solution**: Check migration status:
```powershell
dotnet ef migrations list
# If empty: migrations not created
# Run: dotnet ef migrations add AddMonthlyAttendanceSummary
```

### Issue: "Connection string not found"

**Solution**: Verify appsettings.json has:
```json
"ConnectionStrings": {
  "DefaultConnection": "..."
}
```

### Issue: Employees don't appear on Mark Attendance page

**Solution**:
1. Verify DbInitializer is called in Program.cs
2. Check Program.cs has:
   ```csharp
   using (var scope = app.Services.CreateScope())
   {
       var services = scope.ServiceProvider;
       var context = services.GetRequiredService<ApplicationContext>();
       var hasher = services.GetRequiredService<IPasswordHasher<AppUser>>();
       DbInitializer.Seed(context, hasher);
   }
   ```

## Performance Notes

- **First load**: May take 1-2 seconds (EF Core warming up)
- **Database seeding**: Automatic on first run
- **5 employees**: Loads instantly
- **1000+ employees**: Consider pagination (not in v1.0)

## Cleanup / Reset

### Reset to Fresh State

```powershell
# Delete database and recreate
dotnet ef database drop --force

# Apply migrations (creates fresh database)
dotnet ef database update

# Reseed data (automatic on first run)
dotnet run
```

### Clear Attendance Data Only

```sql
-- Keep employees, clear attendance
DELETE FROM Attendances;
DELETE FROM MonthlyAttendanceSummaries;

-- Re-initialize summaries (optional)
-- Run app or execute custom initialization
```

## Next Steps After Setup

1. ✅ **Test with real employees**: Create actual employee records
2. ✅ **Assign users**: Create login accounts for HR staff
3. ✅ **Mark daily attendance**: Start using Mark Attendance feature
4. ✅ **Monitor reports**: Review Monthly Report and employee details
5. ✅ **Archive old months**: Previous month data auto-saves

## Support & Troubleshooting

- **User Guide**: See `ATTENDANCE_SYSTEM_GUIDE.md`
- **Troubleshooting**: See `ATTENDANCE_TROUBLESHOOTING.md`
- **Quick Checks**:
  - Run build: `dotnet build`
  - Run app: `dotnet run`
  - Check database: `dotnet ef migrations list`
  - Clear cache: Ctrl+Shift+Delete, Ctrl+F5

## Success Criteria

You know the system is set up correctly when:

- [ ] Build succeeds: `dotnet build` completes without errors
- [ ] Database updates: `dotnet ef database update` shows successful
- [ ] Login works: Can login as hruser/Hr@123
- [ ] Employees visible: Mark Attendance page shows 5 employees
- [ ] Marking works: Can mark employees and see success message
- [ ] Reports show: Monthly Report displays employee attendance
- [ ] Details work: Can view individual employee reports
- [ ] Authorization works: Employee role cannot access attendance

## Credentials for Testing

**Admin Account**:
- Username: `admin`
- Password: `Admin@123`
- Role: Admin

**HR Officer Account** (Recommended for testing):
- Username: `hruser`
- Password: `Hr@123`
- Role: HR Officer

**Manager Account**:
- Username: `manager`
- Password: `Mgr@123`
- Role: Manager

**Employee Account** (Limited access):
- Username: `employee`
- Password: `Emp@123`
- Role: Employee (cannot access attendance features)

## Version Info

- **System**: Bulk Attendance Marking System v1.0
- **.NET**: 8.0
- **Database**: SQL Server (LocalDB compatible)
- **Created**: January 2025
- **Last Updated**: January 2025
