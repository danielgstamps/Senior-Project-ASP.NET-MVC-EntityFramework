namespace CapstoneProject.DataMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class QuestionRemoveSelectedAnswer : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Question", "SelectedAnswer");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Question", "SelectedAnswer", c => c.String());
        }
    }
}
