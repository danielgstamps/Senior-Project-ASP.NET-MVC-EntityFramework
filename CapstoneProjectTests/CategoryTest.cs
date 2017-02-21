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
            var category = new Category();
            var name = "Category 1";
            category.Name = name;
            Assert.AreEqual(name, category.Name);
        }

        [TestMethod]
        public void TestCategoryReferencesEvaluation()
        {
            var category = new Category();
            var evaluation = new Evaluation();
            category.Evaluation = evaluation;
            Assert.AreSame(evaluation, category.Evaluation);
        }

        [TestMethod]
        public void TestCategoryDoesNotReferenceEvaluation()
        {
            var category = new Category();
            var referencedEvaluation = new Evaluation();
            var nonReferencedEvaluation = new Evaluation();
            category.Evaluation = referencedEvaluation;
            Assert.AreNotSame(nonReferencedEvaluation, category.Evaluation);
        }

        [TestMethod]
        public void TestCategoryReferencesID()
        {
            var category = new Category();
            var evaluation = new Evaluation();
            category.Evaluation = evaluation;
            Assert.AreEqual(evaluation.EvaluationID, category.EvaluationID);
        }

        [TestMethod]
        public void TestCategoryDoesNotReferenceID()
        {
            var category = new Category();
            var referencedEvaluation = new Evaluation();
            referencedEvaluation.EvaluationID = 0;
            var nonReferencedEvaluation = new Evaluation();
            nonReferencedEvaluation.EvaluationID = 1;
            category.Evaluation = referencedEvaluation;
            Assert.AreNotEqual(nonReferencedEvaluation.EvaluationID, category.EvaluationID);
        }
    }
}
