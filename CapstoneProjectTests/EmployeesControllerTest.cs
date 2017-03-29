using System.Security.Principal;
using System.Web;
using System.Web.Mvc;
using CapstoneProject.Controllers;
using CapstoneProject.Models;
using CapstoneProjectTests.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CapstoneProject.DAL;
using System.Web.Routing;
using Moq;
using System.Collections.Generic;

namespace CapstoneProjectTests
{
    [TestClass]
    public class EmployeesControllerTest
    {
        private Mock<IUnitOfWork> mockUnitOfWork;
        private EmployeesController controller;

        [TestInitialize]
        public void Setup()
        {
            var employees = new List<Employee>()
            {
                new Employee()
                {
                    EmployeeID = 1,
                    FirstName = "Dwight",
                    LastName = "Schrute"
                },
                new Employee()
                {
                    EmployeeID = 2,
                    FirstName = "Angela",
                    LastName = "Martin"
                }
            };
            this.mockUnitOfWork = new Mock<IUnitOfWork>();
            this.mockUnitOfWork.Setup(m => m.EmployeeRepository.Get(null, null, "")).Returns(employees as List<Employee>);
            this.controller = new EmployeesController();
            this.controller.UnitOfWork = mockUnitOfWork.Object;
            this.mockUnitOfWork.Object.EmployeeRepository.Insert(employees[0]);
            this.mockUnitOfWork.Object.EmployeeRepository.Insert(employees[1]);
        }

        [TestMethod]
        public void TestIndex()
        {
            var index = this.mockUnitOfWork.Object.EmployeeRepository.Get() as List<Employee>;
            Assert.AreEqual("Dwight", index[0].FirstName);
        }
    }
}
