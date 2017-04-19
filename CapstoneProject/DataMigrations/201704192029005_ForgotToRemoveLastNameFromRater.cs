namespace CapstoneProject.DataMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ForgotToRemoveLastNameFromRater : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Rater", "LastName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Rater", "LastName", c => c.String());
        }
    }
}
