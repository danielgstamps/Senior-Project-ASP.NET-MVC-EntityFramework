namespace CapstoneProject.DataMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CompletedDateNullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Evaluation", "CompletedDate", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Evaluation", "CompletedDate", c => c.DateTime(nullable: false));
        }
    }
}
