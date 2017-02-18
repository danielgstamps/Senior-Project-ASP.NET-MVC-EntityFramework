using System;
using CapstoneProject.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CapstoneProjectTests
{
    [TestClass]
    public class CohortTest
    {
        [TestMethod]
        public void TestCohortHasName()
        {
            Cohort cohort = new Cohort();
            var name = "My Cohort";
            cohort.Name = name;
            Assert.AreEqual(name, cohort.Name);
        }

        [TestMethod]
        public void TestCohortDoesNotHaveName()
        {
            Cohort cohort = new Cohort();
            var name = "My Cohort";
            var wrongName = "The Cohort";
            cohort.Name = name;
            Assert.AreNotEqual(wrongName, cohort.Name);
        }
    }
}
