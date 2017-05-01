using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CapstoneProject.Models;
using CapstoneProject.Controllers;
using CapstoneProject.DAL;
using System.Collections.Generic;
using Moq;
using CapstoneProject.ViewModels;
using System.Web.Mvc;

namespace CapstoneProjectTests
{
    [TestClass]
    public class RatersControllerTest
    {
        private List<Rater> raters;
        private Evaluation eval;
        private Mock<IUnitOfWork> mockUnitOfWork;
        private RatersController controller;
        private AssignRatersViewModel ratersVm;

        [TestInitialize]
        public void Setup()
        {
            this.eval = new Evaluation
            {
                EvaluationID = 1
            };
            this.raters = new List<Rater>
            {
                new Rater
                {
                    RaterID = 1,
                    Name = "Michael Scarn",
                    Role = "Spy",
                    Email = "mscarn@mailinator.com",
                    Answers = "5,1,5",
                    EvaluationID = this.eval.EvaluationID,
                    Evaluation = this.eval
                },
                new Rater
                {
                    RaterID = 2,
                    Name = "John Adams",
                    Role = "Spy",
                    Email = "jadam@mailinator.com",
                    Answers = "2,1,3",
                    EvaluationID = this.eval.EvaluationID,
                    Evaluation = this.eval
                },
                new Rater
                {
                    RaterID = 3,
                    Name = "Alex Jefferson",
                    Role = "Spy",
                    Email = "ajefferson@mailinator.com",
                    Answers = "4,1,5",
                    EvaluationID = this.eval.EvaluationID,
                    Evaluation = this.eval
                }
            };
            this.mockUnitOfWork = new Mock<IUnitOfWork>();
            this.controller = new RatersController();
            this.controller.UnitOfWork = mockUnitOfWork.Object;
            this.mockUnitOfWork.Setup(
                m => m.RaterRepository.Get(null, null, "")).Returns(
                raters);
            foreach (var rater in this.raters)
            {
                this.mockUnitOfWork.Object.RaterRepository.Insert(rater);
            }

            this.ratersVm = new AssignRatersViewModel
            {
                EvalId = this.eval.EvaluationID,
                Raters = this.raters
            };
        }

        [TestMethod]
        public void TestAssignRaters()
        {
            this.mockUnitOfWork.Setup(m => m.RaterRepository.GetByID(1)).Returns(raters[1]);
            var result = this.controller.AssignRaters(ratersVm.EvalId) as ViewResult;
            
            Assert.AreEqual("AssignRaters", result.ViewName);
        }

        [TestMethod]
        public void TestConfirmRaters()
        {
            this.mockUnitOfWork.Setup(m => m.RaterRepository.GetByID(1)).Returns(raters[1]);
            var result = this.controller.ConfirmRaters(ratersVm.EvalId) as ViewResult;

            Assert.AreEqual("ConfirmRaters", result.ViewName);
        }

        [TestMethod]
        public void TestNotifyRater()
        {
            this.mockUnitOfWork.Setup(m => m.RaterRepository.GetByID(1)).Returns(raters[1]);
            var result = this.controller.NotifyRater(1) as ViewResult;

            Assert.AreEqual("NotifyRater", result.ViewName);
        }

        [TestMethod]
        public void TestRaterCleanup()
        {
            this.mockUnitOfWork.Setup(m => m.RaterRepository.GetByID(2)).Returns(raters[2]);
            var result = this.controller.RaterCleanup(2) as ViewResult;

            Assert.AreEqual("RaterCleanup", result.ViewName);
        }

        [TestMethod]
        public void TestReplaceRater()
        {
            this.mockUnitOfWork.Setup(m => m.RaterRepository.GetByID(3)).Returns(raters[3]);
            var result = this.controller.RaterCleanup(3) as ViewResult;

            Assert.AreEqual("ReplaceRater", result.ViewName);
        }
    }
}
