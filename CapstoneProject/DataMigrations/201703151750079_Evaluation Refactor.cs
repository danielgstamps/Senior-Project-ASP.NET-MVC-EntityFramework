namespace CapstoneProject.DataMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EvaluationRefactor : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Stage",
                c => new
                    {
                        StageID = c.Int(nullable: false, identity: true),
                        StageName = c.String(nullable: false, maxLength: 9),
                    })
                .PrimaryKey(t => t.StageID);
            
            CreateTable(
                "dbo.AbstractType",
                c => new
                    {
                        TypeID = c.Int(nullable: false, identity: true),
                        TypeName = c.String(),
                    })
                .PrimaryKey(t => t.TypeID);
            
            CreateTable(
                "dbo.Answer",
                c => new
                    {
                        AnswerID = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.AnswerID);
            
            CreateTable(
                "dbo.StageEvaluation",
                c => new
                    {
                        Stage_StageID = c.Int(nullable: false),
                        Evaluation_EvaluationID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Stage_StageID, t.Evaluation_EvaluationID })
                .ForeignKey("dbo.Stage", t => t.Stage_StageID, cascadeDelete: true)
                .ForeignKey("dbo.Evaluation", t => t.Evaluation_EvaluationID, cascadeDelete: true)
                .Index(t => t.Stage_StageID)
                .Index(t => t.Evaluation_EvaluationID);
            
            CreateTable(
                "dbo.AnswerQuestion",
                c => new
                    {
                        Answer_AnswerID = c.Int(nullable: false),
                        Question_QuestionID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Answer_AnswerID, t.Question_QuestionID })
                .ForeignKey("dbo.Answer", t => t.Answer_AnswerID, cascadeDelete: true)
                .ForeignKey("dbo.Question", t => t.Question_QuestionID, cascadeDelete: true)
                .Index(t => t.Answer_AnswerID)
                .Index(t => t.Question_QuestionID);
            
            AddColumn("dbo.Category", "TypeId", c => c.Int(nullable: false));
            AddColumn("dbo.Evaluation", "TypeId", c => c.Int(nullable: false));
            CreateIndex("dbo.Category", "TypeId");
            CreateIndex("dbo.Evaluation", "TypeId");
            AddForeignKey("dbo.Evaluation", "TypeId", "dbo.AbstractType", "TypeId", cascadeDelete: true);
            AddForeignKey("dbo.Category", "TypeId", "dbo.AbstractType", "TypeId", cascadeDelete: true);
            DropColumn("dbo.Evaluation", "AbstractType");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Evaluation", "AbstractType", c => c.Int(nullable: false));
            DropForeignKey("dbo.Category", "TypeId", "dbo.AbstractType");
            DropForeignKey("dbo.AnswerQuestion", "Question_QuestionID", "dbo.Question");
            DropForeignKey("dbo.AnswerQuestion", "Answer_AnswerID", "dbo.Answer");
            DropForeignKey("dbo.Evaluation", "TypeId", "dbo.AbstractType");
            DropForeignKey("dbo.StageEvaluation", "Evaluation_EvaluationID", "dbo.Evaluation");
            DropForeignKey("dbo.StageEvaluation", "Stage_StageID", "dbo.Stage");
            DropIndex("dbo.AnswerQuestion", new[] { "Question_QuestionID" });
            DropIndex("dbo.AnswerQuestion", new[] { "Answer_AnswerID" });
            DropIndex("dbo.StageEvaluation", new[] { "Evaluation_EvaluationID" });
            DropIndex("dbo.StageEvaluation", new[] { "Stage_StageID" });
            DropIndex("dbo.Evaluation", new[] { "TypeId" });
            DropIndex("dbo.Category", new[] { "TypeId" });
            DropColumn("dbo.Evaluation", "TypeId");
            DropColumn("dbo.Category", "TypeId");
            DropTable("dbo.AnswerQuestion");
            DropTable("dbo.StageEvaluation");
            DropTable("dbo.Answer");
            DropTable("dbo.AbstractType");
            DropTable("dbo.Stage");
        }
    }
}
