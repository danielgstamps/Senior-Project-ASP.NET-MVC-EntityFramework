using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CapstoneProject.Models;
using System.Collections.Generic;
using CapstoneProject.DAL;
using Moq;
using CapstoneProject.Controllers;
using System.Web.Mvc;

namespace CapstoneProjectTests
{
    [TestClass]
    public class QuestionsControllerTest
    {
        private List<Question> questions;
        private Mock<IUnitOfWork> mockUnitOfWork;
        private QuestionsController controller;

        [TestInitialize]
        public void Setup()
        {
            this.questions = new List<Question>()
            {
                new Question()
                {
                    QuestionID = 0,
                    QuestionText = "abc"
                },
                new Question()
                {
                    QuestionID = 1,
                    QuestionText = "xyz"
                }
            };
            this.mockUnitOfWork = new Mock<IUnitOfWork>();
            this.mockUnitOfWork.Setup(m => m.QuestionRepository.Get(null, null, "")).Returns(this.questions);
            this.controller = new QuestionsController();
            this.controller.UnitOfWork = mockUnitOfWork.Object;
            this.mockUnitOfWork.Object.QuestionRepository.Insert(questions[0]);
            this.mockUnitOfWork.Object.QuestionRepository.Insert(questions[1]);
        }

        [TestMethod]
        public void TestGet()
        {
            var result = this.mockUnitOfWork.Object.QuestionRepository.Get();
            Assert.AreEqual(this.questions, result);
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
            this.mockUnitOfWork.Setup(m => m.QuestionRepository.GetByID(1)).Returns(questions[1]);
            var result = this.mockUnitOfWork.Object.QuestionRepository.GetByID(1);
            Assert.AreEqual("xyz", result.QuestionText);
        }
    }
}
