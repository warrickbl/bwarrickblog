namespace bwarrickblog.Migrations
{
    using bwarrickblog.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<bwarrickblog.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(bwarrickblog.Models.ApplicationDbContext context)
        {
            var roleManager = new RoleManager<IdentityRole>(
                new RoleStore<IdentityRole>(context));
            if (!context.Roles.Any(r => r.Name == "Admin"))
            {
                roleManager.Create(new IdentityRole { Name = "Admin" });
            }
            if (!context.Roles.Any(r => r.Name == "Moderator"))
            {
                roleManager.Create(new IdentityRole { Name = "Moderator" });
            }

            var userManager = new UserManager<ApplicationUser>(
                new UserStore<ApplicationUser>(context));
            if (!context.Users.Any(u => u.Email == "blwarrick1107@gmail.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "blwarrick1107@gmail.com",
                    Email = "blwarrick1107@gmail.com",
                    FirstName = "Bri",
                    LastName = "Warrick",
                }, "briLeigh7!");
            }
            if (!context.Users.Any(u => u.Email == "rchapman@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "rchapman@coderfoundry.com",
                    Email = "rchapman@coderfoundry.com",
                    FirstName = "Ryan",
                    LastName = "Chapman",
                }, "Abc123!");
            }
            if (!context.Users.Any(u => u.Email == "mjaang@coderfoundry.com"))
            {
                userManager.Create(new ApplicationUser
                {
                    UserName = "mjaang@coderfoundry.com",
                    Email = "mjaang@coderfoundry.com",
                    FirstName = "Mark",
                    LastName = "Jaang",
                }, "Abc123!");
            }
            var userId = userManager.FindByEmail("blwarrick1107@gmail.com").Id;
            userManager.AddToRole(userId, "Admin");
            var moderatorId = userManager.FindByEmail("rchapman@coderfoundry.com").Id;
            userManager.AddToRole(moderatorId, "Moderator");
            var moderator2Id = userManager.FindByEmail("mjaang@coderfoundry.com").Id;
            userManager.AddToRole(moderator2Id, "Moderator");
        }
    }
}
