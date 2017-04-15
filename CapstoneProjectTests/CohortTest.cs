using System.Collections.Generic;
using CapstoneProject.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CapstoneProjectTests
{
    [TestClass]
    public class CohortTest
    {
        private Cohort cohort;
        private ICollection<Employee> employees;

        [TestInitialize]
        public void Setup()
        {
            this.employees = new List<Employee>
            {
                new Employee
                {
                    EmployeeID = 1
                },
                new Employee
                {
                    EmployeeID = 2
                },
                new Employee
                {
                    EmployeeID = 3
                }
            };
            this.cohort = new Cohort
            {
                CohortID = 1,
                Name = "Sales Team",
                Employees = this.employees,
                Type1Assigned = false,
                Type2Assigned = false
            };
        }

        [TestMethod]
        public void TestCohortID()
        {
            Assert.AreEqual(1, cohort.CohortID);
        }

        [TestMethod]
        public void TestCohortName()
        {
            Assert.AreEqual("Sales Team", this.cohort.Name);
        }

        [TestMethod]
        public void TestCohortEmployees()
        {
            var ID = 1;
            foreach (var employee in this.cohort.Employees)
            {
                Assert.AreEqual(ID, employee.EmployeeID);
                ID++;
            }
        }

        [TestMethod]
        public void TestCohortType1AssignedFalse()
        {
            Assert.IsFalse(this.cohort.Type1Assigned);
        }

        [TestMethod]
        public void TestCohortType1AssignedTrue()
        {
            this.cohort.Type1Assigned = true;
            Assert.IsTrue(this.cohort.Type1Assigned);
        }

        [TestMethod]
        public void TestCohortType2AssignedFalse()
        {
            Assert.IsFalse(this.cohort.Type2Assigned);
        }

        [TestMethod]
        public void TestCohortType2AssignedTrue()
        {
            this.cohort.Type2Assigned = true;
            Assert.IsTrue(this.cohort.Type2Assigned);
        }
    }
}
