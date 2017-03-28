namespace CapstoneProject.DataMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeEvalFromCategory : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Category", "Evaluation_EvaluationID", "dbo.Evaluation");
            DropIndex("dbo.Category", new[] { "Evaluation_EvaluationID" });
            DropColumn("dbo.Category", "Evaluation_EvaluationID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Category", "Evaluation_EvaluationID", c => c.Int());
            CreateIndex("dbo.Category", "Evaluation_EvaluationID");
            AddForeignKey("dbo.Category", "Evaluation_EvaluationID", "dbo.Evaluation", "EvaluationID");
        }
    }
}
