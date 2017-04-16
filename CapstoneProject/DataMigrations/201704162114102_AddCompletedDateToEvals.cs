namespace CapstoneProject.DataMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCompletedDateToEvals : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Evaluation", "CompletedDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Evaluation", "CompletedDate");
        }
    }
}
