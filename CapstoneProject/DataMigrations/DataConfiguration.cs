using System.Collections.Generic;
using CapstoneProject.Models;

namespace CapstoneProject.DataMigrations
{
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class DataConfiguration : DbMigrationsConfiguration<DAL.DataContext>
    {
        public DataConfiguration()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"DataMigrations";
        }

        protected override void Seed(DAL.DataContext context)
        {
            var cohortList = new List<Cohort>
            {
                new Cohort {CohortID = 1, Name = "Cohort 1", Employees = new List<Employee>()},
                new Cohort {CohortID = 2, Name = "Cohort 2", Employees = new List<Employee>()},
                new Cohort {CohortID = 3, Name = "Cohort 3", Employees = new List<Employee>()}
            };

            //cohortList.ForEach(s => context.Cohorts.Add(s));
            foreach (var cohort in cohortList)
            {
                if (!context.Cohorts.Any(c => c.CohortID == cohort.CohortID))
                {
                    context.Cohorts.Add(cohort);
                }
            }
            context.SaveChanges();

            var employeeList = new List<Employee>
            {
                new Employee {EmployeeID = 1, FirstName = "John", LastName = "Adams", CohortID = 1, Address = "5509 Barker Pl, Lanham, MD, 20706", Email = "johnadams@gmail.com", Phone = "678-444-5531", Evaluations = new List<Evaluation>()},
                new Employee {EmployeeID = 2, FirstName = "Stanley", LastName = "Johnson", CohortID = 1, Address = "1925 Pleasant Ave, Saint Charles, IL, 60174", Email = "stanleyjohnson@gmail.com", Phone = "678-555-5531", Evaluations = new List<Evaluation>()},
                new Employee {EmployeeID = 3, FirstName = "Ryan", LastName = "Anderson", CohortID = 2, Address = "1101 Charter Oak Ct., IL, 56869", Email = "ryananderson@gmail.com", Phone = "678-567-5678", Evaluations = new List<Evaluation>()},
                new Employee {EmployeeID = 4, FirstName = "Curtis", LastName = "Russell", CohortID = 2, Address = "5253 Trophy Dr, Fairfield, CA, 94534", Email = "curtisrussell@gmail.com", Phone = "678-888-9977", Evaluations = new List<Evaluation>()},
                new Employee {EmployeeID = 5, FirstName = "Phillip", LastName = "Davis", CohortID = 3, Address = "3805 Jadewood Drive North Chicago, IL 60064", Email = "PhillipGDavis@rhyta.com", Phone = "224-399-9854", Evaluations = new List<Evaluation>()},
                new Employee {EmployeeID = 6, FirstName = "Don", LastName = "Harmon", CohortID = 3, Address = "4031 Frum Street Smithville, TN 37166", Email = "DonSHarmon@rhyta.com", Phone = "615-273-2077", Evaluations = new List<Evaluation>()},
                new Employee {EmployeeID = 7, FirstName = "Deon", LastName = "Booker", CohortID = 1, Address = "1446 Circle Drive, Houston, TX 77020", Email = "DeonMBooker@armyspy.com", Phone = "832-551-1196", Evaluations = new List<Evaluation>()},
                new Employee {EmployeeID = 8, FirstName = "Joyce", LastName = "Clifton", CohortID = 2, Address = "4675 Cottrill Lane, Stlouis, MO 63101", Email = "JoyceDClifton@dayrep.com", Phone = "314-420-7760", Evaluations = new List<Evaluation>()}
            };
            //employeeList.ForEach(s => context.Employees.Add(s));
            foreach (var emp in employeeList)
            {
                if (!context.Employees.Any(e => e.EmployeeID == emp.EmployeeID))
                {
                    context.Employees.Add(emp);
                }
            }
            context.SaveChanges();

            var evaluationList = new List<Evaluation>
            {
                new Evaluation {EvaluationID = 1, EmployeeID = 1, Stage = "1", Type = 1, Categories = new List<Category>()},
                new Evaluation {EvaluationID = 2, EmployeeID = 2, Stage = "2", Type = 1, Categories = new List<Category>()},
                new Evaluation {EvaluationID = 3, EmployeeID = 3, Stage = "3", Type = 1, Categories = new List<Category>()},
                new Evaluation {EvaluationID = 4, EmployeeID = 4, Stage = "4", Type = 1, Categories = new List<Category>()},
                new Evaluation {EvaluationID = 5, EmployeeID = 5, Stage = "5", Type = 1, Categories = new List<Category>()},
                new Evaluation {EvaluationID = 6, EmployeeID = 6, Stage = "6", Type = 1, Categories = new List<Category>()},
                new Evaluation {EvaluationID = 7, EmployeeID = 7, Stage = "7", Type = 1, Categories = new List<Category>()},
                new Evaluation {EvaluationID = 8, EmployeeID = 8, Stage = "8", Type = 1, Categories = new List<Category>()},
            };
            //evaluationList.ForEach(s => context.Evaluations.Add(s));
            foreach (var eval in evaluationList)
            {
                if (!context.Evaluations.Any(e => e.EvaluationID == eval.EvaluationID))
                {
                    context.Evaluations.Add(eval);
                }
            }
            context.SaveChanges();

            var categoryList = new List<Category>
            {
                new Category { CategoryID = 1, Name = "Category 1", Description = "Category 1", EvaluationID = 1, Questions = new List<Question>()},
                new Category { CategoryID = 2, Name = "Category 2", Description = "Category 2", EvaluationID = 2, Questions = new List<Question>()},
                new Category { CategoryID = 3, Name = "Category 3", Description = "Category 3", EvaluationID = 3, Questions = new List<Question>()},
            };
            // categoryList.ForEach(s => context.Categories.Add(s));
            foreach (var cat in categoryList)
            {
                if (!context.Categories.Any(c => c.CategoryID == cat.CategoryID))
                {
                    context.Categories.Add(cat);
                }
            }
            context.SaveChanges();

            var questionList = new List<Question>
            {
                new Question {QuestionID = 1, Comment = "", QuestionText = "I am never late for work", Categories = new List<Category>()},
                new Question {QuestionID = 2, Comment = "", QuestionText = "I get along with my coworkers", Categories = new List<Category>()},
                new Question {QuestionID = 3, Comment = "", QuestionText = "I complete projects early", Categories = new List<Category>()}
            };
            //questionList.ForEach(s => context.Questions.Add(s));
            foreach (var question in questionList)
            {
                if (!context.Questions.Any(q => q.QuestionID == question.QuestionID))
                {
                    context.Questions.Add(question);
                }
            }
            context.SaveChanges();
        }
    }
}
