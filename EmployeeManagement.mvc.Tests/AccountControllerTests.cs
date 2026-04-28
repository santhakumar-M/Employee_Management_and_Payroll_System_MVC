using System.Threading.Tasks;
using EmployeeHrSystem.Controllers;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.mvc.Tests
{
    [TestFixture]
    public class AccountControllerTests
    {
        [Test]
        public void Login_Get_ReturnsView_WhenNotAuthenticated()
        {
            var ctrl = new AccountController(new FakeAccountService())
            {
                ControllerContext = TestHelper.CreateContext()
            };

            var result = ctrl.Login();
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Login_Post_Invalid_ReturnsView()
        {
            var ctrl = new AccountController(new FakeAccountService())
            {
                ControllerContext = TestHelper.CreateContext()
            };

            var result = await ctrl.Login("", "", "");
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public async Task Login_Post_Valid_Redirects()
        {
            var ctrl = new AccountController(new FakeAccountService())
            {
                ControllerContext = TestHelper.CreateContext()
            };

            var result = await ctrl.Login("valid", "pass", "Employee");
            Assert.IsInstanceOf<RedirectToActionResult>(result);
        }
    }
}
