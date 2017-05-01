using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using CapstoneProject.Models;
using System.Collections.Generic;

namespace CapstoneProjectTests.ModelTests
{
    [TestClass]
    public class StageTest
    {
        private Stage stage;
        private List<Evaluation> evals;

        [TestInitialize]
        public void Setup()
        {
            this.evals = new List<Evaluation>
            {
                new Evaluation
                {
                    EvaluationID = 1
                },
                new Evaluation
                {
                    EvaluationID = 2
                },
                new Evaluation
                {
                    EvaluationID = 3
                }
            };

            this.stage = new Stage
            {
                StageID = 4,
                StageName = "Baseline",
                Evals = this.evals
            };
        }

        [TestMethod]
        public void TestStageID()
        {
            Assert.AreEqual(4, this.stage.StageID);
        }

        [TestMethod]
        public void TestStageName()
        {
            Assert.AreEqual("Baseline", this.stage.StageName);
        }

        [TestMethod]
        public void TestStageEvals()
        {
            Assert.AreSame(this.evals, this.stage.Evals);
        }

    }
}
