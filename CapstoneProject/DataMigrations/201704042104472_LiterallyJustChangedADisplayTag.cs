namespace CapstoneProject.DataMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LiterallyJustChangedADisplayTag : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Evaluation", "OpenDate", c => c.DateTime());
            AlterColumn("dbo.Evaluation", "ClosedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Evaluation", "ClosedDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Evaluation", "OpenDate", c => c.DateTime(nullable: false));
        }
    }
}
