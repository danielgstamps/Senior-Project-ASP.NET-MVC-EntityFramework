namespace CapstoneProject.DataMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OpenDateCloseDateNotNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Evaluation", "OpenDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Evaluation", "CloseDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Evaluation", "CloseDate", c => c.DateTime());
            AlterColumn("dbo.Evaluation", "OpenDate", c => c.DateTime());
        }
    }
}
