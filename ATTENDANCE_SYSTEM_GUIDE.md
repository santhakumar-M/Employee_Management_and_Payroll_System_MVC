# Bulk Attendance Marking System - User Guide

## Overview
The new Bulk Attendance Marking System allows HR officers and managers to efficiently mark attendance for multiple employees on a specific date, with automatic monthly tracking and reporting.

## Features

### 1. **Mark Attendance for Day** (`/Attendance/MarkForDay`)
Mark attendance for all employees on a specific date.

#### How to Use:
1. Navigate to **Attendance > Mark Attendance**
2. Select a date from the date picker (defaults to today)
3. View the list of all employees with checkboxes
4. **Select employees** you want to mark as Present by checking their checkboxes
5. Use **Select All** checkbox to quickly select/deselect all employees
6. Click **"Mark Attendance"** button to save
7. On success, you'll be redirected to the attendance list with a confirmation message

#### Key Features:
- ✅ Date picker for selecting any date
- ✅ Employee list with visual status (Present/Not Marked)
- ✅ Color highlighting (green) for selected employees
- ✅ Quick actions to mark/unmark individual employees
- ✅ Select All toggle for bulk operations
- ✅ Real-time visual feedback

### 2. **Monthly Attendance Report** (`/Attendance/MonthlyReport`)
View monthly attendance summary for all employees.

#### How to Use:
1. Navigate to **Attendance > Monthly Report**
2. The system shows the current month by default
3. View summary statistics:
   - **Days Present**: Number of days marked as present
   - **Working Days**: Total weekdays in the month (excludes weekends)
   - **Attendance %**: Percentage (Days Present ÷ Working Days × 100)
   - **Status Badge**: Visual indicator (Excellent ≥80%, Good 70-79%, Below Target <70%)
4. Click **"View Details"** on any employee to see daily breakdown
5. Click **"Mark Today's Attendance"** for quick access to marking page

#### Summary Statistics Section:
- Total number of employees
- Average attendance percentage for the month
- Count of employees by performance category

### 3. **Employee Attendance Report** (`/Attendance/EmployeeAttendanceReport`)
View detailed attendance report for a specific employee and month.

#### How to Use:
1. From Monthly Report, click **"View Details"** on any employee
2. View at a glance:
   - **Days Present**: Count of days marked as PRESENT
   - **Working Days**: Total weekdays in the month
   - **Attendance %**: Calculated percentage with status
   - **Days Absent**: Auto-calculated (Working Days - Days Present)
3. Scroll down to view **Daily Attendance Records**:
   - Date, Day of Week, Status (Present/Absent), and action indicator
   - Color-coded rows (Green=Present, Red=Absent)
4. View previous month data (if available) for historical comparison
5. Click **"Back to Monthly Report"** to return

## Technical Details

### Data Structure

#### Monthly Attendance Summary
The system tracks attendance at two levels:

**Daily Level** (Attendance Table):
- Employee ID
- Date (DateOnly)
- Status (PRESENT/ABSENT/LEAVE)

**Monthly Level** (MonthlyAttendanceSummary Table):
- Employee ID
- Month (YYYY-MM format)
- Days Present (counted from daily records with PRESENT status)
- Total Working Days (weekdays only, Mon-Fri)
- Attendance Percentage (auto-calculated)
- Previous Month Data (JSON archival)

### Monthly Reset Behavior
At the start of each month:
1. Previous month's statistics are archived in JSON format
2. New monthly summary records are created with:
   - DaysPresent = 0 (resets)
   - TotalWorkingDays = calculated for the month
   - AttendancePercentage = 0
3. Historical data retained for reporting and auditing

### Working Day Calculation
- **Included**: Monday through Friday
- **Excluded**: Saturday, Sunday, and public holidays (current version)
- **Formula**: Automated count of weekdays in the month

## Navigation

### From Attendance Index:
```
Attendance Home (Index)
├── Mark Attendance (MarkForDay)
├── View Monthly Report (MonthlyReport)
└── View All Records (Index with details)
```

### From Mark Attendance:
```
MarkForDay
├── Cancel → Back to Index
└── Mark Attendance → Index (with success message)
```

### From Monthly Report:
```
MonthlyReport
├── Mark Attendance → MarkForDay
├── View Details → EmployeeAttendanceReport (per employee)
└── Back → (navigation based on referring page)
```

### From Employee Report:
```
EmployeeAttendanceReport
└── Back to Monthly Report → MonthlyReport
```

## Common Tasks

### Task: Mark 15 employees as present for today
1. Go to **Mark Attendance**
2. Date is already set to today
3. Click **Select All** checkbox
4. Click **Mark Attendance**
5. All 15 employees marked as present for today ✓

### Task: Check attendance for January 2024
1. Go to **Monthly Report**
2. System shows current month by default
3. To view a different month, use browser back button and navigate to report with ?month=2024-01 parameter
4. View all employees' attendance for January

### Task: View details for one employee
1. Go to **Monthly Report**
2. Find the employee in the table
3. Click **"View Details"** button
4. See daily breakdown with dates and status
5. View previous month data for comparison

### Task: Correct attendance for previous day
1. Go to **Mark Attendance**
2. Change the date to yesterday
3. Uncheck employees who should not be marked
4. Check employees who should be marked
5. Click **Mark Attendance**
6. System replaces previous day's records with new selections ✓

## Error Handling

### "No employees found"
- **Cause**: No employees in the system
- **Solution**: Add employees through Employee Management first

### "No attendance records found for this month"
- **Cause**: This is the employee's first month or no attendance marked yet
- **Solution**: Mark attendance for employees using "Mark Attendance" page

### "Error marking attendance. Please try again."
- **Cause**: Database or validation error
- **Solution**: 
  - Ensure employees exist in the system
  - Check database connection
  - Contact system administrator if issue persists

## Permissions

- **Required Role**: Admin, HR Officer, or Manager
- All Attendance pages require authentication and proper role authorization
- Other users cannot access attendance marking or reporting

## Database Requirements

The following tables must exist:
- `Employees` - Employee master data
- `Attendances` - Daily attendance records
- `MonthlyAttendanceSummaries` - Monthly aggregated data

The system will create these tables automatically during initial setup/migration.

## Best Practices

1. **Daily Marking**: Mark attendance at the end of each business day
2. **Bulk Operations**: Use "Select All" when marking large teams
3. **Corrections**: Reselect employees for a date to replace previous records
4. **Reporting**: Check monthly reports for attendance patterns
5. **Archive**: Previous month data is automatically archived for compliance

## API Reference (For Developers)

### Service Methods

**IAttendanceService Interface** provides:
```
- GetAllEmployeesForAttendanceAsync() → List<Employee>
- MarkMultipleAttendanceAsync(date, employeeIds) → bool
- GetAttendanceForDateAsync(date) → List<Attendance>
- GetMonthlyAttendanceSummaryAsync(employeeId, month) → MonthlyAttendanceSummary
- GetAllMonthlyAttendanceSummariesAsync(month) → List<MonthlyAttendanceSummary>
- GetAttendancePercentageAsync(employeeId, month) → decimal
- ResetMonthlyAttendanceAsync(newMonth) → bool
```

## Support

For issues or feature requests:
1. Check this guide for common tasks
2. Verify employee data exists in the system
3. Check database connectivity
4. Contact system administrator with error messages

## Version History

- **v1.0** (Current): Initial bulk attendance system with monthly tracking
  - Bulk marking UI
  - Monthly summary reporting
  - Employee-level detailed reports
  - Data archival for previous months
  - Automatic working day calculation
