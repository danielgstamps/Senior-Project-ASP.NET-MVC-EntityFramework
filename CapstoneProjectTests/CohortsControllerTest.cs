using System.Web.Mvc;
using CapstoneProject.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CapstoneProjectTests
{
    [TestClass]
    public class CohortsControllerTest
    {
        private CohortsController controller;

        [TestInitialize]
        public void Initialize()
        {
            this.controller = new CohortsController();
        }

        [TestMethod]
        public void TestCreate()
        {
            var result = controller.Create() as ViewResult;
            Assert.AreEqual("Create", result.ViewName);
        }
    }
}
