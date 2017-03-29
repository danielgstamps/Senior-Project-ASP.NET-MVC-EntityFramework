using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CapstoneProject.DAL;
using CapstoneProject.Controllers;
using System.Collections.Generic;
using CapstoneProject.Models;
using System.Web.Mvc;

namespace CapstoneProjectTests
{
    [TestClass]
    public class CohortsControllerTest
    {
        private List<Cohort> cohorts;
        private Mock<IUnitOfWork> mockUnitOfWork;
        private CohortsController controller;

        [TestInitialize]
        public void Setup()
        {
            this.cohorts = new List<Cohort>()
            {
                new Cohort()
                {
                    CohortID = 0,
                    Name = "Sales"
                },
                new Cohort()
                {
                    CohortID = 1,
                    Name = "Accounting"
                }
            };
            this.mockUnitOfWork = new Mock<IUnitOfWork>();
            this.mockUnitOfWork.Setup(m => m.CohortRepository.Get(null, null, "")).Returns(this.cohorts);
            this.controller = new CohortsController();
            this.controller.UnitOfWork = mockUnitOfWork.Object;
            this.mockUnitOfWork.Object.CohortRepository.Insert(cohorts[0]);
        }

        [TestMethod]
        public void TestGet()
        {
            var result = this.mockUnitOfWork.Object.CohortRepository.Get();
            Assert.AreEqual(0, cohorts[0].CohortID);
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
            this.mockUnitOfWork.Setup(m => m.CohortRepository.GetByID(1)).Returns(cohorts[1]);
            var result = this.mockUnitOfWork.Object.CohortRepository.GetByID(1);
            Assert.AreEqual("Accounting", result.Name);
        }

        [TestMethod]
        public void TestDetails()
        {
            this.mockUnitOfWork.Setup(m => m.CohortRepository.GetByID(0)).Returns(cohorts[0]);
            var result = this.controller.Details(0) as ViewResult;
            Assert.AreEqual("Details", result.ViewName);
        }
    }
}
