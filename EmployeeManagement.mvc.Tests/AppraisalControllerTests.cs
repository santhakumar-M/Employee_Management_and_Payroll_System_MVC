using System.Threading.Tasks;
using EmployeeHrSystem.Controllers;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.mvc.Tests
{
    [TestFixture]
    public class AppraisalControllerTests
    {
        [Test]
        public async Task Update_Get_ReturnsView()
        {
            var ctrl = new AppraisalController(new FakeAppraisalService(), new FakeEmployeeService())
            {
                ControllerContext = TestHelper.CreateContext()
            };

            var result = await ctrl.Update();
            Assert.IsInstanceOf<ViewResult>(result);
        }
    }
}
