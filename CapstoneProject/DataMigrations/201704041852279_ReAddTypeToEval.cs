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
            RenameColumn(table: "dbo.Evaluation", name: "Type_TypeID", newName: "TypeId");
            AlterColumn("dbo.Evaluation", "TypeId", c => c.Int(nullable: false));
            CreateIndex("dbo.Evaluation", "TypeId");
            AddForeignKey("dbo.Evaluation", "TypeId", "dbo.Type", "TypeId", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Evaluation", "TypeId", "dbo.Type");
            DropIndex("dbo.Evaluation", new[] { "TypeId" });
            AlterColumn("dbo.Evaluation", "TypeId", c => c.Int());
            RenameColumn(table: "dbo.Evaluation", name: "TypeId", newName: "Type_TypeID");
            CreateIndex("dbo.Evaluation", "Type_TypeID");
            AddForeignKey("dbo.Evaluation", "Type_TypeID", "dbo.Type", "TypeId");
        }
    }
}
