using System.Linq;
using CapstoneProject.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace CapstoneProject.UserMigrations
{
    using System;
    using System.Data.Entity.Migrations;

    internal sealed class UserConfig : DbMigrationsConfiguration<CapstoneProject.Models.ApplicationDbContext>
    {
        public UserConfig()
        {
            AutomaticMigrationsEnabled = false;
            MigrationsDirectory = @"UserMigrations";
        }

        protected override void Seed(CapstoneProject.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var userManager = new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            // If the admin role doesn't exist, create it.
            if (!roleManager.RoleExists("Admin"))
            {
                var role = new IdentityRole { Name = "Admin" };
                roleManager.Create(role);
            }

            // If the user role doesn't exist, create it.
            if (!roleManager.RoleExists("User"))
            {
                var role = new IdentityRole { Name = "User" };
                roleManager.Create(role);
            }

            if (!roleManager.RoleExists("Rater"))
            {
                var role = new IdentityRole { Name = "Rater" };
                roleManager.Create(role);
            }

            // If the admin already exists, delete it. (Can instead be manually deleted from DB)
            if (userManager.FindByEmail("admin@gmail.com") != null)
            {
                userManager.Delete(userManager.FindByEmail("admin@gmail.com"));
            }

            // Create user.
            var user = new ApplicationUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                PhoneNumber = "678-555-3399",
                EmailConfirmed = true
            };

            const string userPwd = "123123";

            // Write user to DB.
            var result = userManager.Create(user, userPwd);
   
            // If that worked, make him an Admin.
            if (result.Succeeded)
            {
                userManager.AddToRole(user.Id, "Admin");
            }
            else
            {
                throw new Exception(result.Errors.First()); }
            }
        }
    }
