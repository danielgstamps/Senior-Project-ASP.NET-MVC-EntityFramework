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
            this.evaluations = new List<Evaluation>
            {
                new Evaluation
                {
                    EvaluationID = 0,
                    SelfAnswers = "1,2,3"
                },
                new Evaluation
                {
                    EvaluationID = 1,
                    SelfAnswers = "2,3,1"
                },
                new Evaluation
                {
                    EvaluationID = 2,
                    SelfAnswers = "3,1,2"
                }
            };
            this.mockUnitOfWork = new Mock<IUnitOfWork>();
            this.controller = new EvaluationsController();
            this.controller.UnitOfWork = this.mockUnitOfWork.Object;
            this.mockUnitOfWork.Setup(
                m => m.EvaluationRepository.Get(null, null, "")).Returns(
                this.evaluations);
            foreach (var eval in this.evaluations)
            {
                this.mockUnitOfWork.Object.EvaluationRepository.Insert(eval);
            }
        }

        [TestMethod]
        public void TestGet()
        {
            var result = this.mockUnitOfWork.Object.EvaluationRepository.Get();
            Assert.AreEqual(this.evaluations, result);
        }

        [TestMethod]
        public void TestGetByID()
        {
            this.mockUnitOfWork.Setup(m => m.EvaluationRepository.GetByID(0)).Returns(this.evaluations[0]);
            var result = this.mockUnitOfWork.Object.EvaluationRepository.GetByID(0);
            Assert.AreEqual("1,2,3", result.SelfAnswers);
        }

        [TestMethod]
        public void TestDelete()
        {
            this.mockUnitOfWork.Setup(m => m.EvaluationRepository.Delete(2));

            this.mockUnitOfWork.Object.EvaluationRepository.Delete(2);

            this.mockUnitOfWork.Verify(u => u.EvaluationRepository.Delete(2), Times.Once);
        }

        [TestMethod]
        public void TestUpdate()
        {
            var evalToUpdate = this.mockUnitOfWork.Object.EvaluationRepository.GetByID(0);
            this.mockUnitOfWork.Setup(m => m.EvaluationRepository.Update(evalToUpdate));

            this.mockUnitOfWork.Object.EvaluationRepository.Update(evalToUpdate);

            this.mockUnitOfWork.Verify(m => m.EvaluationRepository.Update(evalToUpdate), Times.Once);
        }

        [TestMethod]
        public void TestEdit()
        {

        }
    }
}
