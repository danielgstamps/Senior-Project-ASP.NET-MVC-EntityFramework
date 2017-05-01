using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CapstoneProject.Models;
using Type = CapstoneProject.Models.Type;

namespace CapstoneProjectTests.ModelTests
{
    [TestClass]
    public class EvaluationTest
    {
        private Evaluation evaluation;
        private Employee employee;
        private Type type;
        private Stage stage;
        private ICollection<Rater> raters;

        [TestInitialize]
        public void Setup()
        {
            this.raters = new List<Rater>
            {
                new Rater
                {
                    RaterID = 1
                },
                new Rater
                {
                    RaterID = 2
                },
                new Rater
                {
                    RaterID = 3
                }
            };

            this.stage = new Stage
            {
                StageID = 4
            };

            this.type = new Type
            {
                TypeID = 5
            };

            this.employee = new Employee
            {
                EmployeeID = 6
            };

            this.evaluation = new Evaluation
            {
                EvaluationID = 7,
                EmployeeID = this.employee.EmployeeID,
                Employee = this.employee,
                TypeID = this.type.TypeID,
                Type = this.type,
                StageID = this.stage.StageID,
                Stage = this.stage,
                OpenDate = new DateTime(2017, 12, 21),
                CloseDate = new DateTime(2017, 12, 31),
                SelfAnswers = "1,2,3",
                Raters = this.raters
            };
        }

        [TestMethod]
        public void TestEvaluationID()
        {
            Assert.AreEqual(7, this.evaluation.EvaluationID);
        }

        [TestMethod]
        public void TestEmployeeID()
        {
            Assert.AreEqual(this.employee.EmployeeID, this.evaluation.EmployeeID);
        }

        [TestMethod]
        public void TestTypeID()
        {
            Assert.AreEqual(this.type.TypeID, this.evaluation.TypeID);
        }

        [TestMethod]
        public void TestStageID()
        {
            Assert.AreEqual(this.stage.StageID, this.evaluation.StageID);
        }

        [TestMethod]
        public void TestOpenDate()
        {
            Assert.AreEqual(2017, this.evaluation.OpenDate.Year);
            Assert.AreEqual(12, this.evaluation.OpenDate.Month);
            Assert.AreEqual(21, this.evaluation.OpenDate.Day);
        }

        [TestMethod]
        public void TestCloseDate()
        {
            Assert.AreEqual(2017, this.evaluation.CloseDate.Year);
            Assert.AreEqual(12, this.evaluation.CloseDate.Month);
            Assert.AreEqual(31, this.evaluation.CloseDate.Day);
        }

        [TestMethod]
        public void TestSelfAnswers()
        {
            var answers = this.evaluation.SelfAnswers.Split(',');
            var testAnswer = 1;
            foreach (var answer in answers)
            {
                Assert.AreEqual(testAnswer, int.Parse(answer));
                testAnswer++;
            }
        }

        [TestMethod]
        public void TestRaters()
        {
            var ID = 1;
            foreach (var rater in this.evaluation.Raters)
            {
                Assert.AreEqual(ID, rater.RaterID);
                ID++;
            }
        }
    }
}
