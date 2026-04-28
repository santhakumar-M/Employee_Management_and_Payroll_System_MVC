using System.Threading.Tasks;
using EmployeeHrSystem.Controllers;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.mvc.Tests
{
    [TestFixture]
    public class PayrollControllerTests
    {
        [Test]
        public async Task Index_SetsViewBagAndReturnsView()
        {
            var ctrl = new PayrollController(new FakePayrollService(), new FakeEmployeeService(), new FakeAttendanceService())
            {
                ControllerContext = TestHelper.CreateContext()
            };

            var result = await ctrl.Index();
            Assert.IsInstanceOf<ViewResult>(result);
            Assert.IsNotNull(ctrl.ViewBag.EmployeeItems);
            Assert.IsNotNull(ctrl.ViewBag.PayrollDetails);
        }
    }
}
