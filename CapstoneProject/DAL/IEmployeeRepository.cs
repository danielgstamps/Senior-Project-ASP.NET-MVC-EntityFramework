using System;
using System.Collections.Generic;
using CapstoneProject.Models;

namespace CapstoneProject.DAL
{
    /// <summary>
    /// Provides methods for any repos that interact with the employee table.
    /// Inherets from IDisposable.
    /// </summary>
    /// <seealso cref="System.IDisposable" />
    public interface IEmployeeRepository : IDisposable
    {
        void InsertEmployee(Employee employee);

        IEnumerable<Employee> GetEmployees();

        Employee GetEmployeeByID(int? employeeID);

        void UpdateEmployee(Employee employee);

        void DeleteEmployee(int employeeID);

        void Save();

    }
}
