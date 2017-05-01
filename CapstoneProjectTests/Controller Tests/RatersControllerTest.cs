using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CapstoneProject.Models;
using CapstoneProject.Controllers;
using CapstoneProject.DAL;
using System.Collections.Generic;
using Moq;

namespace CapstoneProjectTests.Controller_Tests
{
    [TestClass]
    public class RatersControllerTest
    {
        private List<Rater> raters;
        private Mock<IUnitOfWork> mockUnitOfWork;
        private RatersController controller;

        [TestInitialize]
        public void Setup()
        {
            this.raters = new List<Rater>
            {
                new Rater
                {
                    RaterID = 1,
                    Name = "Michael Scarn",
                    Role = "Spy",
                    Email = "mscarn@mailinator.com",
                    Answers = "5,1,5"
                },
                new Rater
                {
                    RaterID = 2,
                    Name = "John Adams",
                    Role = "Spy",
                    Email = "jadam@mailinator.com",
                    Answers = "2,1,3"
                },
                new Rater
                {
                    RaterID = 3,
                    Name = "Alex Jefferson",
                    Role = "Spy",
                    Email = "ajefferson@mailinator.com",
                    Answers = "4,1,5"
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
        }

        [TestMethod]
        public void TestAssignRaters()
        {

        }

        [TestMethod]
        public void TestConfirmRaters()
        {

        }

        [TestMethod]
        public void TestNotifyRater()
        {

        }

        [TestMethod]
        public void TestRaterCleanup()
        {

        }

        [TestMethod]
        public void TestRaterPrompt()
        {

        }

        [TestMethod]
        public void TestReplaceRater()
        {

        }
    }
}
