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
            cohorts = new List<Cohort>
            {
                new Cohort
                {
                    CohortID = 0,
                    Name = "Sales",
                    Type1Assigned = false, 
                    Type2Assigned = false
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
            var result = this.mockUnitOfWork.Object.CohortRepository.Get().ToList();
            for (var i = 0; i < this.cohorts.Count; i++)
            {
                Assert.AreEqual(this.cohorts[i], result[i]);
            }
        }

        [TestMethod]
        public void TestGetById()
        {
            this.mockUnitOfWork.Setup(m => m.CohortRepository.GetByID(0)).Returns(cohorts[0]);
            var result = mockUnitOfWork.Object.CohortRepository.GetByID(0);
            Assert.AreEqual("Sales", result.Name);
        }

        [TestMethod]
        public void TestIndex()
        {
            var result = controller.Index() as ViewResult;
            Assert.AreEqual("Index", result.ViewName);
        }

        [TestMethod]
        public void TestDetails()
        {
            mockUnitOfWork.Setup(m => m.CohortRepository.GetByID(0)).Returns(cohorts[0]);
            var result = controller.Details(0) as ViewResult;
            Assert.AreEqual("Details", result.ViewName);
        }

        [TestMethod]
        public void TestDetailsViewData()
        {
            this.mockUnitOfWork.Setup(m => m.CohortRepository.GetByID(0)).Returns(cohorts[0]);
            var result = controller.Details(0) as ViewResult;
            var cohort = (Cohort)result.ViewData.Model;
            Assert.AreEqual(0, cohort.CohortID);
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