using System.Web.Mvc;
using CapstoneProject.Controllers;
using CapstoneProject.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CapstoneProject.DAL;
using Moq;
using System.Collections.Generic;
using System.Web.UI.WebControls;

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
            this.employees = new List<Employee>
            {
                new Employee
                {
                    EmployeeID = 0,
                    FirstName = "Dwight",
                    LastName = "Schrute"
                },
                new Employee
                {
                    EmployeeID = 1,
                    FirstName = "Angela",
                    LastName = "Martin"
                },
                new Employee
                {
                    EmployeeID = 2,
                    FirstName = "Andrew",
                    LastName = "Bernard"
                }
            };
            this.mockUnitOfWork = new Mock<IUnitOfWork>();
            this.controller = new EmployeesController();
            this.controller.UnitOfWork = mockUnitOfWork.Object;
            this.mockUnitOfWork.Setup(
                m => m.EmployeeRepository.Get(null, null, "")).Returns(
                employees);
            foreach (var employee in this.employees)
            {
                this.mockUnitOfWork.Object.EmployeeRepository.Insert(employee);
            }
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
        
        [TestMethod]
        public void TestDelete()
        {
            this.mockUnitOfWork.Setup(m => m.EmployeeRepository.Delete(2));

            this.mockUnitOfWork.Object.EmployeeRepository.Delete(2);

            this.mockUnitOfWork.Verify(u => u.EmployeeRepository.Delete(2), Times.Once);
        }

        [TestMethod]
        public void TestUpdate()
        {
            var employeeToUpdate = this.mockUnitOfWork.Object.EmployeeRepository.GetByID(0);
            this.mockUnitOfWork.Setup(m => m.EmployeeRepository.Update(employeeToUpdate));

            this.mockUnitOfWork.Object.EmployeeRepository.Update(employeeToUpdate);

            this.mockUnitOfWork.Verify(m => m.EmployeeRepository.Update(employeeToUpdate), Times.Once);
        }

        [TestMethod]
        public void TestEdit()
        {
            var employeeToEdit = this.mockUnitOfWork.Object.EmployeeRepository.GetByID(0);
            var result = this.controller.Edit(employeeToEdit);

            Assert.AreEqual("Edit", result);
        }
    }
}
