using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using CapstoneProject.Controllers;
using CapstoneProject.DAL;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CapstoneProjectTests
{
    [TestClass]
    public class EmployeesControllerTest
    {
        private EmployeesController controller;

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

        /*[TestInitialize]
        public void Initialize()
        {
            controller = new EmployeesController();
        }

        [TestMethod]
        public void TestCreate()
        {
            var result = controller.Create() as ViewResult;
            Assert.AreEqual("Create", result.ViewName);
        }*/

        [TestMethod]
        public void TestEmployeesIndex()
        {
            var controller = GetEmployeesController(new InMemoryRepository());
        }
    }
}
