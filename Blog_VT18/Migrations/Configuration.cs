namespace Blog_VT18.Migrations
{
    using Blog_VT18.Models;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Collections.Generic;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using System.Data.Entity;

    internal sealed class Configuration : DbMigrationsConfiguration<Blog_VT18.Models.ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Blog_VT18.Models.ApplicationDbContext context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.

            var storeUser = new UserStore<ApplicationUser>(context);
            var userManagerApp = new ApplicationUserManager(storeUser);

            var user = new ApplicationUser
            {
                Name = "FirstUser",
                UserName = "first@user.com",
                Email = "first@user.com"

            };
            userManagerApp.CreateAsync(user, "User1!").Wait();

            base.Seed(context);
        }
    }
}


