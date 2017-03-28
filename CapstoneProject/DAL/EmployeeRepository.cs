using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using CapstoneProject.Models;

namespace CapstoneProject.DAL
{
    public class EmployeeRepository : IEmployeeRepository, IDisposable
    {
        private DataContext context;
        private bool disposed;

        public EmployeeRepository(DataContext context)
        {
            this.context = context;
            this.disposed = false;
        }

        public IEnumerable<Employee> GetEmployees()
        {
            return this.context.Employees.ToList();
        }

        public Employee GetEmployeeByID(int? employeeID)
        {
            return context.Employees.Find(employeeID);
        }

        public void InsertEmployee(Employee employee)
        {
            this.context.Employees.Add(employee);
        }

        public void DeleteEmployee(int employeeID)
        {
            Employee employee = this.context.Employees.Find(employeeID);
            this.context.Employees.Remove(employee);
        }

        public void UpdateEmployee(Employee employee)
        {
            this.context.Entry(employee).State = EntityState.Modified;
        }

        public void Save()
        {
            this.context.SaveChanges();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!this.disposed)
            {
                if (disposing)
                {
                    context.Dispose();
                }
            }
            this.disposed = true;
        }
    }
}