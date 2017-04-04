namespace CapstoneProject.DataMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class ReAddTypeToEval1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Evaluation", "SelfAnswers", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Evaluation", "SelfAnswers");
        }
    }
}
