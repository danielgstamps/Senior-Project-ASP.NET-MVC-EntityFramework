using System.Web.Mvc;
using CapstoneProject.Controllers;
using CapstoneProject.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CapstoneProject.DAL;
using Moq;
using System.Collections.Generic;

namespace CapstoneProjectTests
{
    [TestClass]
    public class EmployeesControllerTest
    {
        private List<Employee> employees;
        private Mock<IUnitOfWork> mockUnitOfWork;
        private EmployeesController controller;

        [TestInitialize]
        public void Setup()
        {
            this.employees = new List<Employee>()
            {
                new Employee()
                {
                    EmployeeID = 0,
                    FirstName = "Dwight",
                    LastName = "Schrute"
                },
                new Employee()
                {
                    EmployeeID = 1,
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
        public void TestGet()
        {
            var result = this.mockUnitOfWork.Object.EmployeeRepository.Get() as List<Employee>;
            Assert.AreEqual("Dwight", result[0].FirstName);
        }

        [TestMethod]
        public void TestIndex()
        {
            var result = this.controller.Index() as ViewResult;
            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public void TestGetByID()
        {
            this.mockUnitOfWork.Setup(m => m.EmployeeRepository.GetByID(1)).Returns(employees[1]);
            var result = this.mockUnitOfWork.Object.EmployeeRepository.GetByID(1);
            Assert.AreEqual("Angela", result.FirstName);
        }

        [TestMethod]
        public void TestDetails()
        {
            this.mockUnitOfWork.Setup(m => m.EmployeeRepository.GetByID(0)).Returns(employees[0]);
            var result = this.controller.Details(0) as ViewResult;
            Assert.AreEqual("Details", result.ViewName);
        }
    }
}
