namespace Blog_VT18.Migrations {
    using Blog_VT18.Models;
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext> {
        public Configuration() {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context) {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
            var userManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));

            if(userManager.FindByName("Admin") == null) {
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
                if(adminUser.Succeeded) { userManager.AddToRole(user.Id, "Administrator"); }
            }
            if(!roleManager.RoleExists("User")) {
                var role = new IdentityRole();
                role.Name = "User";
                roleManager.Create(role);
            }

            //If there is 0 blogPosts, we will create a new blogPost
            if(context.BlogPosts.Where(x => x.ID == 0).Count() == 0) {
                var blogPost = new BlogPost {
                    ID = 0,
                    Content = "Hello",
                    Title = "Mitt meddelande",
                    Hidden = false,
                    From = null
                };
                context.BlogPosts.Add(blogPost);
                context.SaveChanges();
            }

            base.Seed(context);
<<<<<<< HEAD

            if(context.Categories.Count() < 1)
            {
                var cat1 = new Categories
                {
=======
            if(context.Categories.Where(x => x.ID == 0).Count() == 0) {
                var cat1 = new Categories {
>>>>>>> d4e0b9f4e261cf0a19074c111f92e7069d3cee65
                    ID = 1,
                    Name = "Education",
                    Category = null
                };
                context.Categories.Add(cat1);
                var cat2 = new Categories {
                    ID = 2,
                    Name = "Research",
                    Category = null
                };
                context.Categories.Add(cat2);

                var cat3 = new Categories {
                    ID = 3,
                    Name = "Informal",
                    Category = null
                };

                context.Categories.Add(cat3);
                context.SaveChanges();

                for(int i = 1; i < 4; i++) {
                    var subCat = new Categories {
                        Name = "Miscellaneous",
                        Category = i
                    };
                    context.Categories.Add(subCat);
                }
                context.SaveChanges();
            }

        }
    }
}
