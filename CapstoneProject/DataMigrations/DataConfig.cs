using System;
using System.Collections.Generic;
using CapstoneProject.Models;
using CapstoneProject.DAL;
using System.Data.Entity.Migrations;
using System.Linq;

namespace CapstoneProject.DataMigrations
{

    internal sealed class DataConfig : DbMigrationsConfiguration<DataContext>
    {
        public DataConfig()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"DataMigrations";
        }

//        protected override void Seed(DataContext context)
//        {
//            var evaluationList = new List<Evaluation>
//            {
//                new Evaluation
//                {
//                    EvaluationID = 1,
//                   // TypeID = 1,
//                    StageID = 1,
//                    OpenDate = new DateTime(2017, 04, 03),
//                    ClosedDate = new DateTime(2017, 12, 31)
//                }
//            };
//            evaluationList.ForEach(s => context.Evaluations.Add(s));
//            //foreach (var eval in evaluationList)
//            //{
//            //    if (!context.Evaluations.Any(e => e.EvaluationID == eval.EvaluationID))
//            //    {
//            //        context.Evaluations.Add(eval);
//            //    }
//            //}
//            context.SaveChanges();          

//            var categories = new List<Category>
//            {
//                new Category
//                {
//                    CategoryID = 1,
//                    Name = "Company Culture",
//                    Description = "These questions concern your satisfaction with company culture",
//                    TypeID = 1
//                }
//            };
//            categories.ForEach(s => context.Categories.Add(s));
//            //foreach (var cat in categoryList)
//            //{
//            //    if (!context.Categories.Any(c => c.CategoryID == cat.CategoryID))
//            //    {
//            //        context.Categories.Add(cat);
//            //    }
//            //}
//            context.SaveChanges();

//            var questionList = new List<Question>
//            {
//                new Question
//                {
//                    QuestionID = 1,
//                    QuestionText = "I like the ping pong tables at my place of work",
//                    CategoryID = 1,
//                    Category = categories[0]
//                }
//            };
//            questionList.ForEach(s => context.Questions.Add(s));
//            //foreach (var question in questionList)
//            //{
//            //    if (!context.Questions.Any(q => q.QuestionID == question.QuestionID))
//            //    {
//            //        context.Questions.Add(question);
//            //    }
//            //}
//            context.SaveChanges();

//        }
    }
}
