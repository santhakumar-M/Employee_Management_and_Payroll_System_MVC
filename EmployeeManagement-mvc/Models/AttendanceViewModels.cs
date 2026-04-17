namespace EmployeeHrSystem.Models
{
    public class EmployeeAttendanceItem
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public string? Designation { get; set; }
        public string Status { get; set; } = "ABSENT"; // PRESENT or ABSENT
    }

    public class BulkAttendanceViewModel
    {
        public DateOnly SelectedDate { get; set; }
        public List<EmployeeAttendanceItem> Employees { get; set; } = new();
    }

    public class EmployeeAttendanceReportViewModel
    {
        public MonthlyAttendanceSummary? Summary { get; set; }
        public List<Attendance> AttendanceRecords { get; set; } = new();
        public string CurrentMonth { get; set; } = string.Empty;
        public string PreviousMonth { get; set; } = string.Empty;
    }
}
