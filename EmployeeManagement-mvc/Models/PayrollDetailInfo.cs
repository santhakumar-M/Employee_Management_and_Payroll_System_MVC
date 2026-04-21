namespace EmployeeHrSystem.Models
{
    public class PayrollDetailInfo
    {
        public int PayrollId { get; set; }
        public int UnpaidDays { get; set; }
        public decimal PerDayDeduction { get; set; }
        public decimal LeaveDeduction { get; set; }
        public int TotalWorkingDays { get; set; }
    }
}
