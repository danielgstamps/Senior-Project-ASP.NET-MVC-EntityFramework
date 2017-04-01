using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using CapstoneProject.Models;
using CapstoneProject.DAL;
using Moq;
using CapstoneProject.Controllers;
using System.Web.Mvc;

namespace CapstoneProjectTests
{
    [TestClass]
    public class EvaluationsControllerTest
    {
        private List<Evaluation> evaluations;
        private Mock<IUnitOfWork> mockUnitOfWork;
        private EvaluationsController controller;

        [TestInitialize]
        public void Setup()
        {
            this.evaluations = new List<Evaluation>()
            {
                new Evaluation()
                {
                    EvaluationID = 0
                },
                new Evaluation()
                {
                    EvaluationID = 1
                }
            };
            this.mockUnitOfWork = new Mock<IUnitOfWork>();
            this.mockUnitOfWork.Setup(m => m.EvaluationRepository.Get(null, null, "")).Returns(this.evaluations);
            this.controller = new EvaluationsController();
            this.controller.UnitOfWork = this.mockUnitOfWork.Object;
            this.mockUnitOfWork.Object.EvaluationRepository.Insert(evaluations[0]);
            this.mockUnitOfWork.Object.EvaluationRepository.Insert(evaluations[1]);
        }

        [TestMethod]
        public void TestGet()
        {
            var result = this.mockUnitOfWork.Object.EvaluationRepository.Get();
            Assert.AreEqual(this.evaluations, result);
        }

        //[TestMethod]
        //public void TestIndex()
        //{
        //    var result = this.controller.Index() as ViewResult;
        //    Assert.AreEqual("Index", result.ViewName);
        //}

       // [TestMethod]
        //public void TestGetByID()
        //{
        //    this.mockUnitOfWork.Setup(m => m.EvaluationRepository.GetByID(0)).Returns(this.evaluations[0]);
        //    var result = this.mockUnitOfWork.Object.EvaluationRepository.GetByID(0);
        //    Assert.AreEqual("test0", result.Comment);
        //}

        [TestMethod]
        public void TestDetails()
        {
            this.mockUnitOfWork.Setup(m => m.EvaluationRepository.GetByID(1)).Returns(this.evaluations[1]);
            var result = this.controller.Details(1) as ViewResult;
            Assert.AreEqual("Details", result.ViewName);
        }
    }
}
