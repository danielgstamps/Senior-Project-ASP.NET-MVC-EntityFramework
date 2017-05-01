using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CapstoneProject.Models;
using System.Collections.Generic;
using CapstoneProject.DAL;
using Moq;
using CapstoneProject.Controllers;
using System.Web.Mvc;

namespace CapstoneProjectTests.ControllerTests
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
                },
                new Question
                {
                    CategoryID = 2,
                    QuestionText = "lmn"
                }
            };
            this.mockUnitOfWork = new Mock<IUnitOfWork>();
            this.mockUnitOfWork.Setup(m => m.QuestionRepository.Get(null, null, "")).Returns(this.questions);
            this.controller = new QuestionsController();
            this.controller.UnitOfWork = mockUnitOfWork.Object;
            foreach (var question in this.questions)
            {
                this.mockUnitOfWork.Object.QuestionRepository.Insert(question);
            }
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

        [TestMethod]
        public void TestDelete()
        {
            this.mockUnitOfWork.Setup(m => m.QuestionRepository.Delete(It.IsAny<Question>()));

            this.mockUnitOfWork.Object.QuestionRepository.Delete(2);

            this.mockUnitOfWork.Verify(u => u.QuestionRepository.Delete(2));
        }

        [TestMethod]
        public void TestUpdate()
        {
            var questionToUpdate = this.mockUnitOfWork.Object.QuestionRepository.GetByID(0);
            this.mockUnitOfWork.Setup(m => m.QuestionRepository.Update(questionToUpdate));

            this.mockUnitOfWork.Object.QuestionRepository.Update(questionToUpdate);

            this.mockUnitOfWork.Verify(m => m.QuestionRepository.Update(questionToUpdate), Times.Once);
        }
    }
}
