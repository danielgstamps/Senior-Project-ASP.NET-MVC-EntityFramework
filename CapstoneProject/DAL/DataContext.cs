using CapstoneProject.Models;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;

namespace CapstoneProject.DAL
{
    public class DataContext : DbContext
    {
        public DataContext() : base("DefaultConnection")
        {
        }

        public DbSet<Category> Categories { get; set; }
        public DbSet<Cohort> Cohorts { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Evaluation> Evaluations { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<AbstractType> Types { get; set; }
        public DbSet<Rater> Raters { get; set; }
        public DbSet<Stage> Stages { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
          //  modelBuilder.Entity<Employee>().HasOptional(e => e.Supervisor).WithMany().HasForeignKey(e => e.SupervisorID);
         //   modelBuilder.Entity<Employee>().HasOptional(e => e.Manager).WithMany().HasForeignKey(e => e.ManagerID);
        } 
    }
}