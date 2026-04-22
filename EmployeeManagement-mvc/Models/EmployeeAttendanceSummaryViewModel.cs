using System.Collections.Generic;

namespace EmployeeHrSystem.Models
{
    public class EmployeeAttendanceSummaryViewModel
    {
        public Employee? Employee { get; set; }
        public string Month { get; set; } = string.Empty;
        public int PresentDays { get; set; }
        public int AbsentDays { get; set; }
        public int LeaveDays { get; set; }
        public List<Attendance> Records { get; set; } = new();
        // Performance evaluations/feedback for the employee
        public List<Evaluation> Evaluations { get; set; } = new();
    }
}
