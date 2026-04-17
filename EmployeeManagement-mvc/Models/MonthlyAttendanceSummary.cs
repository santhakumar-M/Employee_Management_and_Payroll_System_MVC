using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeHrSystem.Models
{
    public class MonthlyAttendanceSummary
    {
        [Key]
        public int SummaryId { get; set; }

        public int EmployeeId { get; set; }

        [ForeignKey(nameof(EmployeeId))]
        public Employee? Employee { get; set; }

        // Month in format YYYY-MM (e.g., "2024-01")
        public string Month { get; set; } = string.Empty;

        // Total days present in the month
        public int DaysPresent { get; set; } = 0;

        // Total working days in the month
        public int TotalWorkingDays { get; set; } = 0;

        // Attendance percentage
        public decimal AttendancePercentage { get; set; } = 0;

        // When this summary was created/updated
        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Previous month's attendance (for comparison/reporting)
        public string? PreviousMonthData { get; set; }
    }
}
