namespace CapstoneProject.DataMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedALotOfStuffTake2 : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AnswerQuestion", "Answer_AnswerID", "dbo.Answer");
            DropForeignKey("dbo.AnswerQuestion", "Question_QuestionID", "dbo.Question");
            DropForeignKey("dbo.Question", "Evaluation_EvaluationID", "dbo.Evaluation");
            DropForeignKey("dbo.StageEvaluation", "Stage_StageID", "dbo.Stage");
            DropForeignKey("dbo.StageEvaluation", "Evaluation_EvaluationID", "dbo.Evaluation");
            DropForeignKey("dbo.Employee", "ManagerID", "dbo.Employee");
            DropForeignKey("dbo.Employee", "SupervisorID", "dbo.Employee");
            DropForeignKey("dbo.Evaluation", "Employee_EmployeeID", "dbo.Employee");
            DropIndex("dbo.Question", new[] { "Evaluation_EvaluationID" });
            DropIndex("dbo.Employee", new[] { "SupervisorID" });
            DropIndex("dbo.Employee", new[] { "ManagerID" });
            DropIndex("dbo.Evaluation", new[] { "Employee_EmployeeID" });
            DropIndex("dbo.AnswerQuestion", new[] { "Answer_AnswerID" });
            DropIndex("dbo.AnswerQuestion", new[] { "Question_QuestionID" });
            DropIndex("dbo.StageEvaluation", new[] { "Stage_StageID" });
            DropIndex("dbo.StageEvaluation", new[] { "Evaluation_EvaluationID" });
            RenameColumn(table: "dbo.Evaluation", name: "Employee_EmployeeID", newName: "EmployeeID");
            CreateTable(
                "dbo.Rater",
                c => new
                    {
                        RaterID = c.Int(nullable: false, identity: true),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Role = c.String(),
                        Email = c.String(nullable: false),
                        Evaluation_EvaluationID = c.Int(),
                    })
                .PrimaryKey(t => t.RaterID)
                .ForeignKey("dbo.Evaluation", t => t.Evaluation_EvaluationID)
                .Index(t => t.Evaluation_EvaluationID);
            
            AddColumn("dbo.Cohort", "Type1Assigned", c => c.Boolean(nullable: false));
            AddColumn("dbo.Cohort", "Type2Assigned", c => c.Boolean(nullable: false));
            AddColumn("dbo.Evaluation", "StageID", c => c.Int(nullable: false));
            AddColumn("dbo.Evaluation", "OpenDate", c => c.DateTime(nullable: false));
            AddColumn("dbo.Evaluation", "ClosedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Evaluation", "EmployeeID", c => c.Int(nullable: false));
            CreateIndex("dbo.Evaluation", "EmployeeID");
            CreateIndex("dbo.Evaluation", "StageID");
            AddForeignKey("dbo.Evaluation", "StageID", "dbo.Stage", "StageID", cascadeDelete: true);
            AddForeignKey("dbo.Evaluation", "EmployeeID", "dbo.Employee", "EmployeeID", cascadeDelete: true);
            DropColumn("dbo.Question", "Evaluation_EvaluationID");
            DropColumn("dbo.Employee", "SupervisorID");
            DropColumn("dbo.Employee", "ManagerID");
            DropColumn("dbo.Evaluation", "Comment");
            DropTable("dbo.Answer");
            DropTable("dbo.AnswerQuestion");
            DropTable("dbo.StageEvaluation");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.StageEvaluation",
                c => new
                    {
                        Stage_StageID = c.Int(nullable: false),
                        Evaluation_EvaluationID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Stage_StageID, t.Evaluation_EvaluationID });
            
            CreateTable(
                "dbo.AnswerQuestion",
                c => new
                    {
                        Answer_AnswerID = c.Int(nullable: false),
                        Question_QuestionID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Answer_AnswerID, t.Question_QuestionID });
            
            CreateTable(
                "dbo.Answer",
                c => new
                    {
                        AnswerID = c.Int(nullable: false, identity: true),
                        AnswerText = c.String(),
                    })
                .PrimaryKey(t => t.AnswerID);
            
            AddColumn("dbo.Evaluation", "Comment", c => c.String(maxLength: 500));
            AddColumn("dbo.Employee", "ManagerID", c => c.Int());
            AddColumn("dbo.Employee", "SupervisorID", c => c.Int());
            AddColumn("dbo.Question", "Evaluation_EvaluationID", c => c.Int());
            DropForeignKey("dbo.Evaluation", "EmployeeID", "dbo.Employee");
            DropForeignKey("dbo.Evaluation", "StageID", "dbo.Stage");
            DropForeignKey("dbo.Rater", "Evaluation_EvaluationID", "dbo.Evaluation");
            DropIndex("dbo.Rater", new[] { "Evaluation_EvaluationID" });
            DropIndex("dbo.Evaluation", new[] { "StageID" });
            DropIndex("dbo.Evaluation", new[] { "EmployeeID" });
            AlterColumn("dbo.Evaluation", "EmployeeID", c => c.Int());
            DropColumn("dbo.Evaluation", "ClosedDate");
            DropColumn("dbo.Evaluation", "OpenDate");
            DropColumn("dbo.Evaluation", "StageID");
            DropColumn("dbo.Cohort", "Type2Assigned");
            DropColumn("dbo.Cohort", "Type1Assigned");
            DropTable("dbo.Rater");
            RenameColumn(table: "dbo.Evaluation", name: "EmployeeID", newName: "Employee_EmployeeID");
            CreateIndex("dbo.StageEvaluation", "Evaluation_EvaluationID");
            CreateIndex("dbo.StageEvaluation", "Stage_StageID");
            CreateIndex("dbo.AnswerQuestion", "Question_QuestionID");
            CreateIndex("dbo.AnswerQuestion", "Answer_AnswerID");
            CreateIndex("dbo.Evaluation", "Employee_EmployeeID");
            CreateIndex("dbo.Employee", "ManagerID");
            CreateIndex("dbo.Employee", "SupervisorID");
            CreateIndex("dbo.Question", "Evaluation_EvaluationID");
            AddForeignKey("dbo.Evaluation", "Employee_EmployeeID", "dbo.Employee", "EmployeeID");
            AddForeignKey("dbo.Employee", "SupervisorID", "dbo.Employee", "EmployeeID");
            AddForeignKey("dbo.Employee", "ManagerID", "dbo.Employee", "EmployeeID");
            AddForeignKey("dbo.StageEvaluation", "Evaluation_EvaluationID", "dbo.Evaluation", "EvaluationID", cascadeDelete: true);
            AddForeignKey("dbo.StageEvaluation", "Stage_StageID", "dbo.Stage", "StageID", cascadeDelete: true);
            AddForeignKey("dbo.Question", "Evaluation_EvaluationID", "dbo.Evaluation", "EvaluationID");
            AddForeignKey("dbo.AnswerQuestion", "Question_QuestionID", "dbo.Question", "QuestionID", cascadeDelete: true);
            AddForeignKey("dbo.AnswerQuestion", "Answer_AnswerID", "dbo.Answer", "AnswerID", cascadeDelete: true);
        }
    }
}
