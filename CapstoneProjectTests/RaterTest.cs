using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CapstoneProject.Models;

namespace CapstoneProjectTests
{
    [TestClass]
    public class RaterTest
    {
        private Rater rater;
        private Evaluation eval;

        [TestInitialize]
        public void Setup()
        {
            this.eval = new Evaluation
            {
                EvaluationID = 1
            };
            this.rater = new Rater
            {
                RaterID = 2,
                FirstName = "Michael",
                LastName = "Scarn",
                Role = "Spy",
                Email = "mscarn@mailinator.com",
                Answers = "5,1,5",
                EvaluationID = this.eval.EvaluationID,
                Evaluation = this.eval
            };
        }

        [TestMethod]
        public void TestRaterID()
        {
            Assert.AreEqual(2, this.rater.RaterID);
        }

        [TestMethod]
        public void TestRaterFirstName()
        {
            Assert.AreEqual("Michael", this.rater.FirstName);
        }

        [TestMethod]
        public void TestRaterLastname()
        {
            Assert.AreEqual("Scarn", this.rater.LastName);
        }

        [TestMethod]
        public void TestRaterRole()
        {
            Assert.AreEqual("Spy", this.rater.Role);
        }

        [TestMethod]
        public void TestRaterEmail()
        {
            Assert.AreEqual("mscarn@mailinator.com", this.rater.Email);
        }

        [TestMethod]
        public void TestRaterAnswers()
        {
            Assert.AreEqual("5", this.rater.Answers.Split(',')[0]);
            Assert.AreEqual("1", this.rater.Answers.Split(',')[1]);
            Assert.AreEqual("5", this.rater.Answers.Split(',')[2]);
        }

        [TestMethod]
        public void TestRaterEval()
        {
            Assert.AreSame(this.eval, this.rater.Evaluation);
        }

        [TestMethod]
        public void TestRaterEvalID()
        {
            Assert.AreEqual(1, this.rater.Evaluation.EvaluationID);
        }
    }
}
