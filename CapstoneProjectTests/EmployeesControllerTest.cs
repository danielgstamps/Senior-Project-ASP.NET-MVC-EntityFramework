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

        private static EmployeesController GetEmployeesController(IEmployeeRepository repository)
        {
            EmployeesController controller = new EmployeesController(repository);

            controller.ControllerContext = new ControllerContext()
            {
                Controller = controller,
                RequestContext = new RequestContext(new MockHttpContext(), new RouteData())
            };
            return controller;
        }


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
            controller = GetEmployeesController(repo);
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

        /*[TestMethod]
        public void TestCreate()
        {
            var result = controller.Create() as ViewResult;
            Assert.AreEqual("Create", result.ViewName);
        }*/

        [TestMethod]
        public void TestEmployeesIndex()
        {
            //Act
            ViewResult result = (ViewResult) controller.Index();
            //Assert
            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public void TestEmployeesDetails()
        {

            ViewResult result = (ViewResult) controller.Details(1);

            Assert.AreEqual("Details", result.ViewName);
        }

        [TestMethod]
        public void TestReturnCorrectEmployee()
        {
            var employee = repo.GetEmployeeByID(1);
            Assert.AreEqual("Angela", employee.FirstName);
        }


    }
}
