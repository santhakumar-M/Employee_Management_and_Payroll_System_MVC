using EmployeeHrSystem.Controllers;
using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;

namespace EmployeeManagement.mvc.Tests
{
    [TestFixture]
    public class HomeControllerTests
    {
        [Test]
        public void Index_ReturnsView()
        {
            var ctrl = new HomeController();
            var result = ctrl.Index();
            Assert.IsInstanceOf<ViewResult>(result);
        }

        [Test]
        public void Privacy_ReturnsView()
        {
            var ctrl = new HomeController();
            var result = ctrl.Privacy();
            Assert.IsInstanceOf<ViewResult>(result);
        }
    }
}
