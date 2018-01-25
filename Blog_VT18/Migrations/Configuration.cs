namespace Blog_VT18.Migrations {
    using Blog_VT18.Models;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using Microsoft.AspNet.Identity;

    internal sealed class Configuration : DbMigrationsConfiguration<Blog_VT18.Models.ApplicationDbContext> {
        public Configuration() {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context) {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if(!roleManager.RoleExists("Administrator")) {
                var role = new IdentityRole();
                role.Name = "Administrator";
                roleManager.Create(role);
            }

            var user = new ApplicationUser {
                Name = "Admin",
                UserName = "admin@user.com",
                Email = "admin@user.com"
            };

            var adminUser = userManager.Create(user, "User1!");
            //userManager.CreateAsync(user, "User1!").Wait();

            if(adminUser.Succeeded) { var result1 = userManager.AddToRole(user.Id, "Administrator"); }
            base.Seed(context);
        }
    }
}


