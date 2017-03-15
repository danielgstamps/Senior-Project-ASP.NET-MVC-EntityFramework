namespace CapstoneProject.DataMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EvaluationRefactor : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Category", "EvaluationID", "dbo.Evaluation");
            DropIndex("dbo.Category", new[] { "EvaluationID" });
            RenameColumn(table: "dbo.Category", name: "EvaluationID", newName: "Evaluation_EvaluationID");
            AddColumn("dbo.Evaluation", "Comment", c => c.String(maxLength: 500));
            AlterColumn("dbo.Category", "Evaluation_EvaluationID", c => c.Int());
            CreateIndex("dbo.Category", "Evaluation_EvaluationID");
            AddForeignKey("dbo.Category", "Evaluation_EvaluationID", "dbo.Evaluation", "EvaluationID");
            DropColumn("dbo.Evaluation", "Stage");
            DropColumn("dbo.Question", "Comment");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Question", "Comment", c => c.String(maxLength: 500));
            AddColumn("dbo.Evaluation", "Stage", c => c.String(nullable: false, maxLength: 9));
            DropForeignKey("dbo.Category", "Evaluation_EvaluationID", "dbo.Evaluation");
            DropIndex("dbo.Category", new[] { "Evaluation_EvaluationID" });
            AlterColumn("dbo.Category", "Evaluation_EvaluationID", c => c.Int(nullable: false));
            DropColumn("dbo.Evaluation", "Comment");
            RenameColumn(table: "dbo.Category", name: "Evaluation_EvaluationID", newName: "EvaluationID");
            CreateIndex("dbo.Category", "EvaluationID");
            AddForeignKey("dbo.Category", "EvaluationID", "dbo.Evaluation", "EvaluationID", cascadeDelete: true);
        }
    }
}
