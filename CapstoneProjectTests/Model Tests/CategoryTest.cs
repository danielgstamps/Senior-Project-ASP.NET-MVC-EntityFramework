using System.Collections.Generic;
using CapstoneProject.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CapstoneProjectTests
{
    [TestClass]
    public class CategoryTest
    {
        private Category category;
        private ICollection<Question> questions;
        private Type type;

        [TestInitialize]
        public void Setup()
        {
            this.type = new Type
            {
                TypeID = 1,
                TypeName = "Type One"
            };
            this.questions = new List<Question>
            {
                new Question
                {
                    QuestionID = 1,
                    QuestionText = "abc"
                },
                new Question
                {
                    QuestionID = 2,
                    QuestionText = "bcd"
                },
                new Question
                {
                    QuestionID = 3,
                    QuestionText = "cde"
                }
            };
            this.category = new Category
            {
                CategoryID = 1,
                Name = "Category One",
                Description = "Description",
                Questions = this.questions,
                TypeID = this.type.TypeID,
                Type = this.type
            };
        }

        [TestMethod]
        public void TestCategoryID()
        {
            Assert.AreEqual(1, this.category.CategoryID);
        }

        [TestMethod]
        public void TestCategoryName()
        {
            Assert.AreEqual("Category One", this.category.Name);
        }

        [TestMethod]
        public void TestCategoryDescription()
        {
            Assert.AreEqual("Description", this.category.Description);
        }

        [TestMethod]
        public void TestCategoryQuestions()
        {
            var ID = 1;
            foreach (var question in this.category.Questions)
            {
                Assert.AreEqual(ID, question.QuestionID);
                ID++;
            }
        }

        [TestMethod]
        public void TestCategoryTypeID()
        {
            Assert.AreEqual(1, this.category.TypeID);
        }
    }
}
