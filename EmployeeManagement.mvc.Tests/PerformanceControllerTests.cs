using System.Threading.Tasks;
using EmployeeHrSystem.Controllers;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.mvc.Tests
{
    [TestFixture]
    public class PerformanceControllerTests
    {
        [Test]
        public async Task Record_Get_ReturnsView()
        {
            var ctrl = new PerformanceController(new FakePerformanceService(), new FakeEmployeeService())
            {
                ControllerContext = TestHelper.CreateContext()
            };

            var result = await ctrl.Record();
            Assert.IsInstanceOf<ViewResult>(result);
        }
    }
}
