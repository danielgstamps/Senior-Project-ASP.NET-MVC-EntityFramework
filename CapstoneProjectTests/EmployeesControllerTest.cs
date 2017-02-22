using System;
using System.Web.Mvc;
using CapstoneProject.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CapstoneProjectTests
{
    [TestClass]
    public class EmployeesControllerTest
    {
        private EmployeesController controller;

        [TestInitialize]
        public void Initialize()
        {
            controller = new EmployeesController();
        }

        [TestMethod]
        public void TestCreate()
        {
            var result = controller.Create() as ViewResult;
            Assert.AreEqual("Create", result.ViewName);
        }
    }
}
