namespace CapstoneProject.DataMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ChangedRaterEnabledToDisabled : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Rater", "Disabled", c => c.Boolean(nullable: false));
            DropColumn("dbo.Rater", "Enabled");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Rater", "Enabled", c => c.Boolean(nullable: false));
            DropColumn("dbo.Rater", "Disabled");
        }
    }
}
