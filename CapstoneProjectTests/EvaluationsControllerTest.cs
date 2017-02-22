using System;
using System.Security.Cryptography.X509Certificates;
using System.Web.Mvc;
using CapstoneProject.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CapstoneProjectTests
{
    [TestClass]
    public class EvaluationsControllerTest
    {
        private EvaluationsController controller;

        [TestInitialize]
        public void Initialize()
        {
            controller = new EvaluationsController();
        }

        [TestMethod]
        public void TestCreate()
        {
            var result = controller.Create() as ViewResult;
            Assert.AreEqual("Create", result.ViewName);
        }
    }
}
