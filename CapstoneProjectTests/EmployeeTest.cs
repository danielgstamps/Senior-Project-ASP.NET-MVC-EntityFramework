using System;
using System.Collections.Generic;
using CapstoneProject.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace CapstoneProjectTests
{
    [TestClass]
    public class EmployeeTest
    {
        private Employee jimHalpert;
        private Employee michaelScott;
        private Employee dwightSchrute;
        private ICollection<Employee> employees;
        private Cohort cohort0;

        [TestInitialize]
        public void TestInitialize()
        {
            createJimHalpert();
            createMichaelScott();

            addEmployeesToCollection();

            createCohort();

            jimHalpert.CohortID = cohort0.CohortID;
            jimHalpert.Cohort = cohort0;
            michaelScott.CohortID = cohort0.CohortID;
            michaelScott.Cohort = cohort0;
        }

        private void createJimHalpert()
        {
            jimHalpert = new Employee();
            jimHalpert.EmployeeID = 0;
            jimHalpert.FirstName = "Jim";
            jimHalpert.LastName = "Halpert";
            jimHalpert.Address = "123 Columbia Drive, New York, NY 123456";
            jimHalpert.Email = "jhalpert@dundermifflin.com";
            jimHalpert.Phone = "(123) 456-780";
            jimHalpert.ManagerID = michaelScott.EmployeeID;
            jimHalpert.Manager = michaelScott;
            jimHalpert.SupervisorID = michaelScott.EmployeeID;
            jimHalpert.Supervisor = michaelScott;
        }

        private void createMichaelScott()
        {
            michaelScott = new Employee();
            michaelScott.EmployeeID = 1;
            michaelScott.FirstName = "Michael";
            michaelScott.LastName = "Scott";
            michaelScott.Address = "234 Sunshine Avenue, New York, NY 234567";
            michaelScott.Email = "mscott@dundermifflin.com";
            michaelScott.Phone = "(234) 567-8901";
        }

        private void addEmployeesToCollection()
        {
            employees = new List<Employee>();
            employees.Add(jimHalpert);
            employees.Add(michaelScott);
        }

        private void createCohort()
        {
            cohort0 = new Cohort();
            cohort0.CohortID = 0;
            cohort0.Name = "Cohort 0";
            cohort0.Employees = employees;
        }

        [TestMethod]
        public void TestEmployeeHasCohortID()
        {
            Assert.AreEqual(cohort0.CohortID, jimHalpert.CohortID);
        }

        [TestMethod]
        public void TestEmployeeHashCohort()
        {
            Assert.AreSame(cohort0, jimHalpert.Cohort);
        }
    }
}
