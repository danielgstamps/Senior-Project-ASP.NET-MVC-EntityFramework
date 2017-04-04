namespace CapstoneProject.DataMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RenamedClosedDateToCloseDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Evaluation", "CloseDate", c => c.DateTime());
            DropColumn("dbo.Evaluation", "ClosedDate");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Evaluation", "ClosedDate", c => c.DateTime());
            DropColumn("dbo.Evaluation", "CloseDate");
        }
    }
}
