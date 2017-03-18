namespace CapstoneProject.DataMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EvaluationQuestionsRefactor : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Evaluation", "EmployeeID", "dbo.Employee");
            DropIndex("dbo.Evaluation", new[] { "EmployeeID" });
            RenameColumn(table: "dbo.Evaluation", name: "EmployeeID", newName: "Employee_EmployeeID");
            AddColumn("dbo.Question", "SelectedAnswer", c => c.String());
            AddColumn("dbo.Question", "Evaluation_EvaluationID", c => c.Int());
            AddColumn("dbo.Answer", "AnswerText", c => c.String());
            AlterColumn("dbo.Evaluation", "Employee_EmployeeID", c => c.Int());
            CreateIndex("dbo.Evaluation", "Employee_EmployeeID");
            CreateIndex("dbo.Question", "Evaluation_EvaluationID");
            AddForeignKey("dbo.Question", "Evaluation_EvaluationID", "dbo.Evaluation", "EvaluationID");
            AddForeignKey("dbo.Evaluation", "Employee_EmployeeID", "dbo.Employee", "EmployeeID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Evaluation", "Employee_EmployeeID", "dbo.Employee");
            DropForeignKey("dbo.Question", "Evaluation_EvaluationID", "dbo.Evaluation");
            DropIndex("dbo.Question", new[] { "Evaluation_EvaluationID" });
            DropIndex("dbo.Evaluation", new[] { "Employee_EmployeeID" });
            AlterColumn("dbo.Evaluation", "Employee_EmployeeID", c => c.Int(nullable: false));
            DropColumn("dbo.Answer", "AnswerText");
            DropColumn("dbo.Question", "Evaluation_EvaluationID");
            DropColumn("dbo.Question", "SelectedAnswer");
            RenameColumn(table: "dbo.Evaluation", name: "Employee_EmployeeID", newName: "EmployeeID");
            CreateIndex("dbo.Evaluation", "EmployeeID");
            AddForeignKey("dbo.Evaluation", "EmployeeID", "dbo.Employee", "EmployeeID", cascadeDelete: true);
        }
    }
}
