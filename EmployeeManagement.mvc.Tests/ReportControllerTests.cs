using System.Threading.Tasks;
using EmployeeHrSystem.Controllers;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.mvc.Tests
{
    [TestFixture]
    public class ReportControllerTests
    {
        [Test]
        public async Task ExportCsv_ReturnsFile()
        {
            var ctrl = new ReportController(new FakeReportService(), new FakeDashboardService())
            {
                ControllerContext = TestHelper.CreateContext()
            };

            var result = await ctrl.ExportCsv(null, null);
            Assert.IsInstanceOf<FileContentResult>(result);
            var file = result as FileContentResult;
            Assert.AreEqual("text/csv", file?.ContentType);
        }
    }
}
