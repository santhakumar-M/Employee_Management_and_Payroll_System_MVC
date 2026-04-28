using System.Collections.Generic;
using System.Threading.Tasks;
using EmployeeHrSystem.Controllers;
using EmployeeHrSystem.Models;
using NUnit.Framework;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.mvc.Tests
{
    [TestFixture]
    public class EmployeeControllerTests
    {
        [Test]
        public async Task Index_ReturnsViewWithEmployees()
        {
            var ctrl = new EmployeeController(new FakeEmployeeService(), new FakeDepartmentService(), new FakeAccountService())
            {
                ControllerContext = TestHelper.CreateContext()
            };

            var result = await ctrl.Index();
            Assert.IsInstanceOf<ViewResult>(result);
            var view = result as ViewResult;
            Assert.IsInstanceOf<List<Employee>>(view?.Model);
        }

        [Test]
        public async Task Attendance_NonExisting_ReturnsNotFound()
        {
            var ctrl = new EmployeeController(new FakeEmployeeService(), new FakeDepartmentService(), new FakeAccountService())
            {
                ControllerContext = TestHelper.CreateContext()
            };

            var result = await ctrl.Attendance(999, null);
            Assert.IsInstanceOf<NotFoundResult>(result);
        }
    }
}
