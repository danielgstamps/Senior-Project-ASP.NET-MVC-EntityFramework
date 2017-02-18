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
        private Cohort cohort;

        [TestInitialize]
        public void TestInitialize()
        {
            createJimHalpert();
            createMichaelScott();
            createDwightSchrute();
            addEmployeesToCollection();
            createCohort();
            setManagers();
            setSupervisors();
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

        private void createDwightSchrute()
        {
            dwightSchrute = new Employee();
            dwightSchrute.EmployeeID = 2;
            dwightSchrute.FirstName = "Dwight";
            dwightSchrute.LastName = "Schrute";
            dwightSchrute.Address = "345 Moonlight Drive, New York, NY 345678";
            dwightSchrute.Email = "dschrute@dundermifflin.com";
            dwightSchrute.Phone = "(345) 678-9012";
        }

        private void setSupervisors()
        {
            jimHalpert.SupervisorID = michaelScott.EmployeeID;
            jimHalpert.Supervisor = michaelScott;
            dwightSchrute.SupervisorID = jimHalpert.EmployeeID;
            dwightSchrute.Supervisor = jimHalpert;
        }

        private void setManagers()
        {
            jimHalpert.ManagerID = michaelScott.EmployeeID;
            jimHalpert.Manager = michaelScott;
            dwightSchrute.ManagerID = michaelScott.EmployeeID;
            dwightSchrute.Manager = michaelScott;
        }

        private void addEmployeesToCollection()
        {
            employees = new List<Employee>();
            employees.Add(jimHalpert);
            employees.Add(michaelScott);
            employees.Add(dwightSchrute);
        }

        private void createCohort()
        {
            cohort = new Cohort();
            cohort.CohortID = 0;
            cohort.Name = "Cohort 0";
            cohort.Employees = employees;
            jimHalpert.CohortID = cohort.CohortID;
            jimHalpert.Cohort = cohort;
            michaelScott.CohortID = cohort.CohortID;
            michaelScott.Cohort = cohort;
            dwightSchrute.CohortID = cohort.CohortID;
            dwightSchrute.Cohort = cohort;
        }

        [TestMethod]
        public void TestEmployeeHasCohortID()
        {
            Assert.AreEqual(cohort.CohortID, jimHalpert.CohortID);
        }

        [TestMethod]
        public void TestEmployeeHasCohort()
        {
            Assert.AreSame(cohort, jimHalpert.Cohort);
        }

        [TestMethod]
        public void TestJimHalpertHasMichaelScottAsSupervisor()
        {
            Assert.AreSame(this.michaelScott, this.jimHalpert.Supervisor);
        }

        [TestMethod]
        public void TestJimHalpertSupervisorIDIsMichaelScottEmployeeID()
        {
            Assert.AreEqual(michaelScott.EmployeeID, jimHalpert.SupervisorID);
        }

        [TestMethod]
        public void TestJimHalpertManagerIsMichaelScott()
        {
            Assert.AreSame(this.michaelScott, this.jimHalpert.Manager);
        }

        [TestMethod]
        public void TestJimHalpertManagerIDIsMichaelScottEmployeeID()
        {
            Assert.AreEqual(jimHalpert.ManagerID, michaelScott.EmployeeID);
        }
    }
}
