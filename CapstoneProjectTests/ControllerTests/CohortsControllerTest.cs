using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CapstoneProject.DAL;
using CapstoneProject.Controllers;
using System.Collections.Generic;
using System.Linq;
using CapstoneProject.Models;
using System.Web.Mvc;

namespace CapstoneProjectTests.ControllerTests
{
    [TestClass]
    public class CohortsControllerTest
    {
        private List<Cohort> cohorts;
        private List<Employee> employees;
        private List<Evaluation> evals;
        private Mock<IUnitOfWork> mockUnitOfWork;
        private CohortsController controller;
        private const int BadRequestStatusCode = 400;

        [TestInitialize]
        public void Setup()
        {
            evals = new List<Evaluation>
            {
                new Evaluation
                {
                    EvaluationID = 0,
                    OpenDate = DateTime.Today.AddDays(-10),
                    CloseDate = DateTime.Today.AddDays(10),
                    SelfAnswers = "123123",
                    CompletedDate = DateTime.Today.AddDays(-10),
                    Raters = new List<Rater>()
                },
                new Evaluation
                {
                    EvaluationID = 1,
                    OpenDate = DateTime.Today.AddDays(-10),
                    CloseDate = DateTime.Today.AddDays(10),
                    SelfAnswers = null,
                    CompletedDate = null,
                    Raters = new List<Rater>()
                }
            };
            employees = new List<Employee>
            {
                new Employee
                {
                    EmployeeID = 0,
                    FirstName = "Dwight",
                    LastName = "Schrute",
                    CohortID = 0,
                    Evaluations = evals
                },
                new Employee
                {
                    EmployeeID = 1,
                    FirstName = "Angela",
                    LastName = "Martin",
                    CohortID = 0,
                    Evaluations = new List<Evaluation>()
                },
                new Employee
                {
                    EmployeeID = 2,
                    FirstName = "Andrew",
                    LastName = "Bernard",
                    CohortID = 0,
                    Evaluations = new List<Evaluation>()
                }
            };

            cohorts = new List<Cohort>
            {
                new Cohort
                {
                    CohortID = 0,
                    Name = "Sales",
                    Type1Assigned = false, 
                    Type2Assigned = false,
                    Employees = employees
                    
                },
                new Cohort
                {
                    CohortID = 1,
                    Name = "Accounting",
                    Type1Assigned = false,
                    Type2Assigned = false
                },
                new Cohort
                {
                    CohortID = 2,
                    Name = "Human Resources",
                    Type1Assigned = false,
                    Type2Assigned = false
                }
            };

            mockUnitOfWork = new Mock<IUnitOfWork>();
            controller = new CohortsController();
            controller.UnitOfWork = mockUnitOfWork.Object;
            mockUnitOfWork.Setup(
                m => m.CohortRepository.Get(null, null, "")).Returns(
                cohorts);
            foreach (var cohort in cohorts)
            {
                mockUnitOfWork.Object.CohortRepository.Insert(cohort);
            }
        }

        [TestMethod]
        public void TestGet()
        {
            var result = mockUnitOfWork.Object.CohortRepository.Get().ToList();
            for (var i = 0; i < cohorts.Count; i++)
            {
                Assert.AreEqual(cohorts[i], result[i]);
            }
        }

        [TestMethod]
        public void TestGetById()
        {
            mockUnitOfWork.Setup(m => m.CohortRepository.GetByID(0)).Returns(cohorts[0]);
            var result = mockUnitOfWork.Object.CohortRepository.GetByID(0);
            Assert.AreEqual("Sales", result.Name);
        }

        [TestMethod]
        public void TestIndexGet()
        {
            var result = controller.Index() as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public void TestDetails()
        {
            mockUnitOfWork.Setup(m => m.CohortRepository.GetByID(0)).Returns(cohorts[0]);
            var result = controller.Details(0) as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Details", result.ViewName);
        }

        [TestMethod]
        public void TestDetailsViewData()
        {
            mockUnitOfWork.Setup(m => m.CohortRepository.GetByID(0)).Returns(cohorts[0]);
            var result = controller.Details(0) as ViewResult;
            Assert.IsNotNull(result);

            var cohort = (Cohort)result.ViewData.Model;
            Assert.AreEqual(0, cohort.CohortID);
        }

        [TestMethod]
        public void TestGetCreateReturnsView()
        {
            mockUnitOfWork.Setup(m => m.CohortRepository.GetByID(0)).Returns(cohorts[0]);
            var result = controller.Create() as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Create", result.ViewName);
        }

        [TestMethod]
        public void TestPostCreateRedirectsToIndex()
        {
            mockUnitOfWork.Setup(m => m.CohortRepository.GetByID(0)).Returns(cohorts[0]);
            var newCohort = new Cohort
            {
                CohortID = 10,
                Name = "TestRedirect"
            };

            var result = controller.Create(newCohort) as RedirectToRouteResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void TestPostCreateInsertsNewObject()
        {
            mockUnitOfWork.Setup(m => m.CohortRepository.GetByID(0)).Returns(cohorts[0]);
            var newCohort = new Cohort
            {
                CohortID = 999,
                Name = "TestInsert"
            };

            controller.Create(newCohort);
            mockUnitOfWork.Verify(u => u.CohortRepository.Insert(newCohort), Times.Once);
        }

        [TestMethod]
        public void TestPostCreateInvalidReturnsView()
        {
            mockUnitOfWork.Setup(m => m.CohortRepository.GetByID(0)).Returns(cohorts[0]);
            controller.ModelState.AddModelError("error", "some error");

            var result = controller.Create(null) as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Create", result.ViewName);
        }

        [TestMethod]
        public void TestGetDeleteReturnsView()
        {
            mockUnitOfWork.Setup(m => m.CohortRepository.GetByID(0)).Returns(cohorts[0]);
            var result = controller.Delete(0) as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Delete", result.ViewName);
        }

        [TestMethod]
        public void TestGetDeleteNullIdReturnsBadRequest()
        {
            mockUnitOfWork.Setup(m => m.CohortRepository.GetByID(0)).Returns(cohorts[0]);
            var result = controller.Delete(null) as HttpStatusCodeResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(BadRequestStatusCode, result.StatusCode);
        }

        [TestMethod]
        public void TestGetDeleteInvalidIdReturnsBadRequest()
        {
            mockUnitOfWork.Setup(m => m.CohortRepository.GetByID(0)).Returns(cohorts[0]);
            var result = controller.Delete(100) as HttpStatusCodeResult;
            Assert.IsNotNull(result);
            Assert.AreEqual(BadRequestStatusCode, result.StatusCode);
        }

        [TestMethod]
        public void TestPostDeleteReturnsView()
        {
            mockUnitOfWork.Setup(m => m.CohortRepository.GetByID(0)).Returns(cohorts[0]);
            mockUnitOfWork.Setup(m => m.EvaluationRepository.GetByID(0)).Returns(evals[0]);
            var result = controller.DeleteConfirmed(0) as RedirectToRouteResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("Index", result.RouteValues["action"]);
        }

        [TestMethod]
        public void TestPostDeleteRemovesEmployeeFromCohort()
        {
            mockUnitOfWork.Setup(m => m.CohortRepository.GetByID(0)).Returns(cohorts[0]);
            mockUnitOfWork.Setup(m => m.EvaluationRepository.GetByID(0)).Returns(evals[0]);
            Assert.IsNotNull(employees[0].CohortID);
            var result = controller.DeleteConfirmed(0);
            Assert.IsNotNull(result);
            Assert.IsNull(employees[0].CohortID);
        }

        [TestMethod]
        public void TestPostDeleteDeletesIncompleteEval()
        {
            mockUnitOfWork.Setup(m => m.CohortRepository.GetByID(0)).Returns(cohorts[0]);
            mockUnitOfWork.Setup(m => m.EvaluationRepository.GetByID(0)).Returns(evals[0]);
            var result = controller.DeleteConfirmed(0) as RedirectToRouteResult;
            Assert.IsNotNull(result);
            mockUnitOfWork.Verify(u => u.EvaluationRepository.Delete(evals[1]), Times.Once);
        }

        [TestMethod]
        public void TestPostDeleteDoesNotDeleteCompleteEval()
        {
            mockUnitOfWork.Setup(m => m.CohortRepository.GetByID(0)).Returns(cohorts[0]);
            mockUnitOfWork.Setup(m => m.EvaluationRepository.GetByID(0)).Returns(evals[0]);
            var result = controller.DeleteConfirmed(0) as RedirectToRouteResult;
            Assert.IsNotNull(result);
            mockUnitOfWork.Verify(u => u.EvaluationRepository.Delete(evals[0]), Times.Never);
        }

        [TestMethod]
        public void TestPostDeleteDeletesCohort()
        {
            mockUnitOfWork.Setup(m => m.CohortRepository.GetByID(0)).Returns(cohorts[0]);
            mockUnitOfWork.Setup(m => m.EvaluationRepository.GetByID(0)).Returns(evals[0]);
            var result = controller.DeleteConfirmed(0);
            Assert.IsNotNull(result);
            mockUnitOfWork.Verify(u => u.CohortRepository.Delete(cohorts[0]), Times.Once);
        }

        //[TestMethod]
        //public void TestEdit()
        //{
        //    this.mockUnitOfWork.Setup(m => m.CohortRepository.GetByID(0)).Returns(cohorts[0]);
        //    var result = this.controller.Edit(0) as ViewResult;
        //    var resultVm = result?.Model as Cohort;

        //    if (resultVm != null) Assert.AreEqual(cohorts[0].Name, resultVm.Name);
        //    if (result != null) Assert.AreEqual("Edit", result.ViewName);
        //}
    }
}