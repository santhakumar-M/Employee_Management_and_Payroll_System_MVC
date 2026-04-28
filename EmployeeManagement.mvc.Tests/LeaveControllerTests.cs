using System.Security.Claims;
using System.Threading.Tasks;
using EmployeeHrSystem.Controllers;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.mvc.Tests
{
    [TestFixture]
    public class LeaveControllerTests
    {
        [Test]
        public async Task Index_AsEmployee_ReturnsView()
        {
            var claims = new[] { new Claim(ClaimTypes.Role, "Employee"), new Claim("EmployeeId", "1") };
            var user = new ClaimsPrincipal(new ClaimsIdentity(claims));

            var ctrl = new LeaveController(new FakeLeaveService(), new FakeEmployeeService())
            {
                ControllerContext = TestHelper.CreateContext(user)
            };

            var result = await ctrl.Index();
            Assert.IsInstanceOf<ViewResult>(result);
        }
    }
}
