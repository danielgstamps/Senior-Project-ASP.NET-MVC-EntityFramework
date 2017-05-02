using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using CapstoneProject.Models;
using CapstoneProject.DAL;
using Moq;
using CapstoneProject.Controllers;
using System.Web.Mvc;

namespace CapstoneProjectTests.ControllerTests
{
    [TestClass]
    public class EvaluationsControllerTest
    {
        private List<Evaluation> evals;
        private List<Stage> stages;
        private List<CapstoneProject.Models.Type> types;
        private Employee employee;
        private Cohort cohort;

        private Mock<IUnitOfWork> mockUnitOfWork;
        private EvaluationsController controller;

        [TestInitialize]
        public void Setup()
        {
            cohort = new Cohort()
            {
                CohortID = 0,
                Name = "Test",
                Type1Assigned = false,
                Type2Assigned = false
            };

            employee = new Employee
            {
                EmployeeID = 0,
                FirstName = "Dwight",
                LastName = "Schrute",
                CohortID = 0,
                Evaluations = evals,
                Cohort = cohort
            };

            stages = new List<Stage>
            {
                new Stage
                {
                    StageID = 0,
                    StageName = "Baseline",
                    Evals = new List<Evaluation>()
                },
                new Stage
                {
                    StageID = 1,
                    StageName = "Formative",
                    Evals = new List<Evaluation>()
                },
                new Stage
                {
                    StageID = 2,
                    StageName = "Summative",
                    Evals = new List<Evaluation>()
                }
            };

            types = new List<CapstoneProject.Models.Type>
            {
                new CapstoneProject.Models.Type
                {
                    TypeID = 0,
                    TypeName = "Type 1",
                    Categories = new List<Category>(),
                    Evals = new List<Evaluation>()
                },
                new CapstoneProject.Models.Type
                {
                    TypeID = 1,
                    TypeName = "Type 2",
                    Categories = new List<Category>(),
                    Evals = new List<Evaluation>()
                },
            };

            evals = new List<Evaluation>
            {
                new Evaluation
                {
                    EvaluationID = 0,
                    Employee = employee,
                    OpenDate = DateTime.Today.AddDays(-10),
                    CloseDate = DateTime.Today.AddDays(10),
                    SelfAnswers = "123123",
                    CompletedDate = DateTime.Today.AddDays(-10),
                    Raters = new List<Rater>(),
                },
                new Evaluation
                {
                    EvaluationID = 1,
                    Employee = employee,
                    OpenDate = DateTime.Today.AddDays(-10),
                    CloseDate = DateTime.Today.AddDays(10),
                    SelfAnswers = null,
                    CompletedDate = null,
                    Raters = new List<Rater>(),
                }
            };

            mockUnitOfWork = new Mock<IUnitOfWork>();
            controller = new EvaluationsController();
            controller.UnitOfWork = mockUnitOfWork.Object;
            mockUnitOfWork.Setup(
                m => m.EvaluationRepository.Get(null, null, "")).Returns(
                evals);
            foreach (var eval in evals)
            {
                mockUnitOfWork.Object.EvaluationRepository.Insert(eval);
            }
        }

        [TestMethod]
        public void TestGet()
        {
            var result = mockUnitOfWork.Object.EvaluationRepository.Get();
            Assert.AreEqual(evals, result);
        }

        [TestMethod]
        public void TestGetById()
        {
            mockUnitOfWork.Setup(m => m.EvaluationRepository.GetByID(0)).Returns(evals[0]);
            var result = mockUnitOfWork.Object.EvaluationRepository.GetByID(0);
            Assert.AreEqual("123123", result.SelfAnswers);
        }

        [TestMethod]
        public void TestGetAdminEvalsIndexReturnsView()
        {
            mockUnitOfWork.Setup(m => m.EvaluationRepository.GetByID(0)).Returns(evals[0]);
            mockUnitOfWork.Setup(m => m.EmployeeRepository.GetByID(0)).Returns(employee);
            var result = controller.AdminEvalsIndex() as ViewResult;
            Assert.IsNotNull(result);
            Assert.AreEqual("AdminEvalsIndex", result.ViewName);
        }

        //[TestMethod]
        //public void TestGetCreateReturnsView()
        //{
        //    mockUnitOfWork.Setup(m => m.EvaluationRepository.GetByID(0)).Returns(evals[0]);
        //    mockUnitOfWork.Setup(m => m.StageRepository.GetByID(0)).Returns(stages[0]);
        //    mockUnitOfWork.Setup(m => m.TypeRepository.GetByID(0)).Returns(types[0]);
        //    var result = controller.Create(0) as ViewResult;
        //    Assert.IsNotNull(result);
        //    Assert.AreEqual("Create", result.ViewName);
        //}


        [TestMethod]
        public void TestDelete()
        {
            mockUnitOfWork.Setup(m => m.EvaluationRepository.Delete(2));
            mockUnitOfWork.Object.EvaluationRepository.Delete(2);
            mockUnitOfWork.Verify(u => u.EvaluationRepository.Delete(2), Times.Once);
        }

        [TestMethod]
        public void TestUpdate()
        {
            var evalToUpdate = mockUnitOfWork.Object.EvaluationRepository.GetByID(0);
            mockUnitOfWork.Setup(m => m.EvaluationRepository.Update(evalToUpdate));
            mockUnitOfWork.Object.EvaluationRepository.Update(evalToUpdate);
            mockUnitOfWork.Verify(m => m.EvaluationRepository.Update(evalToUpdate), Times.Once);
        }
    }
}
