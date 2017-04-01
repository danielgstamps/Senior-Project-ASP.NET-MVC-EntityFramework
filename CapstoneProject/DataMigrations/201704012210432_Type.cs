namespace CapstoneProject.DataMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Type : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Category", "TypeID", "dbo.Type");
            DropForeignKey("dbo.Evaluation", "TypeID", "dbo.Type");
            DropIndex("dbo.Category", new[] { "TypeID" });
            RenameColumn(table: "dbo.Evaluation", name: "TypeID", newName: "AbstractTypeID");
            RenameIndex(table: "dbo.Evaluation", name: "IX_TypeID", newName: "IX_AbstractTypeID");
            CreateTable(
                "dbo.AbstractType",
                c => new
                    {
                        AbstractTypeID = c.Int(nullable: false, identity: true),
                        TypeName = c.String(),
                        TypeID = c.Int(),
                        TypeID1 = c.Int(),
                        Discriminator = c.String(nullable: false, maxLength: 128),
                    })
                .PrimaryKey(t => t.AbstractTypeID);
            
            AddColumn("dbo.Category", "AbstractTypeID", c => c.Int(nullable: false));
            CreateIndex("dbo.Category", "AbstractTypeID");
            AddForeignKey("dbo.Category", "AbstractTypeID", "dbo.AbstractType", "AbstractTypeID", cascadeDelete: true);
            DropColumn("dbo.Category", "TypeID");
            DropTable("dbo.Type");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Type",
                c => new
                    {
                        TypeID = c.Int(nullable: false, identity: true),
                        TypeName = c.String(),
                    })
                .PrimaryKey(t => t.TypeID);
            
            AddColumn("dbo.Category", "TypeID", c => c.Int(nullable: false));
            DropForeignKey("dbo.Category", "AbstractTypeID", "dbo.AbstractType");
            DropIndex("dbo.Category", new[] { "AbstractTypeID" });
            DropColumn("dbo.Category", "AbstractTypeID");
            DropTable("dbo.AbstractType");
            RenameIndex(table: "dbo.Evaluation", name: "IX_AbstractTypeID", newName: "IX_TypeID");
            RenameColumn(table: "dbo.Evaluation", name: "AbstractTypeID", newName: "TypeID");
            CreateIndex("dbo.Category", "TypeID");
            AddForeignKey("dbo.Category", "TypeID", "dbo.Type", "TypeID", cascadeDelete: true);
        }
    }
}
