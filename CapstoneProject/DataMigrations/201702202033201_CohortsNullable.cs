namespace CapstoneProject.DataMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CohortsNullable : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Employee", "CohortID", "dbo.Cohort");
            DropIndex("dbo.Employee", new[] { "CohortID" });
            AlterColumn("dbo.Employee", "CohortID", c => c.Int());
            CreateIndex("dbo.Employee", "CohortID");
            AddForeignKey("dbo.Employee", "CohortID", "dbo.Cohort", "CohortID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Employee", "CohortID", "dbo.Cohort");
            DropIndex("dbo.Employee", new[] { "CohortID" });
            AlterColumn("dbo.Employee", "CohortID", c => c.Int(nullable: false));
            CreateIndex("dbo.Employee", "CohortID");
            AddForeignKey("dbo.Employee", "CohortID", "dbo.Cohort", "CohortID", cascadeDelete: true);
        }
    }
}
