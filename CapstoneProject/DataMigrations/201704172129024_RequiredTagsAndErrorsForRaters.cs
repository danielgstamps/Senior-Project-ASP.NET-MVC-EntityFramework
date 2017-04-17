namespace CapstoneProject.DataMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequiredTagsAndErrorsForRaters : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Rater", "FirstName", c => c.String(nullable: false));
            AlterColumn("dbo.Rater", "LastName", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Rater", "LastName", c => c.String());
            AlterColumn("dbo.Rater", "FirstName", c => c.String());
        }
    }
}
