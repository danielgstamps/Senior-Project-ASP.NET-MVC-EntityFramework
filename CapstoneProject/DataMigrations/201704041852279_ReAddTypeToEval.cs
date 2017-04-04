namespace CapstoneProject.DataMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReAddTypeToEval : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Evaluation", "Type_TypeID", "dbo.Type");
            DropIndex("dbo.Evaluation", new[] { "Type_TypeID" });
            RenameColumn(table: "dbo.Evaluation", name: "Type_TypeID", newName: "TypeID");
            AlterColumn("dbo.Evaluation", "TypeID", c => c.Int(nullable: false));
            CreateIndex("dbo.Evaluation", "TypeID");
            AddForeignKey("dbo.Evaluation", "TypeID", "dbo.Type", "TypeID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Evaluation", "TypeID", "dbo.Type");
            DropIndex("dbo.Evaluation", new[] { "TypeID" });
            AlterColumn("dbo.Evaluation", "TypeID", c => c.Int());
            RenameColumn(table: "dbo.Evaluation", name: "TypeID", newName: "Type_TypeID");
            CreateIndex("dbo.Evaluation", "Type_TypeID");
            AddForeignKey("dbo.Evaluation", "Type_TypeID", "dbo.Type", "TypeID");
        }
    }
}
