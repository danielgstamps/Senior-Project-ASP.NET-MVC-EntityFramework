namespace CapstoneProject.DataMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RaterFirstLastNameNowJustName : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Rater", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.Rater", "LastName", c => c.String());
            DropColumn("dbo.Rater", "FirstName");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Rater", "FirstName", c => c.String(nullable: false));
            AlterColumn("dbo.Rater", "LastName", c => c.String(nullable: false));
            DropColumn("dbo.Rater", "Name");
        }
    }
}
