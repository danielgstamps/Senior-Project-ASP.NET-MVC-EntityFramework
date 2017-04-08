namespace CapstoneProject.DataMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class changedRaterAnswersToString : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Rater", "Answers", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Rater", "Answers");
        }
    }
}
