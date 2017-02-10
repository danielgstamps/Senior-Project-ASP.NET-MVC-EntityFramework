using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CapstoneProject.Models
{
    public class Employee
    {
        public int EmployeeID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Phone { get; set; }
        public int CohortID { get; set; }
        public Cohort Cohort { get; set; }
        public int SupervisorID { get; set; }
        public Employee Supervisor { get; set; }
        public int ManagerID { get; set; }
        public Employee Manager { get; set; }
    }
}