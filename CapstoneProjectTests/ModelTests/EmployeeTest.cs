using System.Collections.Generic;
using System.Threading;
using CapstoneProject.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CapstoneProjectTests.ModelTests
{
    [TestClass]
    public class EmployeeTest
    {
        private Employee employee;
        private Cohort cohort;
        private ICollection<Evaluation> evaluations;

        [TestInitialize]
        public void Setup()
        {
            this.cohort = new Cohort
            {
                CohortID = 2
            };
            this.evaluations = new List<Evaluation>
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
            this.employee = new Employee
            {
                EmployeeID = 1,
                FirstName = "Saul",
                LastName = "Goodman",
                Email = "sgoodman@mailinator.com",
                Address = "123 Killian Ln, Albuquerque, NM 123456",
                Phone = "(123) 123-1234",
                CohortID = this.cohort.CohortID,
                Cohort = this.cohort,
                Evaluations = this.evaluations
            };
        }

        [TestMethod]
        public void TestEmployeeID()
        {
            Assert.AreEqual(1, this.employee.EmployeeID);
        }

        [TestMethod]
        public void TestEmployeeFirstname()
        {
            Assert.AreEqual("Saul", this.employee.FirstName);
        }

        [TestMethod]
        public void TestEmployeeLastName()
        {
            Assert.AreEqual("Goodman", this.employee.LastName);
        }

        [TestMethod]
        public void TestEmployeeEmail()
        {
            Assert.AreEqual("sgoodman@mailinator.com", this.employee.Email);
        }

        [TestMethod]
        public void TestEmployeeAddress()
        {
            Assert.AreEqual("123 Killian Ln, Albuquerque, NM 123456", this.employee.Address);
        }

        [TestMethod]
        public void TestEmployeePhone()
        {
            Assert.AreEqual("(123) 123-1234", this.employee.Phone);
        }

        [TestMethod]
        public void TestEmployeeCohort()
        {
            Assert.AreEqual(2, this.employee.CohortID);
        }

        [TestMethod]
        public void TestEmployeeEvaluations()
        {
            var ID = 1;
            foreach (var evaluation in this.employee.Evaluations)
            {
                Assert.AreEqual(ID, evaluation.EvaluationID);
                ID++;
            }
        }

    }
}
