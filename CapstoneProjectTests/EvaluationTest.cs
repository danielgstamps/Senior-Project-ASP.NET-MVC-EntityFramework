using Microsoft.VisualStudio.TestTools.UnitTesting;
using CapstoneProject.Models;

namespace CapstoneProjectTests
{
    [TestClass]
    public class EvaluationTest
    {
        private Evaluation evaluation;
        private Employee employee;

        [TestInitialize]
        public void TestInitialize()
        {
            this.evaluation = new Evaluation();
            this.evaluation.EvaluationID = 1;
            this.employee = new Employee();
            this.employee.EmployeeID = 2;
        }

        //[TestMethod]
        //public void TestEvaluationHasEmployeeID()
        //{
        //    this.evaluation.EmployeeID = 2;
        //    Assert.AreEqual(employee.EmployeeID, this.evaluation.EmployeeID);
        //}

        [TestMethod]
        public void TestEvaluationHasEmployee()
        {
            this.evaluation.Employee = this.employee;
            Assert.AreEqual(this.employee, this.evaluation.Employee);
        }

        //[TestMethod]
        //public void TestEvaluationDoesNotHaveEmployeeID()
        //{
        //    this.evaluation.EmployeeID = 3;
        //    Assert.AreNotEqual(this.employee.EmployeeID, this.evaluation.EmployeeID);
        //}

        [TestMethod]
        public void TestEvaluationDoesNotHaveEmployee()
        {
            this.evaluation.Employee = new Employee();
            Assert.AreNotSame(this.employee, this.evaluation.Employee);
        }
    }
}
