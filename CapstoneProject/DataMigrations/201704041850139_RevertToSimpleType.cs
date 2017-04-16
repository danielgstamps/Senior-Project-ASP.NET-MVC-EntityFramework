namespace CapstoneProject.DataMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RevertToSimpleType : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Category", "AbstractTypeID", "dbo.AbstractType");
            DropForeignKey("dbo.AbstractType", "TypeOneID", "dbo.AbstractType");
            DropForeignKey("dbo.AbstractType", "TypeTwoID", "dbo.AbstractType");
            DropForeignKey("dbo.Evaluation", "AbstractTypeID", "dbo.AbstractType");
            DropForeignKey("dbo.Evaluation", "StageID", "dbo.Stage");
            DropIndex("dbo.Category", new[] { "AbstractTypeID" });
            DropIndex("dbo.AbstractType", new[] { "TypeOneID" });
            DropIndex("dbo.AbstractType", new[] { "TypeTwoID" });
            DropIndex("dbo.Evaluation", new[] { "AbstractTypeID" });
            DropPrimaryKey("dbo.Stage");
            CreateTable(
                "dbo.Type",
                c => new
                    {
                        TypeID = c.Int(nullable: false, identity: true),
                        TypeName = c.String(),
                    })
                .PrimaryKey(t => t.TypeID);
            
            AddColumn("dbo.Category", "TypeId", c => c.Int(nullable: false));
            AddColumn("dbo.Evaluation", "Type_TypeID", c => c.Int());
            AlterColumn("dbo.Stage", "StageID", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Stage", "StageID");
            CreateIndex("dbo.Category", "TypeId");
            CreateIndex("dbo.Evaluation", "Type_TypeID");
            AddForeignKey("dbo.Category", "TypeId", "dbo.Type", "TypeId", cascadeDelete: true);
            AddForeignKey("dbo.Evaluation", "Type_TypeID", "dbo.Type", "TypeId");
            AddForeignKey("dbo.Evaluation", "StageID", "dbo.Stage", "StageID", cascadeDelete: true);
            DropColumn("dbo.Category", "AbstractTypeID");
            DropColumn("dbo.Evaluation", "AbstractTypeID");
            DropTable("dbo.AbstractType");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.AbstractType",
                c => new
                    {
                        AbstractTypeID = c.Int(nullable: false),
                        TypeName = c.String(),
                        TypeOneID = c.Int(),
                        TypeTwoID = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.AbstractTypeID);
            
            AddColumn("dbo.Evaluation", "AbstractTypeID", c => c.Int(nullable: false));
            AddColumn("dbo.Category", "AbstractTypeID", c => c.Int(nullable: false));
            DropForeignKey("dbo.Evaluation", "StageID", "dbo.Stage");
            DropForeignKey("dbo.Evaluation", "Type_TypeID", "dbo.Type");
            DropForeignKey("dbo.Category", "TypeId", "dbo.Type");
            DropIndex("dbo.Evaluation", new[] { "Type_TypeID" });
            DropIndex("dbo.Category", new[] { "TypeId" });
            DropPrimaryKey("dbo.Stage");
            AlterColumn("dbo.Stage", "StageID", c => c.Int(nullable: false));
            DropColumn("dbo.Evaluation", "Type_TypeID");
            DropColumn("dbo.Category", "TypeId");
            DropTable("dbo.Type");
            AddPrimaryKey("dbo.Stage", "StageID");
            CreateIndex("dbo.Evaluation", "AbstractTypeID");
            CreateIndex("dbo.AbstractType", "TypeTwoID");
            CreateIndex("dbo.AbstractType", "TypeOneID");
            CreateIndex("dbo.Category", "AbstractTypeID");
            AddForeignKey("dbo.Evaluation", "StageID", "dbo.Stage", "StageID", cascadeDelete: true);
            AddForeignKey("dbo.Evaluation", "AbstractTypeID", "dbo.AbstractType", "AbstractTypeID", cascadeDelete: true);
            AddForeignKey("dbo.AbstractType", "TypeTwoID", "dbo.AbstractType", "AbstractTypeID");
            AddForeignKey("dbo.AbstractType", "TypeOneID", "dbo.AbstractType", "AbstractTypeID");
            AddForeignKey("dbo.Category", "AbstractTypeID", "dbo.AbstractType", "AbstractTypeID", cascadeDelete: true);
        }
    }
}
