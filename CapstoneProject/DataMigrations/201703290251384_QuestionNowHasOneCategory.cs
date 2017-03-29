namespace CapstoneProject.DataMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QuestionNowHasOneCategory : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.QuestionCategory", "Question_QuestionID", "dbo.Question");
            DropForeignKey("dbo.QuestionCategory", "Category_CategoryID", "dbo.Category");
            DropIndex("dbo.QuestionCategory", new[] { "Question_QuestionID" });
            DropIndex("dbo.QuestionCategory", new[] { "Category_CategoryID" });
            AddColumn("dbo.Question", "Category_CategoryID", c => c.Int());
            CreateIndex("dbo.Question", "Category_CategoryID");
            AddForeignKey("dbo.Question", "Category_CategoryID", "dbo.Category", "CategoryID");
            DropTable("dbo.QuestionCategory");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.QuestionCategory",
                c => new
                    {
                        Question_QuestionID = c.Int(nullable: false),
                        Category_CategoryID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Question_QuestionID, t.Category_CategoryID });
            
            DropForeignKey("dbo.Question", "Category_CategoryID", "dbo.Category");
            DropIndex("dbo.Question", new[] { "Category_CategoryID" });
            DropColumn("dbo.Question", "Category_CategoryID");
            CreateIndex("dbo.QuestionCategory", "Category_CategoryID");
            CreateIndex("dbo.QuestionCategory", "Question_QuestionID");
            AddForeignKey("dbo.QuestionCategory", "Category_CategoryID", "dbo.Category", "CategoryID", cascadeDelete: true);
            AddForeignKey("dbo.QuestionCategory", "Question_QuestionID", "dbo.Question", "QuestionID", cascadeDelete: true);
        }
    }
}
