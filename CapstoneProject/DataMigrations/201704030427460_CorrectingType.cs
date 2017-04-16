namespace CapstoneProject.DataMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CorrectingType : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Question", "Category_CategoryID", "dbo.Category");
            DropForeignKey("dbo.Rater", "Evaluation_EvaluationID", "dbo.Evaluation");
            DropIndex("dbo.Question", new[] { "Category_CategoryID" });
            DropIndex("dbo.Rater", new[] { "Evaluation_EvaluationID" });
            RenameColumn(table: "dbo.Question", name: "Category_CategoryID", newName: "CategoryID");
            RenameColumn(table: "dbo.Rater", name: "Evaluation_EvaluationID", newName: "EvaluationID");
            AddColumn("dbo.AbstractType", "TypeOneID", c => c.Int());
            AddColumn("dbo.AbstractType", "TypeTwoID", c => c.Int());
            AlterColumn("dbo.Question", "CategoryID", c => c.Int(nullable: false));
            AlterColumn("dbo.Rater", "EvaluationID", c => c.Int(nullable: false));
            CreateIndex("dbo.AbstractType", "TypeOneID");
            CreateIndex("dbo.AbstractType", "TypeTwoID");
            CreateIndex("dbo.Question", "CategoryID");
            CreateIndex("dbo.Rater", "EvaluationID");
            AddForeignKey("dbo.AbstractType", "TypeOneID", "dbo.AbstractType", "AbstractTypeID");
            AddForeignKey("dbo.AbstractType", "TypeTwoID", "dbo.AbstractType", "AbstractTypeID");
            AddForeignKey("dbo.Question", "CategoryID", "dbo.Category", "CategoryID", cascadeDelete: true);
            AddForeignKey("dbo.Rater", "EvaluationID", "dbo.Evaluation", "EvaluationID", cascadeDelete: true);
            DropColumn("dbo.AbstractType", "TypeId");
            DropColumn("dbo.AbstractType", "TypeID1");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AbstractType", "TypeID1", c => c.Int());
            AddColumn("dbo.AbstractType", "TypeId", c => c.Int());
            DropForeignKey("dbo.Rater", "EvaluationID", "dbo.Evaluation");
            DropForeignKey("dbo.Question", "CategoryID", "dbo.Category");
            DropForeignKey("dbo.AbstractType", "TypeTwoID", "dbo.AbstractType");
            DropForeignKey("dbo.AbstractType", "TypeOneID", "dbo.AbstractType");
            DropIndex("dbo.Rater", new[] { "EvaluationID" });
            DropIndex("dbo.Question", new[] { "CategoryID" });
            DropIndex("dbo.AbstractType", new[] { "TypeTwoID" });
            DropIndex("dbo.AbstractType", new[] { "TypeOneID" });
            AlterColumn("dbo.Rater", "EvaluationID", c => c.Int());
            AlterColumn("dbo.Question", "CategoryID", c => c.Int());
            DropColumn("dbo.AbstractType", "TypeTwoID");
            DropColumn("dbo.AbstractType", "TypeOneID");
            RenameColumn(table: "dbo.Rater", name: "EvaluationID", newName: "Evaluation_EvaluationID");
            RenameColumn(table: "dbo.Question", name: "CategoryID", newName: "Category_CategoryID");
            CreateIndex("dbo.Rater", "Evaluation_EvaluationID");
            CreateIndex("dbo.Question", "Category_CategoryID");
            AddForeignKey("dbo.Rater", "Evaluation_EvaluationID", "dbo.Evaluation", "EvaluationID");
            AddForeignKey("dbo.Question", "Category_CategoryID", "dbo.Category", "CategoryID");
        }
    }
}
