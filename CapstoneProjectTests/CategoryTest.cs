using System;
using CapstoneProject.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CapstoneProjectTests
{
    [TestClass]
    public class CategoryTest
    {
        [TestMethod]
        public void TestName()
        {
            Category category = new Category();
            var name = "Category 1";
            category.Name = name;
            Assert.AreEqual(name, category.Name);
        }

        [TestMethod]
        public void TestCategoryReferencesEvaluation()
        {
            Category category = new Category();
            Evaluation evaluation = new Evaluation();
            category.Evaluation = evaluation;
            Assert.AreSame(evaluation, category.Evaluation);
        }

        [TestMethod]
        public void TestCategoryDoesNotReferenceEvaluation()
        {
            Category category = new Category();
            Evaluation referencedEvaluation = new Evaluation();
            Evaluation nonReferencedEvaluation = new Evaluation();
            category.Evaluation = referencedEvaluation;
            Assert.AreNotSame(nonReferencedEvaluation, category.Evaluation);
        }

        [TestMethod]
        public void TestCategoryReferencesID()
        {
            Category category = new Category();
            Evaluation evaluation = new Evaluation();
            category.Evaluation = evaluation;
            Assert.AreEqual(evaluation.EvaluationID, category.EvaluationID);
        }

        [TestMethod]
        public void TestCategoryDoesNotReferenceID()
        {
            Category category = new Category();
            Evaluation referencedEvaluation = new Evaluation();
            referencedEvaluation.EvaluationID = 0;
            Evaluation nonReferencedEvaluation = new Evaluation();
            nonReferencedEvaluation.EvaluationID = 1;
            category.Evaluation = referencedEvaluation;
            Assert.AreNotEqual(nonReferencedEvaluation.EvaluationID, category.EvaluationID);
        }
    }
}
