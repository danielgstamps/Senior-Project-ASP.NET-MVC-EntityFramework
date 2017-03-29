using Microsoft.VisualStudio.TestTools.UnitTesting;
using CapstoneProject.Models;
using System.Collections.Generic;

namespace CapstoneProjectTests
{
    [TestClass]
    public class QuestionTest
    {
        private Category category;
        private Question question;

        [TestInitialize]
        public void TestInitialize()
        {
            this.category = new Category();
            this.question = new Question();
            this.category.Questions = new List<Question>();
            this.category.Questions.Add(this.question);
            //this.question.Categories = new List<Category>();
            //this.question.Categories.Add(this.category);
        }

        [TestMethod]
        public void TestCategoryHasQuestion()
        {
            Assert.IsTrue(this.category.Questions.Contains(this.question));
        }

        [TestMethod]
        public void TestCategoryDoesNotHaveQuestion()
        {
            Assert.IsFalse(this.category.Questions.Contains(new Question()));
        }

        [TestMethod]
        public void TestQuestionHasCategory()
        {
            //Assert.IsTrue(this.question.Categories.Contains(this.category));
        }

        [TestMethod]
        public void TestQuestionDoesNotHaveCategory()
        {
            //Assert.IsFalse(this.question.Categories.Contains(new Category()));
        }
    }
}
