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
        GenericRepository<Category> CategoryRepository { get; }

        GenericRepository<Employee> EmployeeRepository { get; }

        GenericRepository<Evaluation> EvaluationRepository { get; }

        GenericRepository<Cohort> CohortRepository { get; }

        GenericRepository<Question> QuestionRepository { get; }

        GenericRepository<Models.Type> TypeRepository { get; }

        GenericRepository<Stage> StageRepository { get; }

        void Save();

        void Dispose();
    }
}
