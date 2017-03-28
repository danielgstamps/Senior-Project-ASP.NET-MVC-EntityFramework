using System;
using CapstoneProject.Models;
using Type = CapstoneProject.Models.Type;

namespace CapstoneProject.DAL
{
    public class UnitOfWork : IDisposable
    {
        private DataContext context = new DataContext();

        private GenericRepository<Answer> answerRepository;
        private GenericRepository<Category> categoryRepository;
        private GenericRepository<Cohort> cohortRepository;
        private GenericRepository<Employee> employeeRepository;
        private GenericRepository<Evaluation> evaluationRepository;
        private GenericRepository<Question> questionRepository;
        private GenericRepository<Stage> stageRepository;
        private GenericRepository<Models.Type> typeRepository; // Type's namespace must be explicitly called to avoid ambiguity with System.Type

        private bool disposed = false;

        public GenericRepository<Answer> AnsweRepository
        {
            get
            {
                if (this.answerRepository == null)
                {
                    this.answerRepository = new GenericRepository<Answer>(this.context);
                }
                return this.answerRepository;
            }
        }

        public GenericRepository<Category> CategoryRepository
        {
            get
            {
                if (this.categoryRepository == null)
                {
                    this.categoryRepository = new GenericRepository<Category>(this.context);
                }
                return this.categoryRepository;
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

        public GenericRepository<Question> QuestionRepository
        {
            get
            {
                if (this.questionRepository == null)
                {
                    this.questionRepository = new GenericRepository<Question>(this.context);   
                }
                return this.questionRepository;
            }
        }

        public GenericRepository<Stage> StageRepository
        {
            get
            {
                if (this.stageRepository == null)
                {
                    this.stageRepository = new GenericRepository<Stage>(this.context);
                }
                return this.stageRepository;
            }
        }

        public GenericRepository<Type> TypeRepository
        {
            get
            {
                if (this.typeRepository == null)
                {
                    this.typeRepository = new GenericRepository<Type>(this.context);
                }
                return this.typeRepository;
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