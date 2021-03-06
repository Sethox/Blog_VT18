﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Collections;
using System.Web;

namespace Blog_VT18.Models {
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser {
        public string Name { set; get; }
        public bool IsEnabled { set; get; } = true;
        public string Title { set; get; }

        public virtual ICollection<TimeSuggestion> TimeSuggestions { get; set; }

        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager) {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        internal ApplicationUser ToList() {
            throw new NotImplementedException();
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
        //public DbSet<Meeting> CalendarEvents { get; set; }
        public DbSet<InvitedToMeetings> InvitedToMeetings { get; set; }
    }

    public class Categories {
        public int ID { get; set; }
        [Required(ErrorMessage = "This field is required")]
        [StringLength(20, MinimumLength = 1, ErrorMessage = "Blogpost should contain between 1 and 20 characters")]
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
        public virtual ICollection<Date> Dates { get; set; }
        public virtual ApplicationUser Sender { get; set; }
        public virtual ApplicationUser Invited { get; set; }
        public virtual Meeting Meeting { get; set; }
        public bool Accepted { get; set; }
        public bool Denied { get; set; }
    }

    public class Date {
        public int Id { get; set; }
        [DisplayName("Date")]
        [DataType(DataType.Date)]
        public DateTime TheDate { get; set; }
    }


    public class Meeting {
        public Meeting() { }
        public int ID { get; set; }
        public string text { set; get; }
        public DateTime start_date { set; get; }
        public DateTime end_date { set; get; }
        public virtual ApplicationUser Booker { get; set; }
    }

    public class InvitedToMeetings {
        public int Id { get; set; }
        public virtual int MeetingID { get; set; }
        public virtual ApplicationUser Invited { get; set; }
    }

    public class BlogPost {
        public BlogPost() { }
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
        [Required(ErrorMessage = "This field is required")]
        [StringLength(5000, MinimumLength = 1, ErrorMessage = "Blogpost should contain between 1 and 5000 characters")]
        public string Content { get; set; }
        public bool Hidden { get; set; } = false;
        public virtual Categories Category { get; set; }
        public virtual ApplicationUser From { get; set; }
        public string Filename { get; set; }
        public string ContentType { get; set; }
        public byte[] File { get; set; }
    }
}