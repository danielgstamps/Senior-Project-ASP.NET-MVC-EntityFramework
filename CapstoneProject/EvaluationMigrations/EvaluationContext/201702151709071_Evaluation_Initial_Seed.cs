namespace CapstoneProject.EvaluationMigrations.EvaluationContext
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EvaluationInitialSeed : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Category",
                c => new
                {
                    CategoryID = c.Int(nullable: false, identity: true),
                    Name = c.String(nullable: false, maxLength: 50),
                    Description = c.String(nullable: false, maxLength: 500),
                    EvaluationID = c.Int(nullable: false),
                })
                .PrimaryKey(t => t.CategoryID)
                .ForeignKey("dbo.Evaluation", t => t.EvaluationID, cascadeDelete: true)
                .Index(t => t.EvaluationID);

            CreateTable(
                "dbo.Evaluation",
                c => new
                    {
                        EvaluationID = c.Int(nullable: false, identity: true),
                        Stage = c.String(nullable: false, maxLength: 9),
                        Type = c.Int(nullable: false),
                        EmployeeID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.EvaluationID)
                .ForeignKey("dbo.Employee", t => t.EmployeeID, cascadeDelete: true)
                .Index(t => t.EmployeeID);
            
            CreateTable(
                "dbo.Employee",
                c => new
                    {
                        EmployeeID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(nullable: false, maxLength: 50),
                        LastName = c.String(nullable: false, maxLength: 50),
                        Email = c.String(nullable: false),
                        Address = c.String(nullable: false),
                        Phone = c.String(nullable: false),
                        CohortID = c.Int(nullable: false),
                        SupervisorID = c.Int(),
                        ManagerID = c.Int(),
                    })
                .PrimaryKey(t => t.EmployeeID)
                .ForeignKey("dbo.Cohort", t => t.CohortID, cascadeDelete: true)
                .ForeignKey("dbo.Employee", t => t.ManagerID)
                .ForeignKey("dbo.Employee", t => t.SupervisorID)
                .Index(t => t.CohortID)
                .Index(t => t.SupervisorID)
                .Index(t => t.ManagerID);
            
            CreateTable(
                "dbo.Cohort",
                c => new
                    {
                        CohortID = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.CohortID);
            
            CreateTable(
                "dbo.Question",
                c => new
                    {
                        QuestionID = c.Int(nullable: false, identity: true),
                        QuestionText = c.String(nullable: false, maxLength: 500),
                        Comment = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.QuestionID);
            
            CreateTable(
                "dbo.QuestionCategory",
                c => new
                    {
                        Question_QuestionID = c.Int(nullable: false),
                        Category_CategoryID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Question_QuestionID, t.Category_CategoryID })
                .ForeignKey("dbo.Question", t => t.Question_QuestionID, cascadeDelete: true)
                .ForeignKey("dbo.Category", t => t.Category_CategoryID, cascadeDelete: true)
                .Index(t => t.Question_QuestionID)
                .Index(t => t.Category_CategoryID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.QuestionCategory", "Category_CategoryID", "dbo.Category");
            DropForeignKey("dbo.QuestionCategory", "Question_QuestionID", "dbo.Question");
            DropForeignKey("dbo.Employee", "SupervisorID", "dbo.Employee");
            DropForeignKey("dbo.Employee", "ManagerID", "dbo.Employee");
            DropForeignKey("dbo.Evaluation", "EmployeeID", "dbo.Employee");
            DropForeignKey("dbo.Employee", "CohortID", "dbo.Cohort");
            DropForeignKey("dbo.Category", "EvaluationID", "dbo.Evaluation");
            DropIndex("dbo.QuestionCategory", new[] { "Category_CategoryID" });
            DropIndex("dbo.QuestionCategory", new[] { "Question_QuestionID" });
            DropIndex("dbo.Employee", new[] { "ManagerID" });
            DropIndex("dbo.Employee", new[] { "SupervisorID" });
            DropIndex("dbo.Employee", new[] { "CohortID" });
            DropIndex("dbo.Evaluation", new[] { "EmployeeID" });
            DropIndex("dbo.Category", new[] { "EvaluationID" });
            DropTable("dbo.QuestionCategory");
            DropTable("dbo.Question");
            DropTable("dbo.Cohort");
            DropTable("dbo.Employee");
            DropTable("dbo.Evaluation");
            DropTable("dbo.Category");
        }
    }
}
