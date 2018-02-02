using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;

namespace Blog_VT18.Models {
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser {
        public string Name { set; get; }

        public virtual ICollection<TimeSuggestion> TimeSuggestions { get; set; } 

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager) {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser> {
        public ApplicationDbContext()
            : base("DBUnicorn", throwIfV1Schema: false) {
        }

        public static ApplicationDbContext Create() {
            return new ApplicationDbContext();
        }

        public DbSet<BlogPost> BlogPosts { get; set; }
        public DbSet<Meeting> Meetings { get; set; }
        public DbSet<Invitation> Invitations { get; set; }
        public DbSet<Categories> Categories { get; set; }
        public DbSet<TimeSuggestion> TimeSuggestions { get; set; }
       // public DbSet<Date> Dates { get; set; }

    }

    public class Categories {
        public int ID { get; set; }
        public string Name { get; set; }
        public int? Category { get; set; }
        public virtual List<BlogPost> BlogPosts { get; set; }
    }

    public class Invitation {
        public int ID { get; set; }
        public bool? Accepted { get; set; }
        public DateTime Dates { get; set; }
        public virtual ApplicationUser Booker { get; set; }
        public virtual ApplicationUser Invited { get; set; }

    }
    public class TimeSuggestion {
        public int ID { get; set; }
  //      public virtual ICollection<Date> Dates{ get; set; }
        public virtual ApplicationUser Sender { get; set; }
      
       // public ApplicationUser invited { get; set; }
          public virtual ICollection<ApplicationUser> Invited { get; set; }
    }

    //public class Date {
    //    public int Id { get; set; }
    //    public DateTime TheDate { get; set; }
    //}


    public class Meeting {
        public int ID { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public string Info { get; set; }
        public virtual ApplicationUser Booker { get; set; }
        public virtual ApplicationUser Invited { get; set; }
    }
    
    public class BlogPost {

        public BlogPost()
        { }
            // Copy Constructor
            public BlogPost(BlogPost CP) {
            this.ID = CP.ID;
            this.Title = CP.Title;
            this.From = CP.From;
            this.Hidden = CP.Hidden;
            this.Category = CP.Category;
            this.Content = CP.Content;
        }
        public int ID { get; set; }
        [Required(ErrorMessage = "This field is required")]
        [StringLength(100, MinimumLength = 1, ErrorMessage = "Blogpost should contain between 1 and 100 characters")]
        public string Title { get; set; }
        [Required (ErrorMessage = "This field is required")]
        [StringLength(1500, MinimumLength = 1, ErrorMessage = "Blogpost should contain between 1 and 1500 characters")]
        public string Content { get; set; }
        public bool Hidden { get; set; } = false;
        public virtual Categories Category { get; set; }
        public virtual ApplicationUser From { get; set; }
    }
}