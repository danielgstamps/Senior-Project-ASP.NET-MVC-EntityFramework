using System;
using System.Collections.Generic;
using CapstoneProject.Models;
using CapstoneProject.DAL;
using System.Data.Entity.Migrations;
using System.Linq;
using Type = CapstoneProject.Models.Type;

namespace CapstoneProject.DataMigrations
{

    internal sealed class DataConfig : DbMigrationsConfiguration<DataContext>
    {
        public DataConfig()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"DataMigrations";
        }

        protected override void Seed(DataContext context)
        {
            //foreach (var stage in stageList)
            //{
            //    if (!context.Stages.Any(s => s.StageID == stage.StageID))
            //    {
            //        context.Stages.Add(stage);
            //    }
            //}
            var evaluationList = new List<Evaluation>
            {
                new Evaluation
                {
                    EvaluationID = 1,
                    EmployeeID = 130,
                    TypeID = 1,
                    StageID = 20,
                    OpenDate = new DateTime(2017, 04, 03),
                    CloseDate = new DateTime(2017, 12, 31),
                    SelfAnswers = "1,2,3,4,5"
                },
                new Evaluation
                {
                    EvaluationID = 2,
                    EmployeeID = 129,
                    TypeID = 2,
                    StageID = 20,
                    OpenDate = new DateTime(2017, 04, 03),
                    CloseDate = new DateTime(2017, 12, 31),
                    SelfAnswers = "1,2,3,4,5,6,7,8,9,10",
                },
                new Evaluation
                {
                    EvaluationID = 3,
                    EmployeeID = 158,
                    TypeID = 1,
                    StageID = 20,
                    OpenDate = new DateTime(2017, 04, 03),
                    CloseDate = new DateTime(2017, 12, 31),
                    SelfAnswers = "12345"
                }
            };
            evaluationList.ForEach(s => context.Evaluations.Add(s));
            //foreach (var eval in evaluationList)
            //{
            //    if (!context.Evaluations.Any(e => e.EvaluationID == eval.EvaluationID))
            //    {
            //        context.Evaluations.Add(eval);
            //    }
            //}
            context.SaveChanges();

            var categories = new List<Category>
            {
                new Category
                {
                    CategoryID = 1,
                    Name = "Company Culture",
                    Description = "These questions concern your satisfaction with company culture",
                    TypeID = 1
                },
                new Category
                {
                    CategoryID = 2,
                    Name = "Company Ethics",
                    Description = "These questions concern your satisfaction with company ethics",
                    TypeID = 1
                },
                new Category
                {
                    CategoryID = 3,
                    Name = "Company Values",
                    Description = "These questions concern your satisfaction with company values",
                    TypeID = 1
                },
                new Category
                {
                    CategoryID = 4,
                    Name = "Company Traits",
                    Description = "These questions concern your satisfaction with company traits",
                    TypeID = 1
                },
                new Category
                {
                    CategoryID = 5,
                    Name = "Company Sanitation",
                    Description = "These questions concern your satisfaction with company sanitation",
                    TypeID = 1
                },
                new Category
                {
                    CategoryID = 6,
                    Name = "Company Communication",
                    Description = "These questions concern your satisfaction with company communication",
                    TypeID = 2
                },
                new Category
                {
                    CategoryID = 7,
                    Name = "Company Collaboration",
                    Description = "These questions concern your satisfaction with company teamwork",
                    TypeID = 2
                },
                new Category
                {
                    CategoryID = 8,
                    Name = "Company Regards",
                    Description = "These questions concern your satisfaction with company regards",
                    TypeID = 2
                },
                new Category
                {
                    CategoryID = 9,
                    Name = "Company Empowerment",
                    Description = "These questions concern your satisfaction with company decision making",
                    TypeID = 2
                },
                new Category
                {
                    CategoryID = 10,
                    Name = "Company Rules",
                    Description = "These questions concern your satisfaction with company rules",
                    TypeID = 2
                },
                new Category
                {
                    CategoryID = 11,
                    Name = "Company Policy",
                    Description = "These questions concern your satisfaction with company policy",
                    TypeID = 2
                },
                new Category
                {
                    CategoryID = 12,
                    Name = "Company Services",
                    Description = "These questions concern your satisfaction with company services",
                    TypeID = 2
                }
            };
           // categories.ForEach(s => context.Categories.Add(s));
            //foreach (var cat in categoryList)
            //{
            //    if (!context.Categories.Any(c => c.CategoryID == cat.CategoryID))
            //    {
            //        context.Categories.Add(cat);
            //    }
            //}
            context.SaveChanges();

            var questionList = new List<Question>
            {
                new Question
                {
                    QuestionID = 1,
                    QuestionText = "I like the ping pong tables at my place of work",
                    CategoryID = 1,
                    Category = categories[0]
                },
                new Question
                {
                    QuestionID = 2,
                    QuestionText = "I like the microwaves at my place of work",
                    CategoryID = 1,
                    Category = categories[0]
                },
                new Question
                {
                    QuestionID = 3,
                    QuestionText = "I like the restrooms at my place of work",
                    CategoryID = 1,
                    Category = categories[0]
                },
                new Question
                {
                    QuestionID = 4,
                    QuestionText = "I like the maids at my place of work",
                    CategoryID = 2,
                    Category = categories[1]
                },
                new Question
                {
                    QuestionID = 5,
                    QuestionText = "I like the black people at my place of work",
                    CategoryID = 2,
                    Category = categories[1]
                },
                new Question
                {
                    QuestionID = 6,
                    QuestionText = "I like the oxygen at my place of work",
                    CategoryID = 2,
                    Category = categories[1]
                },
                new Question
                {
                    QuestionID = 7,
                    QuestionText = "I like the tables at my place of work",
                    CategoryID = 3,
                    Category = categories[2]
                },
                new Question
                {
                    QuestionID = 8,
                    QuestionText = "I like the walls at my place of work",
                    CategoryID = 3,
                    Category = categories[2]
                },
                new Question
                {
                    QuestionID = 9,
                    QuestionText = "I like the managers at my place of work",
                    CategoryID = 3,
                    Category = categories[2]
                },
                new Question
                {
                    QuestionID = 10,
                    QuestionText = "I like the floors at my place of work",
                    CategoryID = 4,
                    Category = categories[3]
                },
                new Question
                {
                    QuestionID = 11,
                    QuestionText = "I like the dress code at my place of work",
                    CategoryID = 4,
                    Category = categories[3]
                },
                new Question
                {
                    QuestionID = 12,
                    QuestionText = "I like the computers at my place of work",
                    CategoryID = 4,
                    Category = categories[3]
                },
                new Question
                {
                    QuestionID = 13,
                    QuestionText = "I like the books at my place of work",
                    CategoryID = 5,
                    Category = categories[4]
                },
                new Question
                {
                    QuestionID = 14,
                    QuestionText = "I like the horses at my place of work",
                    CategoryID = 5,
                    Category = categories[4]
                },
                new Question
                {
                    QuestionID = 15,
                    QuestionText = "I like the unicorns at my place of work",
                    CategoryID = 5,
                    Category = categories[4]
                },
                new Question
                {
                    QuestionID = 16,
                    QuestionText = "I like the frogs at my place of work",
                    CategoryID = 6,
                    Category = categories[5]
                },
                new Question
                {
                    QuestionID = 17,
                    QuestionText = "I like the queers at my place of work",
                    CategoryID = 6,
                    Category = categories[5]
                },
                new Question
                {
                    QuestionID = 18,
                    QuestionText = "I like the sluts at my place of work",
                    CategoryID = 6,
                    Category = categories[5]
                },
                new Question
                {
                    QuestionID = 19,
                    QuestionText = "I like the gays at my place of work",
                    CategoryID = 6,
                    Category = categories[5]
                },
                new Question
                {
                    QuestionID = 20,
                    QuestionText = "I like the apes at my place of work",
                    CategoryID = 7,
                    Category = categories[6]
                },
                new Question
                {
                    QuestionID = 21,
                    QuestionText = "I like the pianos at my place of work",
                    CategoryID = 7,
                    Category = categories[6]
                },
                new Question
                {
                    QuestionID = 22,
                    QuestionText = "I like the kites at my place of work",
                    CategoryID = 7,
                    Category = categories[6]
                },
                new Question
                {
                    QuestionID = 23,
                    QuestionText = "I like the cars at my place of work",
                    CategoryID = 7,
                    Category = categories[6]
                },
                new Question
                {
                    QuestionID = 24,
                    QuestionText = "I like the food at my place of work",
                    CategoryID = 8,
                    Category = categories[7]
                },
                new Question
                {
                    QuestionID = 25,
                    QuestionText = "I like the coffee at my place of work",
                    CategoryID = 8,
                    Category = categories[7]
                },
                new Question
                {
                    QuestionID = 26,
                    QuestionText = "I like the latinos at my place of work",
                    CategoryID = 8,
                    Category = categories[7]
                },
                new Question
                {
                    QuestionID = 27,
                    QuestionText = "I like the amphibians at my place of work",
                    CategoryID = 8,
                    Category = categories[7]
                },
                new Question
                {
                    QuestionID = 28,
                    QuestionText = "I like the workload at my place of work",
                    CategoryID = 9,
                    Category = categories[8]
                },
                new Question
                {
                    QuestionID = 29,
                    QuestionText = "I like the oxygen at my place of work",
                    CategoryID = 9,
                    Category = categories[8]
                },
                new Question
                {
                    QuestionID = 30,
                    QuestionText = "I like the kids at my place of work",
                    CategoryID = 9,
                    Category = categories[8]
                },
                new Question
                {
                    QuestionID = 31,
                    QuestionText = "I like the ping pong tables at my place of work",
                    CategoryID = 9,
                    Category = categories[8]
                },
                new Question
                {
                    QuestionID = 32,
                    QuestionText = "I like the ping pong tables at my place of work",
                    CategoryID = 10,
                    Category = categories[9]
                },
                new Question
                {
                    QuestionID = 33,
                    QuestionText = "I like the ping pong tables at my place of work",
                    CategoryID = 10,
                    Category = categories[9]
                },
                new Question
                {
                    QuestionID = 34,
                    QuestionText = "I like the ping pong tables at my place of work",
                    CategoryID = 10,
                    Category = categories[9]
                },
                new Question
                {
                    QuestionID = 35,
                    QuestionText = "I like the ping pong tables at my place of work",
                    CategoryID = 10,
                    Category = categories[9]
                },
                new Question
                {
                    QuestionID = 36,
                    QuestionText = "I like the ping pong tables at my place of work",
                    CategoryID = 11,
                    Category = categories[10]
                },
                new Question
                {
                    QuestionID = 37,
                    QuestionText = "I like the ping pong tables at my place of work",
                     CategoryID = 11,
                    Category = categories[10]
                },
                new Question
                {
                    QuestionID = 38,
                    QuestionText = "I like the ping pong tables at my place of work",
                    CategoryID = 11,
                    Category = categories[10]
                },
                new Question
                {
                    QuestionID = 39,
                    QuestionText = "I like the ping pong tables at my place of work",
                    CategoryID = 11,
                    Category = categories[10]
                },
                new Question
                {
                    QuestionID = 40,
                    QuestionText = "I like the ping pong tables at my place of work",
                    CategoryID = 12,
                    Category = categories[11]
                },
                new Question
                {
                    QuestionID = 41,
                    QuestionText = "I like the ping pong tables at my place of work",
                    CategoryID = 12,
                    Category = categories[11]
                },
                new Question
                {
                    QuestionID = 42,
                    QuestionText = "I like the ping pong tables at my place of work",
                    CategoryID = 12,
                    Category = categories[11]
                },
                new Question
                {
                    QuestionID = 43,
                    QuestionText = "I like the ping pong tables at my place of work",
                    CategoryID = 12,
                    Category = categories[11]
                },
                new Question
                {
                    QuestionID = 43,
                    QuestionText = "I like the ping pong tables at my place of work",
                    CategoryID = 12,
                    Category = categories[11]
                }
            };
            questionList.ForEach(s => context.Questions.Add(s));
            //foreach (var question in questionList)
            //{
            //    if (!context.Questions.Any(q => q.QuestionID == question.QuestionID))
            //    {
            //        context.Questions.Add(question);
            //    }
            //}
            context.SaveChanges();

        }
    }
}
