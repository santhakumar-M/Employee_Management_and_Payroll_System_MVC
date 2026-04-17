using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeManagement_mvc.Migrations
{
    /// <inheritdoc />
    public partial class AddMonthlyAttendanceSummary : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MonthlyAttendanceSummaries",
                columns: table => new
                {
                    SummaryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeId = table.Column<int>(type: "int", nullable: false),
                    Month = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DaysPresent = table.Column<int>(type: "int", nullable: false),
                    TotalWorkingDays = table.Column<int>(type: "int", nullable: false),
                    AttendancePercentage = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PreviousMonthData = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyAttendanceSummaries", x => x.SummaryId);
                    table.ForeignKey(
                        name: "FK_MonthlyAttendanceSummaries_Employees_EmployeeId",
                        column: x => x.EmployeeId,
                        principalTable: "Employees",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyAttendanceSummaries_EmployeeId",
                table: "MonthlyAttendanceSummaries",
                column: "EmployeeId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonthlyAttendanceSummaries");
        }
    }
}
