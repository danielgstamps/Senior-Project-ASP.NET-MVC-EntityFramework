using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CapstoneProject.Controllers;
using CapstoneProject.DAL;
using CapstoneProject.Models;
using CapstoneProjectTests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CapstoneProjectTests
{
    [TestClass]
    public class EmployeesControllerTest
    {
        private EmployeesController controller;
        private InMemoryEmployeeRepository repo;

        /*private static EmployeesController GetEmployeesController(IEmployeeRepository repository)
        {
            EmployeesController controller = new EmployeesController(repository);

            controller.ControllerContext = new ControllerContext()
            {
                Controller = controller,
                RequestContext = new RequestContext(new MockHttpContext(), new RouteData())
            };
            return controller;
        }

        private static EmployeesController GetEmployeesController(GenericRepository<Employee> repository)
        {
            EmployeesController controller = new EmployeesController(repository);

            controller.ControllerContext = new ControllerContext()
            {
                Controller = controller,
                RequestContext = new RequestContext(new MockHttpContext(), new RouteData())
            };

            return controller;
        }*/


        private class MockHttpContext : HttpContextBase
        {
            private readonly IPrincipal _user = new GenericPrincipal(
                     new GenericIdentity("someUser"), null /* roles */);

            public override IPrincipal User
            {
                get
                {
                    return _user;
                }
                set
                {
                    base.User = value;
                }
            }
        }

        [TestInitialize]
        public void Initialize()
        {
            //Arrange
            repo = new InMemoryEmployeeRepository();
            //controller = GetEmployeesController(repo);
            controller.Create(new Employee()
            {
                EmployeeID = 1,
                FirstName = "Angela",
                LastName = "Martin",
                Email = "amartin@mailinator.com",
                Address = "123 Scranton Ave, New York, NY 123456",
                Phone = "(770) 123-1234",
                CohortID = null,
                Cohort = null,
                SupervisorID = null,
                Supervisor = null,
                ManagerID = null,
                Manager = null,
                Evaluations = null
            });
        }

        [TestMethod]
        public void TestCreate()
        {
            controller.ModelState.AddModelError("", "Invalid model state");
            ViewResult result = controller.Create() as ViewResult;
            Assert.AreEqual("Create", result.ViewName);
        }

        [TestMethod]
        public void TestReturnCorrectEmployee()
        {
            var employee = repo.GetEmployeeByID(1);
            Assert.AreEqual("Angela", employee.FirstName);
        }

        [TestMethod]
        public void TestEmployeesIndex()
        {
            ViewResult result = controller.Index() as ViewResult;
            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public void TestEmployeesDetails()
        {
            ViewResult result = controller.Details(1) as ViewResult;
            Assert.AreEqual("Details", result.ViewName);
        }

        [TestMethod]
        public void TestEmployeesEdit()
        {
            ViewResult result = controller.Edit(1) as ViewResult;
            Assert.AreEqual("Edit", result.ViewName);
        }

        [TestMethod]
        public void TestEmployeesDelete()
        {
            ViewResult result = controller.Delete(1) as ViewResult;
            Assert.AreEqual("Delete", result.ViewName);
        }
    }
}
