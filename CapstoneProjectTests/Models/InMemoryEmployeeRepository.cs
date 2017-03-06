using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CapstoneProject.DAL;
using CapstoneProject.Models;

namespace CapstoneProjectTests.Models
{
    public class InMemoryEmployeeRepository : IEmployeeRepository
    {
        private List<Employee> db = new List<Employee>();

        public void InsertEmployee(Employee employee)
        {
            db.Add(employee);
        }

        public IEnumerable<Employee> GetEmployees()
        {
            return db.ToList();
        }

        public Employee GetEmployeeByID(int? employeeID)
        {
            return db.FirstOrDefault(e => e.EmployeeID == employeeID);
        }

        public void UpdateEmployee(Employee employee)
        {
            foreach (Employee currentEmployee in db)
            {
                if (currentEmployee.EmployeeID == employee.EmployeeID)
                {
                    db.Remove(currentEmployee);
                    db.Add(employee);
                    break;
                }
            }
        }

        public void DeleteEmployee(int employeeID)
        {
            db.Remove(GetEmployeeByID(employeeID));
        }

        public void Save()
        {
            // not needed
        }

        public void Dispose()
        {
            // not needed
        }
    }
}
