using System;
using CapstoneProject.Models;

namespace CapstoneProject.DAL
{
    public class UnitOfWork : IDisposable
    {
        private DataContext context = new DataContext();
        private GenericRepository<Employee> employeeRepository;
        private GenericRepository<Cohort> cohortRepository;
        private GenericRepository<Evaluation> evaluationRepository;
        private GenericRepository<Answer> answeRepository;
        private GenericRepository<Category> categoryRepository;
        private GenericRepository<Question> questionRepository;
        private GenericRepository<Stage> stageRepository;
        private GenericRepository<CapstoneProject.Models.Type> typeRepository; // Type's namespace must be explicitly called to avoid ambiguity with System.Type
        private bool disposed = false;

        public GenericRepository<Employee> EmployeeRepository
        {
            get
            {
                if (this.employeeRepository == null)
                {
                    this.employeeRepository = new GenericRepository<Employee>(this.context);
                }
                return employeeRepository;
            }
        }

        public GenericRepository<Cohort> CohortRepository
        {
            get
            {
                if (this.cohortRepository == null)
                {
                    this.cohortRepository = new GenericRepository<Cohort>(this.context);
                }
                return cohortRepository;
            }
        }

        public GenericRepository<Evaluation> EvaluationRepository
        {
            get
            {
                if (this.evaluationRepository == null)
                {
                    this.evaluationRepository = new GenericRepository<Evaluation>(this.context);
                }
                return this.evaluationRepository;
            }
        } 

        public void Save()
        {
            context.SaveChanges();
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