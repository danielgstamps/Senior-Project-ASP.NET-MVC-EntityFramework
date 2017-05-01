using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using CapstoneProject.DAL;
using CapstoneProject.Controllers;
using System.Collections.Generic;
using System.Linq;
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
                new Cohort
                {
                    CohortID = 0,
                    Name = "Sales"
                },
                new Cohort
                {
                    CohortID = 1,
                    Name = "Accounting"
                },
                new Cohort()
                {
                    CohortID = 2,
                    Name = "Human Resources"
                }
            };
            this.mockUnitOfWork = new Mock<IUnitOfWork>();
            this.controller = new CohortsController();
            this.controller.UnitOfWork = mockUnitOfWork.Object;
            this.mockUnitOfWork.Setup(
                m => m.CohortRepository.Get(null, null, "")).Returns(
                this.cohorts);
            foreach (var cohort in this.cohorts)
            {
                this.mockUnitOfWork.Object.CohortRepository.Insert(cohort);
            }
        }

        [TestMethod]
        public void TestGet()
        {
            var result = this.mockUnitOfWork.Object.CohortRepository.Get().ToList();
            for (var i = 0; i < this.cohorts.Count; i++)
            {
                Assert.AreEqual(this.cohorts[i], result[i]);
            }
        }

        [TestMethod]
        public void TestGetByID()
        {
            this.mockUnitOfWork.Setup(m => m.CohortRepository.GetByID(0)).Returns(cohorts[0]);
            var result = this.mockUnitOfWork.Object.CohortRepository.GetByID(0);
            Assert.AreEqual("Sales", result.Name);
        }

        [TestMethod]
        public void TestIndex()
        {
            var result = this.controller.Index() as ViewResult;
            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public void TestDetails()
        {
            this.mockUnitOfWork.Setup(m => m.CohortRepository.GetByID(0)).Returns(cohorts[0]);
            var result = this.controller.Details(0) as ViewResult;
            Assert.AreEqual("Details", result.ViewName);
        }

        [TestMethod]
        public void TestDelete()
        {
            this.mockUnitOfWork.Setup(m => m.CohortRepository.Delete(2));
            this.mockUnitOfWork.Object.CohortRepository.Insert(cohorts[2]);
            
            this.mockUnitOfWork.Object.CohortRepository.Delete(2);

            this.mockUnitOfWork.Verify(u =>u.CohortRepository.Delete(2), Times.Once);
        }

        [TestMethod]
        public void TestUpdate()
        {
            var cohortToUpdate = this.mockUnitOfWork.Object.CohortRepository.GetByID(0);
            this.mockUnitOfWork.Setup(m => m.CohortRepository.Update(cohortToUpdate));
            
            this.mockUnitOfWork.Object.CohortRepository.Update(cohortToUpdate);

            this.mockUnitOfWork.Verify(m => m.CohortRepository.Update(cohortToUpdate), Times.Once);
        }

        [TestMethod]
        public void TestEdit()
        {
            this.mockUnitOfWork.Setup(m => m.CohortRepository.GetByID(0)).Returns(cohorts[0]);
            var result = this.controller.Edit(0) as ViewResult;
            var resultVm = result?.Model as Cohort;

            if (resultVm != null) Assert.AreEqual(cohorts[0].Name, resultVm.Name);
            if (result != null) Assert.AreEqual("Edit", result.ViewName);
        }
    }
}