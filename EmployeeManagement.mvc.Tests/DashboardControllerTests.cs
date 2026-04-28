using System.Threading.Tasks;
using EmployeeHrSystem.Controllers;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.mvc.Tests
{
    [TestFixture]
    public class DashboardControllerTests
    {
        [Test]
        public async Task Admin_ReturnsView()
        {
            var ctrl = new DashboardController(new FakeDashboardService())
            {
                ControllerContext = TestHelper.CreateContext()
            };

            var result = await ctrl.Admin();
            Assert.IsInstanceOf<ViewResult>(result);
        }
    }
}
