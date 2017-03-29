using CapstoneProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CapstoneProject.DAL
{
    public interface IUnitOfWork
    {
        GenericRepository<Employee> EmployeeRepository { get; }

        GenericRepository<Evaluation> EvaluationRepository { get; }

        GenericRepository<Cohort> CohortRepository { get; }

        void Save();

        void Dispose();
    }
}
