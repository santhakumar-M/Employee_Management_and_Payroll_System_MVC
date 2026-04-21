using System;
using System.Collections.Generic;

namespace EmployeeHrSystem.Models
{
    public class HRReportViewModel
    {
        // Header
        public string CompanyName { get; set; } = "[Company Name]";
        public string CompanyLogoUrl { get; set; } = string.Empty; // placeholder for logo
        public string ReportTitle { get; set; } = "Monthly Payroll Report";
        public string ReportingPeriod { get; set; } = string.Empty; // e.g., March 2026
        public DateTime GeneratedOn { get; set; }
        public int ReportingYear { get; set; }
        public int ReportingMonth { get; set; }

        // Employee Summary
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int NewHires { get; set; }
        public int Resignations { get; set; }

        // Payroll Overview
        public decimal TotalGrossSalary { get; set; }
        public decimal TotalDeductions { get; set; }
        public decimal TotalNetSalary { get; set; }
        public decimal AverageSalary { get; set; }

        // Salary Breakdown
        public decimal TotalBasic { get; set; }
        public decimal TotalAllowances { get; set; }
        public decimal TotalOvertime { get; set; }
        public decimal TotalOtherBenefits { get; set; }

        // Deductions Summary
        public decimal TotalTax { get; set; }
        public decimal TotalPF { get; set; }
        public decimal TotalInsurance { get; set; }
        public decimal TotalLoanDeductions { get; set; }

        // Department-wise payroll
        public List<(string DepartmentName, int EmployeeCount, decimal PayrollCost)> DepartmentBreakdown { get; set; } = new();

        // Attendance & Leave
        public int PaidLeaveDays { get; set; }
        public int UnpaidLeaveDays { get; set; }
        public decimal LossOfPay { get; set; }

        // Compliance & Notes
        public string ComplianceStatus { get; set; } = "Compliant";
        public string Notes { get; set; } = string.Empty;

        // Authorization
        public string PreparedBy { get; set; } = string.Empty;
        public string ReviewedBy { get; set; } = string.Empty;
        public string ApprovedBy { get; set; } = string.Empty;
    }
}
