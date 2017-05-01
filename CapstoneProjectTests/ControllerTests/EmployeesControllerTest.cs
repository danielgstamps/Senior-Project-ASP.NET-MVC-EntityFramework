using System.Web.Mvc;
using CapstoneProject.Controllers;
using CapstoneProject.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CapstoneProject.DAL;
using Moq;
using System.Collections.Generic;

namespace CapstoneProjectTests.ControllerTests
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
            employees = new List<Employee>
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
            mockUnitOfWork = new Mock<IUnitOfWork>();
            controller = new EmployeesController();
            controller.UnitOfWork = mockUnitOfWork.Object;
            mockUnitOfWork.Setup(
                m => m.EmployeeRepository.Get(null, null, "")).Returns(
                employees);
            foreach (var employee in employees)
            {
                mockUnitOfWork.Object.EmployeeRepository.Insert(employee);
            }
        }

        [TestMethod]
        public void TestGet()
        {
            var result = mockUnitOfWork.Object.EmployeeRepository.Get() as List<Employee>;
            Assert.AreEqual("Dwight", result[0].FirstName);
        }

        [TestMethod]
        public void TestIndex()
        {
            var result = controller.Index() as ViewResult;
            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public void TestGetById()
        {
            mockUnitOfWork.Setup(m => m.EmployeeRepository.GetByID(1)).Returns(employees[1]);
            var result = mockUnitOfWork.Object.EmployeeRepository.GetByID(1);
            Assert.AreEqual("Angela", result.FirstName);
        }

        [TestMethod]
        public void TestDetails()
        {
            mockUnitOfWork.Setup(m => m.EmployeeRepository.GetByID(0)).Returns(employees[0]);
            var result = controller.Details(0) as ViewResult;
            Assert.AreEqual("Details", result.ViewName);
        }
        
        [TestMethod]
        public void TestDelete()
        {
            mockUnitOfWork.Setup(m => m.EmployeeRepository.Delete(2));
            mockUnitOfWork.Object.EmployeeRepository.Delete(2);
            mockUnitOfWork.Verify(u => u.EmployeeRepository.Delete(2), Times.Once);
        }

        [TestMethod]
        public void TestUpdate()
        {
            var employeeToUpdate = this.mockUnitOfWork.Object.EmployeeRepository.GetByID(0);
            mockUnitOfWork.Setup(m => m.EmployeeRepository.Update(employeeToUpdate));
            mockUnitOfWork.Object.EmployeeRepository.Update(employeeToUpdate);
            mockUnitOfWork.Verify(m => m.EmployeeRepository.Update(employeeToUpdate), Times.Once);
        }

        [TestMethod]
        public void TestEdit()
        {
            mockUnitOfWork.Setup(m => m.EmployeeRepository.GetByID(0)).Returns(employees[0]);
            var result = controller.Edit(0) as ViewResult;
            if (result == null) return;
            var resultModel = result.Model as Employee;
            Assert.AreEqual("Edit", result.ViewName);
            if (resultModel == null) return;
            Assert.AreEqual(employees[0].Address, resultModel.Address);
            Assert.AreEqual(employees[0].Email, resultModel.Email);
            Assert.AreEqual(employees[0].FirstName, resultModel.FirstName);
            Assert.AreEqual(employees[0].LastName, resultModel.LastName);
            Assert.AreEqual(employees[0].Phone, resultModel.Phone);
        }
    }
}