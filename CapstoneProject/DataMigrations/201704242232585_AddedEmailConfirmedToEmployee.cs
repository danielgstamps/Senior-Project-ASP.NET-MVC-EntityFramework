namespace CapstoneProject.DataMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedEmailConfirmedToEmployee : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Employee", "EmailConfirmed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Employee", "EmailConfirmed");
        }
    }
}
