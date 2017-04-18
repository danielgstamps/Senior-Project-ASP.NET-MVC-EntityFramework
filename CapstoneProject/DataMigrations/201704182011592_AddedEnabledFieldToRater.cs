namespace CapstoneProject.DataMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedEnabledFieldToRater : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Rater", "Enabled", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Rater", "Enabled");
        }
    }
}
